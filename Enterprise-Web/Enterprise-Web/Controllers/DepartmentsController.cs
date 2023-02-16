using Enterprise_Web.DTOs;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Pagination.Helpers;
using Enterprise_Web.Pagination.Service;
using Enterprise_Web.Repository;
using Enterprise_Web.Repository.IRepository;
using EnterpriseWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUriService _uriService;

        public DepartmentsController(IDepartmentRepository departmentRepository, IUriService uriService)
        {
            _departmentRepository = departmentRepository;
            _uriService = uriService;
        }

        #region Post API

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] Department department)
        {
            if (_departmentRepository.CheckNameExist(department))
            {
                return StatusCode(400, "Department Name already exists");
            }

            await _departmentRepository.Create(department);
            return Ok(department);
        }

        #endregion

        #region Get API

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var listDepts = _departmentRepository.GetAll(filter);
            var pagedResponse = PaginationHelper.CreatePagedReponse(listDepts.Item1, listDepts.Item2, listDepts.Item3, _uriService, route);
            return Ok(pagedResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeptDetail(int id)
        {
            var dept = _departmentRepository.GetDeptById(id);
            if (dept == null)
            {
                return StatusCode(400, "Department not found");
            }
            return Ok(dept);
        }

        #endregion

        #region Delete API

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDept(int id)
        {
            var dept = _departmentRepository.GetDeptById(id);
            if (dept == null)
            {
                return StatusCode(400, "Department not found");
            }
            await _departmentRepository.Delete(id);
            return Ok();
        }

        #endregion

        #region Put API
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(Department department, int id)
        {
            if (id != department.Id)
            {
                return BadRequest("Department not found");
            }

            if (_departmentRepository.CheckNameExist(department))
            {
                return StatusCode(400, "Department Name already exists");
            }

            await _departmentRepository.Update(department);
            return Ok();
        }
        #endregion
    }
}
