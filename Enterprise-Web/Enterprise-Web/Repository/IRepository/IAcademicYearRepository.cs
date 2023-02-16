using Enterprise_Web.Models;
using Enterprise_Web.Pagination.Filter;
using EnterpriseWeb.Models;

namespace Enterprise_Web.Repository.IRepository
{
    public interface IAcademicYearRepository
    {
        (List<AcademicYear>, PaginationFilter, int) GetAll(PaginationFilter filter);
        AcademicYear GetAcadaById(int id);
        Task Create(AcademicYear academicYear);
        Task Update(AcademicYear academicYear);
        Task Delete(int id);
        bool CheckNameExist(AcademicYear academicYear);
    }
}
