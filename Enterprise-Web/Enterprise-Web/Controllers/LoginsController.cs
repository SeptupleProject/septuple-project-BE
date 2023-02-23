using Enterprise_Web.Pagination.Service;
using Enterprise_Web.Repository.IRepository;
using Enterprise_Web.ViewModels;
using EnterpriseWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Enterprise_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public LoginsController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        #region Login

        [HttpPost]
        public async Task<IActionResult> LoginUser(UserViewModel userViewModel)
        {
            var accessToken = _userRepository.Authenticate(userViewModel);
            if (accessToken == null)
            {
                return BadRequest("Email or password is not correct");
            }
            return Ok(accessToken);
        }

        #endregion
    }
}
