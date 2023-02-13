﻿using Enterprise_Web.DTOs;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Repository.IRepository;
using EnterpriseWeb.Data;
using EnterpriseWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace Enterprise_Web.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public (List<UserDTO>, PaginationFilter, int) GetAll(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize, filter.Search);

            var listUser = (from u in _dbContext.Users select 
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

            return (listUser,validFilter,countUser);
        }

        public async Task<User> GetUserById(int id)
        {
            var findUser = await _dbContext.Users.FindAsync(id);
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
            if(findUser != null)
            {
                findUser.Role = user.Role;
            }
            _dbContext.Update(findUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var findUser = await _dbContext.Users.FindAsync(id);
            _dbContext.Remove(findUser);
            await _dbContext.SaveChangesAsync();
        }

        public bool CheckEmailExist(User user)
        {
            var findEmail = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
            if (findEmail == null)
            {
                return false;
            }
            return true;
        }
    }
}
