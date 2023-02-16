using Enterprise_Web.DTOs;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Repository.IRepository;
using EnterpriseWeb.Data;
using EnterpriseWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace Enterprise_Web.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public DepartmentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool CheckNameExist(Department department)
        {
            var result = _dbContext.Departments.Where(d => d.Name.ToLower().Equals(department.Name.ToLower()) && d.Id != department.Id).ToList();
            if (result.Any())
            {
                return true;
            }
            return false;
        }

        public async Task Create(Department department)
        {
            ICollection<User> users = new List<User>();
            foreach (var i in department.Users)
            {
                var user = _dbContext.Users.Where(x => x.Id == i.Id).First();
                users.Add(user);
            }

            var newDept = new Department
            {
                Name = department.Name,
                Users = users,
                CreatedBy = department.CreatedBy,
                CreatedAt = DateTime.Now,
            };

            _dbContext.Add(newDept);
            _dbContext.SaveChanges();
        }

        public async Task Delete(int id)
        {
            var dept = await _dbContext.Departments.FindAsync(id);
            _dbContext.Remove(dept);
            await _dbContext.SaveChangesAsync();
        }

        public (List<DepartmentDTO>, PaginationFilter, int) GetAll(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize, filter.Search, filter.Role);

            var listDepts = (from d in _dbContext.Departments
                            select
                            new DepartmentDTO
                            {
                                Id = d.Id,
                                Name = d.Name,
                                Users = d.Users.Count,
                                ManagedBy = d.Users.SingleOrDefault(u => u.Role == "QAC").Email == null ? "": d.Users.SingleOrDefault(u => u.Role == "QACoordinator").Email
                            })
                           .OrderByDescending(d => d.Id)
                           .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                           .Take(validFilter.PageSize)
                           .ToList();

            var countDepts = (from d in _dbContext.Departments select d).Count();

            return (listDepts, validFilter, countDepts);
        }

        public Department GetDeptById(int id)
        {
            var department = _dbContext.Departments.Include(x => x.Users).Where(x => x.Id == id).FirstOrDefault();
            if (department == null)
            {
                return null;
            }
            return department;
        }

        public async Task Update(Department department)
        {
            List<User>? users = department.Users as List<User>;
            List<int> listNewUserInput = new List<int>();

            foreach (var user in users)
            {
                listNewUserInput.Add(user.Id);
            }

            var checkUser = _dbContext.Users.Where(e => e.Department.Id == department.Id).ToList();
            List<User> listCurrentUser = checkUser;

            List<User> listRemoveUser = new List<User>();

            ICollection<User> newUser = new List<User>();
            List<User>? listNewUser = newUser as List<User>;

            foreach (var user in listCurrentUser)
            {
                if (listNewUserInput.Contains(user.Id))
                {
                    listNewUser.Add(user);
                    //continue;
                }
                else
                {
                    listRemoveUser.Add(user);
                }
            }

            foreach (User user in listRemoveUser)
            {
                var userH = _dbContext.Users.Where(x => x.Id == user.Id).Include(x => x.Department).FirstOrDefault();
                var dept = _dbContext.Departments.Find(department.Id);
                dept.Users.Remove(userH);
            }
            _dbContext.SaveChanges();

            foreach (var user in listNewUserInput)
            {
                if (!listCurrentUser.Select(x => x.Id).Contains(user))
                {
                    var findUser = _dbContext.Users.Find(user);
                    listNewUser.Add(findUser);
                }
            }

            foreach (User user in listNewUser)
            {
                var userH = _dbContext.Users.Where(x => x.Id == user.Id).Include(x => x.Department).FirstOrDefault();
                var divisionD = _dbContext.Departments.Find(department.Id);
                divisionD.Users.Add(userH);
            }
            _dbContext.SaveChanges();


            var newDepartment = new Department()
            {
                Id = department.Id,
                Name = department.Name,
                Users = newUser,
                CreatedBy = department.CreatedBy,
                CreatedAt = department.CreatedAt
            };
            _dbContext.ChangeTracker.Clear();
            _dbContext.Entry(newDepartment).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
    }
}
