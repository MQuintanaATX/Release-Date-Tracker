using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Release_Date_Tracker.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Release_Date_Tracker.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TokenController : Controller
{
    private readonly Credentials _credentials;

    public TokenController(Credentials credentials)
    {
        this._credentials = credentials;
    }


    [HttpPost]
    public IActionResult GenerateToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_credentials.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(_credentials.Issuer,
            _credentials.Issuer,
          null,
          expires: DateTime.Now.AddMinutes(120),
          signingCredentials: credentials);

        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }
}