using Enterprise_Web.DTOs;
using Enterprise_Web.Models;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Repository.IRepository;
using EnterpriseWeb.Data;
using Microsoft.EntityFrameworkCore;

namespace Enterprise_Web.Repository
{
    public class IdeaRepository : IIdeaRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public IdeaRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public (List<IdeaDTO>, PaginationFilter, int) GetAll(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize, filter.Search, filter.Role);
            var listIdeas = (from idea in _dbContext.Ideas
                             select
                                 new IdeaDTO
                                 {
                                     Id = idea.Id,
                                     Title = idea.Title,
                                     Content = idea.Content,
                                     CategoryName = idea.Category.Name
                                 })
                               .OrderByDescending(x => x.Id)
                               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                               .Take(validFilter.PageSize)
                               .ToList();
            var countIdeas = (from idea in _dbContext.Ideas select idea).Count();

            return (listIdeas, validFilter, countIdeas);
        }

        public Idea GetById(int id)
        {
            var ideaToGet = _dbContext.Ideas.FirstOrDefault(i => i.Id == id);
            if (ideaToGet == null)
            {
                return null;
            }
            return ideaToGet;

        }

        public async Task Create(Idea idea)
        {
            var newIdea = new Idea()
            {
                Title = idea.Title,
                Content = idea.Content,
                Views = idea.Views,
                Image = idea.Image,
                IsAnonymos = idea.IsAnonymos,
                AcademicYearId = idea.AcademicYearId,
                CategoryId = idea.CategoryId,
                CreatedBy = idea.CreatedBy,
                CreatedAt = idea.CreatedAt,
            
            };
            await _dbContext.AddAsync(newIdea);
            await _dbContext.SaveChangesAsync();
        }


        public async Task Update(Idea idea)
        {
            var findIdeas = await _dbContext.Ideas.FirstOrDefaultAsync(x => x.Id == idea.Id);
            if (findIdeas != null)
            {
                findIdeas.Title = idea.Title;
                findIdeas.Content = idea.Content;
                findIdeas.Views = idea.Views;
                findIdeas.Image = idea.Image;
                findIdeas.IsAnonymos = idea.IsAnonymos;
                findIdeas.AcademicYearId = idea.AcademicYearId;
                findIdeas.CategoryId = idea.CategoryId;
                findIdeas.CreatedBy = idea.CreatedBy;
                findIdeas.CreatedAt = idea.CreatedAt;
            }
            _dbContext.Update(findIdeas);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var ideaDelete = await _dbContext.Ideas.FindAsync(id);
            _dbContext.Remove(ideaDelete);
             await _dbContext.SaveChangesAsync();
        }
    }
}
