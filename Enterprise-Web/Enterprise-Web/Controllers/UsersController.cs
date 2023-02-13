using Enterprise_Web.DTOs;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Pagination.Helpers;
using Enterprise_Web.Pagination.Service;
using Enterprise_Web.Repository.IRepository;
using EnterpriseWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise_Web.Controllers
{
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
            var route = Request.Path.Value;
            var listUser = _userRepository.GetAll(filter);
            var pagedResponse = PaginationHelper.CreatePagedReponse<UserDTO>(listUser.Item1, listUser.Item2, listUser.Item3, _uriService, route);
            return Ok(pagedResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetail(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if(user == null)
            {
                return NotFound();
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
                return BadRequest();
            }
            await _userRepository.Update(user);
            return Ok();
        }

        #endregion

        #region Delete API

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = _userRepository.GetUserById(id);
            if(user == null)
            {
                return NotFound();
            }
            await _userRepository.Delete(id);
            return Ok();
        }

        #endregion
    }
}
