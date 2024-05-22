using System.Security.Cryptography;
using System.Text;
using API;
using API.Data;
using API.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly DataContext _context;
    
    private readonly ITokenService _tokenService;

    private readonly IMapper _mapper;

    public AccountController(DataContext context, ITokenService tokenService, IMapper mapper){

        _context = context;
        _tokenService = tokenService;
        _mapper = mapper;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO){

      if(await UserExists(registerDTO.Username)){

          return BadRequest("Username Already Taken");
      }

      var user = _mapper.Map<AppUser>(registerDTO);

       using var hmac = new HMACSHA512();

       
      user.UserName = registerDTO.Username.ToLower();
      user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
      user.PasswordSalt = hmac.Key;
       

       _context.Users.Add(user);
       await _context.SaveChangesAsync();
       return new UserDTO{
         Username = user.UserName,
         Token = _tokenService.CreateToken(user),
         KnownAs = user.KnownAs
       };

    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO){
            
            var user = await _context.Users
                             .Include(p => p.Photos)  
                             .SingleOrDefaultAsync(x => x.UserName == loginDTO.Username);

            if(user == null){
              return Unauthorized("Invalid Username");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for(int i =0 ; i < computedHash.Length; i++){
              if(computedHash[i] != user.PasswordHash[i]){
                return Unauthorized("Invalid Password");
              }
            }

            return new UserDTO{
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs
            };
    }

    private async Task<bool> UserExists(String Username){
      return await _context.Users.AnyAsync(x => x.UserName == Username.ToLower());
    }

}
