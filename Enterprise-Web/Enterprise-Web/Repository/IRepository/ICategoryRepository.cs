using Enterprise_Web.DTOs;
using Enterprise_Web.Pagination.Filter;
using EnterpriseWeb.Models;

namespace Enterprise_Web.Repository.IRepository
{
  public interface ICategoryRepository
  {
    (List<CategoryDTO>, PaginationFilter, int) GetAll(PaginationFilter filter);
    Category GetCategoryById(int id);
    Task Create(Category category);
    Task Update(Category category);
    Task Delete(int id);
    bool CheckNameExist(Category category);
  }
}
