using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AutoMapper;
using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IAppUser appUserRepository, IMapper mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<GetUserDTO>>> GetAllUsers()
        {
            var users = await appUserRepository.GetAllUsers();
            var mappedUsers = mapper.Map<List<GetUserDTO>>(users);
            return mappedUsers;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetUserDTO>> GetUser([FromRoute] int id)
        {
            var user = await appUserRepository.GetUser(id);
            var mappedUser = mapper.Map<GetUserDTO>(user);
            return mappedUser.Id > 0 ? Ok(mappedUser) : NotFound("User not found") ;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Response>> Register([FromBody] AppUserDTO appUserDto)
        {
            if (!ModelState.IsValid) return BadRequest("Wrong input");
            var response = await appUserRepository.Register(appUserDto);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response>> LoginUser([FromBody] LoginDTO loginUserDTO)
        {
            if (!ModelState.IsValid) return BadRequest("Wrong input");
            var response = await appUserRepository.Login(loginUserDTO);
            return response.Flag ? Ok(response) : BadRequest(response);
        }
    }
}
