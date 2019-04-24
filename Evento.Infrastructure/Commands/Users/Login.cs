using System;
using System.Collections.Generic;
using System.Text;

namespace Evento.Infrastructure.Commands.Users
{
    public class Login
    {
        public string Email { get; protected set; }
        public string Password { get; protected set; }
    }
}
