using Enterprise_Web.DTOs;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Pagination.Helpers;
using Enterprise_Web.Pagination.Service;
using Enterprise_Web.Repository.IRepository;
using EnterpriseWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Enterprise_Web.Controllers
{
    [Authorize(Roles ="Admin,QAM,QAC")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUriService _uriService;

        public UsersController(IUserRepository userRepository, IUriService uriService)
        {
            _userRepository = userRepository;
            _uriService = uriService;
        }

        #region Get API

        [HttpGet]
        public async Task<IActionResult> GetAllUser([FromQuery] PaginationFilter filter)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Subject.Claims.ToList()[2];
            var route = Request.Path.Value;
            var user = _userRepository.GetUserById(Int32.Parse(userId.Value));
            if (user == null)
            {
                return StatusCode(400, "QAC not belong to Department");
            }
            var listUser = _userRepository.GetAll(filter, Int32.Parse(userId.Value));
            var pagedResponse = PaginationHelper.CreatePagedReponse<UserDTO>(listUser.Item1, listUser.Item2, listUser.Item3, _uriService, route);
            return Ok(pagedResponse);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetUser([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var listUser = _userRepository.GetUser(filter);
            var pagedResponse = PaginationHelper.CreatePagedReponse<UserDTO>(listUser.Item1, listUser.Item2, listUser.Item3, _uriService, route);
            return Ok(pagedResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetail(int id)
        {
            var user = _userRepository.GetUserById(id);
            if(user == null)
            {
                return StatusCode(400, "User does not exist");
            }
            return Ok(user);
        }

        #endregion

        #region Post API

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (_userRepository.CheckEmailExist(user))
            {
                return StatusCode(400, "Email already Exist");
            }
            await _userRepository.Create(user);
            return Ok(user);
        }

        #endregion

        #region Update API

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromBody] User user, int id)
        {
            if(id != user.Id)
            {
                return BadRequest("User Not found");
            }
            await _userRepository.Update(user);
            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ResetUserPwd(int id, string newPwd)
        {
            var updatedPwd = await _userRepository.ResetPassword(id, newPwd);
            if (updatedPwd == null)
            {
                return NotFound("Not found user");
            }
            return Ok(updatedPwd);
        }

        #endregion

        #region Delete API

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = _userRepository.GetUserById(id);
            if(user == null)
            {
                return StatusCode(400, "User does not exist");
            }
            await _userRepository.Delete(id);
            return Ok();
        }

        #endregion
    }
}
