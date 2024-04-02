using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class User
{
    public string? Email { get; set; }

    public string? Name { get; set; }

    public string? Password { get; set; }

    public Guid UserId { get; set; }
}

public partial class UserDTO
{
    public string? Email { get; set; }

    public string? Name { get; set; }

    public string? Password { get; set; }

}

public partial class LoginDTO
{
    public string? Email { get; set; }

    public string? Password { get; set; }

}
