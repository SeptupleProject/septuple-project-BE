using Enterprise_Web.DTOs;
using Enterprise_Web.Models;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Pagination.Helpers;
using Enterprise_Web.Pagination.Service;
using Enterprise_Web.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;

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

        [HttpGet("Download")]
        public IActionResult Download()
        {
            var download = _ideaRepository.Download("Images");
            return Ok(download);
        }

        [HttpGet("Download_csv")]
        public IActionResult DownloadCsv()
        {
            var ideaViewModels = _ideaRepository.GetIdeasToDownload();
            _ideaRepository.DownloadIdeas(ideaViewModels);
            return Ok(ideaViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Idea idea)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Subject.Claims.ToList()[2];
            var userEmail = claim.Subject.Claims.ToList()[1];

            await _ideaRepository.Create(idea, int.Parse(userId.Value), userEmail.Value);
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

        [HttpPut("incrementview/{id}")]
        public async Task<IActionResult> IncrementView(int id)
        {
            await _ideaRepository.IcrementedView(id);
            return Ok(id);
        }

        [HttpGet("mostLike")]
        public async Task<IActionResult> MostLikeIdea()
        {
            var mostLikeIdea = await _ideaRepository.MostLikeIdea();
            return Ok(mostLikeIdea);
        }

        [HttpGet("mostDislike")]
        public async Task<IActionResult> MostDisikeIdea()
        {
            var mostLikeIdea = await _ideaRepository.MostDislikeIdea();
            return Ok(mostLikeIdea);
        }

        [HttpGet("mostComments")]
        public async Task<IActionResult> MostCommentsIdea()
        {
            var mostCommentIdea = await _ideaRepository.MostCommentIdea();
            return Ok(mostCommentIdea);
        }

        [HttpGet("mostViews")]
        public async Task<IActionResult> MostViewsIdea()
        {
            var mostViewsIdea = await _ideaRepository.MostViewsIdea();
            return Ok(mostViewsIdea);
        }

        [HttpPost]
        [Route("likeIdea/{idIdea}")]
        public async Task<IActionResult> LikeIdea(int ideaId)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Subject.Claims.ToList()[2].Value;

            var ideaToLike = _ideaRepository.GetById(ideaId);
            var result = await _ideaRepository.LikeIdea(int.Parse(userId), ideaToLike);
        
            return Ok(result);
        }
        
        [HttpPost]
        [Route("dislikeIdea/{idIdea}")]
        public async Task<IActionResult> DislikeIdea(int ideaId)
        {

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Subject.Claims.ToList()[2].Value;

            var ideaToLike = _ideaRepository.GetById(ideaId);
            var result = await _ideaRepository.DislikeIdea(int.Parse(userId), ideaToLike);

            return Ok(result);
        }

        [HttpGet("IdeasCommentsByDept")]
        public async Task<IActionResult> GetIdeasCmtsByDept()
        {
            var ideasCmtsByDept = _ideaRepository.IdeasCmtsPerDept();
            return Ok(ideasCmtsByDept);
        }
    }
}