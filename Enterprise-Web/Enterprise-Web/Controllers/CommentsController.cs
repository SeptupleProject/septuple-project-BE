using Enterprise_Web.DTOs;
using Enterprise_Web.Models;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Pagination.Helpers;
using Enterprise_Web.Pagination.Service;
using Enterprise_Web.Repository.IRepository;
using EnterpriseWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise_Web.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUriService _uriService;
        
        public CommentsController(ICommentRepository commentRepository, IUriService uriService)
        {
            _commentRepository = commentRepository;
            _uriService = uriService; 
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var listComment = _commentRepository.GetAll(filter);
            var pagedResponse = PaginationHelper.CreatePagedReponse<CommentDTO>(listComment.Item1, listComment.Item2, listComment.Item3, _uriService, route); 
            return Ok(pagedResponse);
        }
        
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var comment = _commentRepository.GetById(id);
            return Ok(comment); 
        }

        [HttpPost]
        public async Task<IActionResult> Create(Comment comment)
        {
            await _commentRepository.Create(comment);
            return Ok(comment); 
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] Comment comment, int id )
        {
            if(id != comment.Id)
            {
                return BadRequest("Comments Not found");
            }
            await _commentRepository.Update(comment);

            return Ok(comment); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult>  Delete(int id)
        {
            var idea =  _commentRepository.GetById(id);
            if(idea == null)
            {
                return StatusCode(400, "Comments does not exist");
            }
            await _commentRepository.Delete(id);
            return Ok();
        }
        
        
        
        
        
   
   
    }