﻿using System.Security.Cryptography;
using System.Text;
using API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API;

public class AccountController : BaseApiController
{
    private readonly DataContext _context;

    public AccountController(DataContext context){

        _context = context;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<AppUser>> Register(RegisterDTO registerDTO){

      if(await UserExists(registerDTO.Username)){

          return BadRequest("Username Already Taken");
      }

       using var hmac = new HMACSHA512();

       var user = new AppUser 
       {
         UserName = registerDTO.Username.ToLower(),
         PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
         PasswordSalt = hmac.Key
       };

       _context.Users.Add(user);
       await _context.SaveChangesAsync();
       return user;

    }

    [HttpPost("login")]
    public async Task<ActionResult<AppUser>> Login(LoginDTO loginDTO){
            
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.Username);

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

            return user;
    }

    private async Task<bool> UserExists(String Username){
      return await _context.Users.AnyAsync(x => x.UserName == Username.ToLower());
    }

}