using Enterprise_Web.DTOs;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Repository.IRepository;
using Enterprise_Web.ViewModels;
using EnterpriseWeb.Data;
using EnterpriseWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Metrics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Enterprise_Web.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public UserRepository(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public (List<UserDTO>, PaginationFilter, int) GetAll(PaginationFilter filter, int userId)
        {
            var findUser = _dbContext.Users.Include(x => x.Department).FirstOrDefault(x => x.Id == userId);
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize, filter.Search, filter.Role);

            if(findUser.Role == "QAC")
            {
                var listUserQAC = (from u in _dbContext.Users.Include(x=>x.Department) where u.Department.Id == findUser.Department.Id
                                select
                                new UserDTO
                                {
                                    Id = u.Id,
                                    Email = u.Email,
                                    Role = u.Role,
                                    DepartmentName = u.Department.Name == null ? "" : u.Department.Name
                                })
                           .OrderByDescending(x => x.Id)
                           .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                           .Take(validFilter.PageSize)
                           .ToList();

                var countUserQAC = (from u in _dbContext.Users where u.Department.Id == findUser.Department.Id select u).Count();

                return (listUserQAC, validFilter, countUserQAC);
            }

            var listUser = (from u in _dbContext.Users.Include(x => x.Department)
                            select
                            new UserDTO
                            {
                                Id = u.Id,
                                Email = u.Email,
                                Role = u.Role,
                                DepartmentName = u.Department.Name == null ? "" : u.Department.Name
                            })
                           .OrderByDescending(x => x.Id)
                           .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                           .Take(validFilter.PageSize)
                           .ToList();

            var countUser = (from u in _dbContext.Users select u).Count();

            return (listUser, validFilter, countUser);
        }

        public (List<UserDTO>, PaginationFilter, int) GetUser(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize, filter.Search, filter.Role);

            var countUser = 0;

            if (!String.IsNullOrEmpty(filter.Role) && !String.IsNullOrEmpty(filter.Search))
            {
                var filterUser = (from u in _dbContext.Users
                                  where u.Email.Contains(filter.Search) && u.Role.Contains(filter.Role)
                                  select new UserDTO
                                  {
                                      Id = u.Id,
                                      Email = u.Email,
                                      Role = u.Role,
                                      DepartmentName = u.Department.Name == null ? "" : u.Department.Name
                                  }).ToList();
                return (filterUser, validFilter, countUser);
            }

            if (!String.IsNullOrEmpty(filter.Role))
            {
                var filterUser = (from u in _dbContext.Users
                                  where u.Role.Contains(filter.Role)
                                  select new UserDTO
                                  {
                                      Id = u.Id,
                                      Email = u.Email,
                                      Role = u.Role,
                                      DepartmentName = u.Department.Name == null ? "" : u.Department.Name
                                  }).ToList();
                return (filterUser, validFilter, countUser);
            }

            var listUser = (from u in _dbContext.Users
                            select new UserDTO
                            {
                                Id = u.Id,
                                Email = u.Email,
                                Role = u.Email,
                                DepartmentName = u.Department.Name == null ? "" : u.Department.Name
                            }).ToList();
            return (listUser, validFilter, countUser);
        }

        public User GetUserById(int id)
        {
            var findUser = _dbContext.Users.Include(x=> x.Department).FirstOrDefault(x => x.Id == id);
            if (findUser == null)
            {
                return null;
            }
            if(findUser.Department == null)
            {
                return null;
            }
            return findUser;
        }

        public async Task Create(User user)
        {
            var newUser = new User
            {
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Role = user.Role
            };
            await _dbContext.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(User user)
        {
            var findUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
            if (findUser != null)
            {
                findUser.Role = user.Role;
            }
            _dbContext.Update(findUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> ResetPassword(int id, string newPwd)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != null)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPwd);
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                return user.Password;
            }
            else return null;
        }

        public async Task Delete(int id)
        {
            var findUser = await _dbContext.Users.FindAsync(id);
            _dbContext.Remove(findUser);
            await _dbContext.SaveChangesAsync();
        }

        public bool CheckEmailExist(User user)
        {
            var findEmail = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email && u.Id != user.Id);
            if (findEmail == null)
            {
                return false;
            }
            return true;
        }

        public string Authenticate(UserViewModel userViewModel)
        {
            var loginResult = _dbContext.Users.Where(u => u.Email == userViewModel.Email).FirstOrDefault();
            if (loginResult != null)
            {
                var isCheckPwd = BCrypt.Net.BCrypt.Verify(userViewModel.Password, loginResult.Password);
                if (isCheckPwd)
                {
                    var accessToken = GenerateAccessToken(loginResult);
                    return accessToken;
                }
                return null;
            }
            return null;
        }

        private string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var login = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: login);

            string Token = new JwtSecurityTokenHandler().WriteToken(token);
            return Token;
        }
    }
}
