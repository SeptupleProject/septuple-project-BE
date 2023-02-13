using Enterprise_Web.DTOs;
using Enterprise_Web.Pagination.Filter;
using EnterpriseWeb.Models;


namespace Enterprise_Web.Repository.IRepository
{
    public interface IUserRepository
    {
        (List<UserDTO>, PaginationFilter, int) GetAll(PaginationFilter filter);
        Task<User> GetUserById(int id);
        Task Create(User user);
        Task Update(User user);
        Task Delete(int id);
        bool CheckEmailExist(User user);
    }
}
