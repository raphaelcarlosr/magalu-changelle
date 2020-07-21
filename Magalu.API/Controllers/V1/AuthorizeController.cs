using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magalu.API.Models;
using Magalu.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Magalu.API.Controllers.V1
{
    [Route("api/authorize")]
    [ApiController]
    public class Authorize : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        public ActionResult<dynamic> Post([FromForm] User model)
        {
            var user = UserService.Get(model.Username, model.Password);
            if (user == null) return NotFound(new { message = "Usuário ou senha inválidos" });
            var token = JwtService.GenerateToken(user);
            user.Password = "";
            return new
            {
                user,
                token
            };
        }
    }
}
