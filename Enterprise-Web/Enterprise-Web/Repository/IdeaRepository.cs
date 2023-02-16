using Enterprise_Web.DTOs;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Repository.IRepository;
using EnterpriseWeb.Data;

namespace Enterprise_Web.Repository;

public class IdeaRepository : IIdeaRepository
{
    private ApplicationDbContext _context;

    public IdeaRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public (List<IdeaDTO>, PaginationFilter, int) GetAll(PaginationFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task<IdeaDTO> GetById(int id)
    {
        throw new NotImplementedException();
    }
    // this is create for new Idea
    public Task Add(IdeaDTO model)
    {
        throw new NotImplementedException();
    }

    public Task Update(int id, IdeaDTO model)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }
}