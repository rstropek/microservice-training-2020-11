using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCoreMicroserviceSample.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Auth : ControllerBase
    {
        /// <summary>
        /// Trigger login process
        /// </summary>
        [HttpGet("login", Name = "Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task Login()
        {
            await HttpContext.ChallengeAsync("IdentityServer", new AuthenticationProperties() { RedirectUri = "/" });
        }

        /// <summary>
        /// Trigger logout process
        /// </summary>
        [HttpGet("logout", Name = "Logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("IdentityServer");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Get user profile of currently signed in user
        /// </summary>
        /// <returns>User profile</returns>
        [HttpGet("profile", Name = "GetProfile")]
        [Authorize]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserProfile), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetProfile()
        {
            var identity = HttpContext.User.Identities.First();
            return Ok(new UserProfile(
                identity.Claims.First(c => c.Type == ClaimTypes.Name).Value,
                identity.Claims.First(c => c.Type == ClaimTypes.Email).Value,
                identity.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));
        }
    }
}
