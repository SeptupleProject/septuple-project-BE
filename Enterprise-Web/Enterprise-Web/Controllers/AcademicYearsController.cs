using Enterprise_Web.DTOs;
using Enterprise_Web.Models;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Pagination.Helpers;
using Enterprise_Web.Pagination.Service;
using Enterprise_Web.Repository;
using Enterprise_Web.Repository.IRepository;
using EnterpriseWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Enterprise_Web.Controllers
{
    [Authorize(Roles ="Admin,QAM")]
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicYearsController : ControllerBase
    {
        private readonly IAcademicYearRepository _academicYearRepository;
        private readonly IUriService _uriService;

        public AcademicYearsController(IAcademicYearRepository academicYearRepository, IUriService uriService)
        {
            _academicYearRepository = academicYearRepository;
            _uriService = uriService;
        }

        #region Get API

        [HttpGet]
        public async Task<IActionResult> GetAllAcade([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var listAcade = _academicYearRepository.GetAll(filter);
            var pagedResponse = PaginationHelper.CreatePagedReponse(listAcade.Item1, listAcade.Item2, listAcade.Item3, _uriService, route);
            return Ok(pagedResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAcadeDetails(int id)
        {
            var acade = _academicYearRepository.GetAcadaById(id);
            if (acade == null)
            {
                return StatusCode(400, "User does not exist");
            }
            return Ok(acade);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrnetAcade()
        {
            var acade = _academicYearRepository.GetCurrentAcade();
            if (acade == null)
            {
                return StatusCode(400, "Acade does not exist");
            }
            return Ok(acade);
        }

        #endregion

        #region Post API

        [HttpPost]
        public async Task<IActionResult> CreateAcade([FromBody] AcademicYear academicYear)
        {
            if (_academicYearRepository.CheckNameExist(academicYear))
            {
                return StatusCode(400, "Name already exist");
            }
            await _academicYearRepository.Create(academicYear);
            return Ok(academicYear);
        }

        #endregion

        #region Update API

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAcade([FromBody] AcademicYear academicYear, int id)
        {
            if (id != academicYear.Id)
            {
                return BadRequest("Academic Year Not found");
            }
            if (_academicYearRepository.CheckNameExist(academicYear))
            {
                return StatusCode(400, "Name already exist");
            }
            await _academicYearRepository.Update(academicYear);
            return Ok();
        }

        #endregion

        #region Delete API

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAcade(int id)
        {
            var acade = _academicYearRepository.GetAcadaById(id);
            if (acade == null)
            {
                return StatusCode(400, "Academic year does not exist");
            }
            await _academicYearRepository.Delete(id);
            return Ok();
        }

        #endregion
    }
}
