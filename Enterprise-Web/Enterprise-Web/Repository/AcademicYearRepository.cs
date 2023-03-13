using Enterprise_Web.DTOs;
using Enterprise_Web.Models;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Repository.IRepository;
using EnterpriseWeb.Data;
using EnterpriseWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace Enterprise_Web.Repository
{
    public class AcademicYearRepository : IAcademicYearRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public AcademicYearRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public bool CheckNameExist(AcademicYear academicYear)
        {
            var findAcadeName = _dbContext.AcademicYears.FirstOrDefault(a => a.Name == academicYear.Name && a.Id != academicYear.Id);
            if (findAcadeName == null)
            {
                return false;
            }
            return true;
        }

        public async Task Create(AcademicYear academicYear)
        {
            var newAcada = new AcademicYear
            {
                Name = academicYear.Name,
                StartDate = academicYear.StartDate,
                EndDate = academicYear.EndDate,
            };
            await _dbContext.AddAsync(newAcada);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var findAcade = await _dbContext.AcademicYears.FindAsync(id);
            _dbContext.Remove(findAcade);
            await _dbContext.SaveChangesAsync();
        }

        public AcademicYear GetAcadaById(int id)
        {
            var findAcade = _dbContext.AcademicYears.Find(id);
            if (findAcade == null)
            {
                return null;
            }
            return findAcade;
        }

        public (List<AcademicYearDTO>, PaginationFilter, int) GetAll(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize, filter.Search, filter.Role);

            var listAcade = (from a in _dbContext.AcademicYears
                             select
                            new AcademicYearDTO
                            {
                                Id = a.Id,
                                Name = a.Name,
                                StartDate = a.StartDate,
                                EndDate = a.EndDate,
                                IdeaDeadline = a.IdeaDeadline == null ? null : a.IdeaDeadline,
                            })
                           .OrderByDescending(x => x.Id)
                           .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                           .Take(validFilter.PageSize)
                           .ToList();

            var countAcade = (from u in _dbContext.AcademicYears select u).Count();

            return (listAcade, validFilter, countAcade);
        }

        public async Task Update(AcademicYear academicYear)
        {
            var findAcade = await _dbContext.AcademicYears.FirstOrDefaultAsync(x => x.Id == academicYear.Id);

            if (findAcade != null)
            {
                findAcade.Name = academicYear.Name;
                findAcade.StartDate = academicYear.StartDate;
                findAcade.EndDate = academicYear.EndDate;
                findAcade.IdeaDeadline = academicYear.IdeaDeadline;
            }
            _dbContext.Update(findAcade);
            await _dbContext.SaveChangesAsync();
        }
    }
}
