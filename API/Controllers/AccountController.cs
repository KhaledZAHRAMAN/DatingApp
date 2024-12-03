using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(Datacontext context, ITokenService tokenService): BaseApiController
{
[HttpPost("register")] //account/register
public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
{
    if (await UserExists(registerDto.UserName)) return BadRequest("UserName is taken");

    using var hmac = new HMACSHA512();

    var user = new AppUser
    {
        UserName = registerDto.UserName.ToLower(),
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
        passwordsalt = hmac.Key
    };
    context.Users.Add(user);
    await context.SaveChangesAsync();

    return new UserDto
    {
        UserName = user.UserName,
        token = TokenService.CreateToken(user)
    };
}

[HttpPost("login")]
public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
{
    var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

    if (user == null) return Unauthorized("Invalid username");

    using var hmac = new HMACSHA512(user.passwordsalt);

    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

    for (int i = 0; i < computedHash.Length; i++)
    {
        if (computedHash[i] != user.passwordHash[i]) return Unauthorized("Invalid password");
    }

    return new UserDto
    {
        UserName = user.UserName,
        token = tokenService.createToken(user)
    };
}

private async Task<bool> UserExists(string username)
{
    return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower()); //Bob != bob
}
}