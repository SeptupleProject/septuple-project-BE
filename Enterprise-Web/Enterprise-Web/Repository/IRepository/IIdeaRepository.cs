using Enterprise_Web.DTOs;
using Enterprise_Web.Models;
using Enterprise_Web.Pagination.Filter;

namespace Enterprise_Web.Repository.IRepository;

public interface IIdeaRepository
{
    (List<IdeaDTO>, PaginationFilter, int) GetAll(PaginationFilter filter);
    Task<IdeaDTO> GetById(int id);
    Task Add(IdeaDTO model);
    Task Update(int id, IdeaDTO model);
    Task Delete(int id); 
    
}