using ApplicationAPI.DTOs;
using ApplicationAPI.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationAPI.Controllers
{
    [Route("api/user-accounts")]
    [ApiController]
    public class UserAccountsController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public UserAccountsController(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }
        
        [Authorize]
        [HttpGet("{id}", Name = "GetUserAccountById")]
        public async Task<IActionResult> GetUserAccountById([FromRoute] string id)
        {
            if (id is null)
            {
                return BadRequest("id is null");
            }

            var userAccount = await _repositoryManager.UserAccountsRepository.GetUserAccountById(id);

            return Ok(_mapper.Map<UserAccountDto>(userAccount));
        }
    }
}
