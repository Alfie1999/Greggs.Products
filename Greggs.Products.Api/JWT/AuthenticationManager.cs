using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Greggs.Products.Api.JWT;
/// <summary>
/// Managers JWT tokens
/// </summary>
public class JwtAuthenticationManager : IJwtAuthenticationManager
{

  private readonly string _key;
  /// <summary>
  /// DISCLAIMER: users - This is only here to help enable the purpose of this exercise, this doesn't reflect the way we work!
  /// logging can also be improved
  /// </summary>
  private readonly IDictionary<string, string> _users = new Dictionary<string, string>
  {
    {"test1", "password" }, {"test2", "password2" }
  };
  public JwtAuthenticationManager(string key)
  {
    _key = key;
  }
  /// <summary>
  /// Authenticate user and return a token on success
  /// </summary>
  /// <param name="username"></param>
  /// <param name="password"></param>
  /// <returns></returns>
  public string Authenticate(string username, string password)
  {
    if (!_users.Any(u => u.Key == username && u.Value == password))
    {
      Log.Warning("username and or password not found");
      return null;
    }
    if (string.IsNullOrWhiteSpace(_key))
    {
      Log.Error("JWT token key not found");
      return null;
    }
    try
    {
      var tokenhandler = new JwtSecurityTokenHandler();
      var tokenKey = Encoding.ASCII.GetBytes(_key);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[]
         {
         new Claim(ClaimTypes.Name, username)
         }),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials =
           new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)

      };
      var token = tokenhandler.CreateToken(tokenDescriptor);
      return tokenhandler.WriteToken(token);
    }
    catch (ArgumentOutOfRangeException aEx)
    {
      Log.Error("JWT token ArgumentOutOfRangeException {aEx}", aEx.ToString());
      throw;
    }
    catch (Exception ex)
    {
      Log.Error("JWT token Exception {ex}", ex.ToString());
      throw;
    }

  }
}
