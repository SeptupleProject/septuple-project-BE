using Enterprise_Web.DTOs;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.ViewModels;
using EnterpriseWeb.Models;

namespace Enterprise_Web.Repository.IRepository
{
    public interface IUserRepository
    {
        (List<UserDTO>, PaginationFilter, int) GetAll(PaginationFilter filter, int userId);
        (List<UserDTO>, PaginationFilter, int) GetUser(PaginationFilter filter);
        User GetUserById(int id);
        Task Create(User user);
        Task Update(User user);
        Task Delete(int id);
        bool CheckEmailExist(User user);
        string Authenticate(UserViewModel userViewModel);
        Task<string> ResetPassword(int id, string newPwd);
    }
}
