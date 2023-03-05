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
    public class PostsController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public PostsController(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        // GET: api/<PostsController>
        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> GetPosts([FromQuery] PostsRequestParameters parameters)
        {
            var posts = await _repositoryManager.PostsRepository.GetPosts(parameters);
            return Ok(_mapper.Map<IEnumerable<PostDto>>(posts));
        }

        // GET api/<PostsController>/5
        [Authorize]
        [HttpGet("{id}", Name = "GetPostByID")]
        public async Task<IActionResult> GetPostByID([FromRoute] string id)
        {
            var post = await _repositoryManager.PostsRepository.GetPostById(id);
            if (post is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PostDto>(post));
        }

        // POST api/<PostsController>
        [Authorize]
        [HttpPost("")]
        public async Task<IActionResult> CreatePost([FromBody] PostCreationDto postCreationDto)
        {
            if (postCreationDto is null)
            {
                return BadRequest("postCreationDto is null");
            }

            var post = _mapper.Map<Post>(postCreationDto);
            await _repositoryManager.PostsRepository.CreatePost(post);
            return CreatedAtRoute("GetPostByID", new { id = post.Id }, _mapper.Map<PostDto>(post));
        }

        // PUT api/<PostsController>/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost([FromBody] PostUpdateDto postUpdateDto)
        {
            if (postUpdateDto is null)
            {
                return BadRequest("postCreationDto is null");
            }

            var post = _mapper.Map<Post>(postUpdateDto);
            await _repositoryManager.PostsRepository.UpdatePost(post);
            return NoContent();
        }

        // DELETE api/<PostsController>/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost([FromRoute] string id)
        {
            await _repositoryManager.PostsRepository.DeletePost(id);
            return NoContent();
        }

        // PATCH api/<PostsController>/5
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchPost([FromRoute] string id, [FromBody] JsonPatchDocument<Post> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest("Patch doc was null");
            }

            var post = await _repositoryManager.PostsRepository.GetPostById(id);
            patchDoc.ApplyTo(post);

            await _repositoryManager.PostsRepository.UpdatePost(post);

            return new ObjectResult(post);
        }
    }
}