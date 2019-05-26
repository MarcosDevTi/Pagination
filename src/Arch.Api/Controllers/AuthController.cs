using Arch.Cqrs.Client.Command.User;
using Arch.Cqrs.Client.Query.User;
using Arch.Domain.Core.DomainNotifications;
using Arch.Infra.Shared.Cqrs;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;

namespace Arch.Api.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : BaseController
    {
        private readonly IProcessor _processor;

        public AuthController(IProcessor processor, IDomainNotification notifications) : base(notifications)
        {
            _processor = processor;
        }

        [HttpPost, Route("v1/public/register")]
        public HttpResponseMessage Register(RegisterUser registerUser)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            registerUser.Username = registerUser.Username.ToLower();
            if (_processor.Get(new UserExists(registerUser.Username)))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Username already exists");

            _processor.Send(registerUser);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet, Route("v1/public/login")]
        public HttpResponseMessage Login(LoginUser registerUser)
        {
            var userLogin = _processor.Get(registerUser);
            if (userLogin == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("Super44646464asdasdahjlsdj");
            IdentityModelEventSource.ShowPII = true;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userLogin.Id.ToString()),
                    new Claim(ClaimTypes.Name, userLogin.Username)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature),

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Request.CreateResponse(HttpStatusCode.OK, new { token = tokenHandler.WriteToken(token) });

        }
    }
}
