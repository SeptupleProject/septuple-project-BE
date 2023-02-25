using Enterprise_Web.DTOs;
using Enterprise_Web.Models;
using Enterprise_Web.Pagination.Filter;
using EnterpriseWeb.Models;

namespace Enterprise_Web.Repository.IRepository
{
    public interface ICommentRepository
    {
        (List<CommentDTO>, PaginationFilter, int) GetAll(PaginationFilter filter);
        Comment GetById(int id);
        Task Create(Comment comment, int userId, string userEmail);
        Task Update(Comment comment);
        Task Delete(int id); 
    }
}
