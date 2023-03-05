using ApplicationAPI.DTOs;
using ApplicationAPI.Entities;
using ApplicationAPI.Repository;
using ApplicationAPI.Requests;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authorization;

namespace ApplicationAPI.Controllers
{
    [Route("api/user-profiles")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public UserProfilesController(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        // GET: api/user-profiles
        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> GetUsers([FromQuery] UsersRequestParameters parameters)
        {
            var users = await _repositoryManager.UserProfilesRepository.GetUserProfiles(parameters);
            return Ok(_mapper.Map<IEnumerable<UserProfileDto>>(users));
        }

        // GET: api/user-profiles/5
        [Authorize]
        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            var user = await _repositoryManager.UserProfilesRepository.GetUserProfileById(id);
            if (user is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserProfileDto>(user));
        }

        // PUT api/user-profiles/5
        [Authorize]
        [HttpPut("")]
        public async Task<IActionResult> UpdateUser([FromBody] UserProfileUpdateDto userUpdateDto)
        {
            if (userUpdateDto is null)
            {
                return BadRequest("UserUpdateDto is null");
            }

            var user = _mapper.Map<UserProfile>(userUpdateDto);
            await _repositoryManager.UserProfilesRepository.UpdateUserProfile(user);
            return NoContent();
        }

        // DELETE api/user-profiles/5
        [Authorize]
        [HttpDelete("{id}", Name = "DeleteUserById")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            await _repositoryManager.UserProfilesRepository.DeleteUserProfile(id);
            return NoContent();
        }

        // PATCH api/user-profiles/5
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchUser([FromRoute] string id, [FromBody] JsonPatchDocument<UserProfile> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest("Patch doc was null");
            }

            var userProfile = await _repositoryManager.UserProfilesRepository.GetUserProfileById(id);
            patchDoc.ApplyTo(userProfile);

            await _repositoryManager.UserProfilesRepository.UpdateUserProfile(userProfile);

            return new ObjectResult(userProfile);
        }

        // GET api/user-profiles/subscriptions
        [Authorize]
        [HttpGet("{id}/subscriptions")]
        public async Task<IActionResult> GetSubscriptions([FromRoute] string id)
        {
            if (id is null)
            {
                return BadRequest("subscriptionDto is null");
            }

            var subscriptions = await _repositoryManager.UserProfilesRepository.GetSubscriptions(id);
            return Ok(subscriptions);
        }

        // POST api/user-profiles
        [Authorize]
        [HttpPost("{id}/subscriptions")]
        public async Task<IActionResult> AddSubscription([FromRoute] string id, [FromBody] SubscriptionDto subscriptionDto)
        {
            if (subscriptionDto is null)
            {
                return BadRequest("subscriptionDto is null");
            }

            if (id == subscriptionDto.SubscriptionTargetId)
            {
                return Conflict("Пользователь не может подписаться сам на себя");
            }

            var subscriptionObjectId = new ObjectId(subscriptionDto.SubscriptionTargetId);
            var userProfile = await _repositoryManager.UserProfilesRepository.AddSubscription(id, subscriptionDto.SubscriptionTargetId);
            return Created(
                "GetSubscription",
                new
                {
                    subscriptionTargetId = userProfile.SubscriptionsIds.First(x => x == subscriptionObjectId)
                });
        }

        // POST api/user-profiles/5/feed
        [Authorize]
        [HttpGet("{id}/feed")]
        public async Task<IActionResult> GetFeedForUserById([FromRoute] string id, [FromQuery] PostsRequestParameters parameters)
        {
            if (id is null)
            {
                return BadRequest("id was null");
            }

            var userProfile = await _repositoryManager.UserProfilesRepository.GetUserProfileById(id);
            var posts = await _repositoryManager.PostsRepository.GetLatestPostsAsync(userProfile.SubscriptionsIds, parameters);

            return Ok(_mapper.Map<IEnumerable<PostDto>>(posts));
        }
    }
}
