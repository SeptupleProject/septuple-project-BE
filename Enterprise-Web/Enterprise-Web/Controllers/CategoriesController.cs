using Enterprise_Web.DTOs;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Pagination.Helpers;
using Enterprise_Web.Pagination.Service;
using Enterprise_Web.Repository;
using Enterprise_Web.Repository.IRepository;
using EnterpriseWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace Enterprise_Web.Controllers
{
    //[Authorize(Roles = "QAM")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUriService _uriService;
        public CategoriesController(ICategoryRepository categoryRepository, IUriService uriService)
        {
            _categoryRepository = categoryRepository;
            _uriService = uriService;
        }

        #region Get API
        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var listCategories = _categoryRepository.GetAll(filter);
            var pagedResponse = PaginationHelper.CreatePagedReponse<CategoryDTO>(listCategories.Item1, listCategories.Item2, listCategories.Item3, _uriService, route);
            return Ok(pagedResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryDetail(int id)
        {
            var category = _categoryRepository.GetCategoryById(id);
            if (category == null)
            {
                return StatusCode(400, "Category Does Not Exist");
            }
            return Ok(category);
        }

        #endregion

        #region Post API

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] Category category)
        {
            if (_categoryRepository.CheckNameExist(category))
            {
                return StatusCode(400, "Category Name already Exist");
            }

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userEmail = claim.Subject.Claims.ToList()[1];

            await _categoryRepository.Create(category, userEmail.Value);
            return Ok(category);
        }

        #endregion

        #region Update API

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Category category, int id)
        {
            if (id != category.Id)
            {
                return BadRequest("Category not found");
            }
            if (_categoryRepository.CheckNameExist(category))
            {
                return StatusCode(400, "Category Name already exists");
            }
            await _categoryRepository.Update(category);
            return Ok();
        }
        #endregion

        #region Delete API

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = _categoryRepository.GetCategoryById(id);
            if (category == null)
            {
                return StatusCode(400, "Category Does Not Exist");
            }
            await _categoryRepository.Delete(id);
            return Ok();
        }

        #endregion
    }
}
