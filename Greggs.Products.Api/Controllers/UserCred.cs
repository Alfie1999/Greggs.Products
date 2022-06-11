using System.ComponentModel.DataAnnotations;

namespace Greggs.Products.Api.Controllers;
/// <summary>
/// Used to Authenticate a Greggs Entrepreneur 
/// </summary>
public class UserCred
{
  [Required]
  public string Username { get; set; }
  [Required]
  public string Password { get; set; }
}

