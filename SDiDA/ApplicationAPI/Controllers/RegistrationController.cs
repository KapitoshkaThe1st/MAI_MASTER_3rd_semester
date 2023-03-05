using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;

using BC = BCrypt.Net.BCrypt;

using ApplicationAPI.DTOs;
using ApplicationAPI.Entities;
using ApplicationAPI.Repository;

namespace ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repositoryManager;

        public RegistrationController(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _mapper = mapper;
            _repositoryManager = repositoryManager;
        }

        [AllowAnonymous]
        [HttpPost("")]
        public async Task<IActionResult> Register([FromBody] UserAccountRegistrationDto userAccountRegistrationDto)
        {

            if (userAccountRegistrationDto is null)
            {
                return BadRequest("userAccountRegistrationDto is null");
            }

            if (await LoginExistsAsync(userAccountRegistrationDto.Login))
            {
                return Conflict("Login is already taken");
            }

            var userProfile = await CreateUserProfile(_mapper.Map<UserProfileCreationDto>(userAccountRegistrationDto));
            var userAccount = await CreateUserAccount(userAccountRegistrationDto, userProfile.Id);

            //_mapper.Map<UserProfileDto>(userAccount)
            return CreatedAtRoute("GetUserAccountById", new { id = userAccount.Id }, _mapper.Map<UserAccountDto>(userAccount));
        }

        private async Task<bool> LoginExistsAsync(string login)
        {
            var user = await _repositoryManager.UserAccountsRepository.GetUserAccountByLogin(login);
            return user != null;
        }

        private async Task<UserAccount> CreateUserAccount(UserAccountRegistrationDto userAccountRegistrationDto, ObjectId userProfileId)
        {
            var userAccount = _mapper.Map<UserAccount>(userAccountRegistrationDto);
            userAccount.ProfileId = userProfileId;
            userAccount.PasswordHash = BC.HashPassword(userAccountRegistrationDto.Password);

            await _repositoryManager.UserAccountsRepository.CreateUserAccount(userAccount);
            return userAccount;
        }

        private async Task<UserProfile> CreateUserProfile(UserProfileCreationDto userProfileCreationDto)
        {
            var userProfile = _mapper.Map<UserProfile>(userProfileCreationDto);
            await _repositoryManager.UserProfilesRepository.CreateUserProfile(userProfile);
            return userProfile;
        }
    }
}
