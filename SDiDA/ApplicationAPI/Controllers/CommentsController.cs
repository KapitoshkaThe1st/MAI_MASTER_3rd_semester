using ApplicationAPI.DTOs;
using ApplicationAPI.Entities;
using ApplicationAPI.Repository;
using ApplicationAPI.Requests;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public CommentsController(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        // GET: api/<CommentsController>
        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> GetComments([FromQuery] CommentsRequestParameters parameters)
        {
            var comments = await _repositoryManager.CommentsRepository.GetComments(parameters);
            return Ok(_mapper.Map<IEnumerable<CommentDto>>(comments));
        }

        // GET api/<CommentsController>/5
        [Authorize]
        [HttpGet("{id}", Name = "GetCommentById")]
        public async Task<IActionResult> GetCommentById(string id)
        {
            var comment = await _repositoryManager.CommentsRepository.GetCommentById(id);
            if (comment is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommentDto>(comment));
        }

        // POST api/<CommentsController>
        [Authorize]
        [HttpPost("")]
        public async Task<IActionResult> CreateComment([FromBody] CommentCreationDto commentCreationDto)
        {
            if (commentCreationDto is null)
            {
                return BadRequest("CommentCreationDto is null");
            }

            var comment = _mapper.Map<Comment>(commentCreationDto);
            await _repositoryManager.CommentsRepository.CreateComment(comment);
            return CreatedAtRoute("GetCommentById", new { id = comment.Id }, _mapper.Map<CommentDto>(comment));
        }

        // PUT api/<CommentsController>/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment([FromBody] CommentUpdateDto commentUpdateDto)
        {
            if (commentUpdateDto is null)
            {
                return BadRequest("commentUpdateDto is null");
            }

            var comment = _mapper.Map<Comment>(commentUpdateDto);
            await _repositoryManager.CommentsRepository.UpdateComment(comment);
            return NoContent();
        }

        // DELETE api/<CommentsController>/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] string id)
        {
            await _repositoryManager.CommentsRepository.DeleteComment(id);
            return NoContent();
        }

        // PATCH api/<CommentsController>/5
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchComment([FromRoute] string id, [FromBody] JsonPatchDocument<Comment> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest("Patch doc was null");
            }

            var comment = await _repositoryManager.CommentsRepository.GetCommentById(id);
            patchDoc.ApplyTo(comment);

            await _repositoryManager.CommentsRepository.UpdateComment(comment);

            return new ObjectResult(comment);
        }
    }
}
