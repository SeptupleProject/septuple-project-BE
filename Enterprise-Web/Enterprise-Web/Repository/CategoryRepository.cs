using Enterprise_Web.DTOs;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Repository.IRepository;
using EnterpriseWeb.Data;
using EnterpriseWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;

namespace Enterprise_Web.Repository
{
  public class CategoryRepository : ICategoryRepository
  {
    private readonly ApplicationDbContext _dbContext;
    public CategoryRepository(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }
    public bool CheckNameExist(Category category)
    {
      var result = _dbContext.Categories.Where(c => c.Name.ToLower().Equals(category.Name.ToLower()) && c.Id != category.Id);
      if (result.Any())
      {
        return true;
      }
      else
      {
          return false;
      }
    }
    public (List<CategoryDTO>, PaginationFilter, int) GetAll(PaginationFilter filter)
    {
      var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize, filter.Search, filter.Role);

      var listCategory = (from c in _dbContext.Categories
                          select
                          new CategoryDTO
                          {
                            Id = c.Id,
                            Name = c.Name,
                            NumOfIdeas = 0
                          })
                     .OrderByDescending(x => x.Id)
                     .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                     .Take(validFilter.PageSize)
                     .ToList();

      var countCategories = (from c in _dbContext.Categories select c).Count();

      return (listCategory, validFilter, countCategories);
    }

    public Category GetCategoryById(int id)
    {
      var findCategory = _dbContext.Categories.Find(id);
      if (findCategory == null)
      {
        return null;
      }
      return findCategory;
    }

    public async Task Create(Category category)
    {
      var newCategory = new Category
      {
        Name = category.Name,
        CreatedBy = category.CreatedBy,
        CreatedAt = DateTime.Now,
      };
      await _dbContext.AddAsync(newCategory);
      await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
      var findCategory = await _dbContext.Categories.FindAsync(id);
      _dbContext.Remove(findCategory);
      await _dbContext.SaveChangesAsync();
    }
    public async Task Update(Category category)
    {
      var findCategory = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
      if (findCategory != null)
      {
        findCategory.Name = category.Name;
      }
      _dbContext.Update(findCategory);
      await _dbContext.SaveChangesAsync();
    }
  }
}
