using Microsoft.AspNetCore.Mvc;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using API.DTO;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController{

    private readonly IUserRepository _userRepository;

    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
       _userRepository = userRepository;
       _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers(){
      
       var users = await _userRepository.GetMembersAsync();

       return Ok(users);
        
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDTO>> GetUser(string username){

        return await _userRepository.GetMemberAsync(username);

        
    }


}
