using Enterprise_Web.DTOs;
using Enterprise_Web.Models;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Pagination.Helpers;
using Enterprise_Web.Pagination.Service;
using Enterprise_Web.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Enterprise_Web.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IdeasController : ControllerBase
    {
        private readonly IIdeaRepository _ideaRepository;
        private readonly IUriService _uriService;

        public IdeasController(IIdeaRepository ideaRepository, IUriService uriService)
        {
            _ideaRepository = ideaRepository;
            _uriService = uriService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var listIdea = _ideaRepository.GetAll(filter);
            var pagedResponse = PaginationHelper.CreatePagedReponse<IdeaDTO>(listIdea.Item1, listIdea.Item2, listIdea.Item3, _uriService, route);
            return Ok(pagedResponse);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var idea = _ideaRepository.GetById(id);
            return Ok(idea);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]Idea idea)
        {
            await _ideaRepository.Create(idea);
            return Ok(idea);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromForm] Idea idea, int id)
        {
            if (id != idea.Id)
            {
                return BadRequest("Ideas Not found");
            }
            await _ideaRepository.Update(idea);

            return Ok(idea);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var idea = _ideaRepository.GetById(id);
            if (idea == null)
            {
                return StatusCode(400, "Ideas does not exist");
            }
            await _ideaRepository.Delete(id);
            return Ok();
        }
    }
}