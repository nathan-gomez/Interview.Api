using System.ComponentModel.DataAnnotations;

namespace Domain.Contracts.Requests;

public class LoginRequest
{
    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }
}