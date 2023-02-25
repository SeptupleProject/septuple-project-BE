using Enterprise_Web.DTOs;
using Enterprise_Web.Models;
using Enterprise_Web.Pagination.Filter;

namespace Enterprise_Web.Repository.IRepository
{
    public interface IIdeaRepository
    {
        (List<IdeaDTO>, PaginationFilter, int) GetAll(PaginationFilter filter);
        Idea GetById(int id);
        Task Create(Idea idea, int userId, string userEmail);
        Task Update(Idea idea);
        Task Delete(int id); 
    }
}
