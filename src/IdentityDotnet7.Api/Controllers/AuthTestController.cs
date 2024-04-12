using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityDotnet7.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityDotnet7.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthTestController : ControllerBase
    {

        [ProfileAuthorize("role", "Foda-se")]
        [HttpGet("test")]
        public async Task<IActionResult> GetTest1()
        {
            return Ok(new
            {
                test = 1,
                status = "Ok"
            });
        }
        
        [ProfileAuthorize("Darth", "Vader")]
        [HttpGet("testtest")]
        public async Task<IActionResult> GetTest2()
        {
            return Ok(new
            {
                test = 2,
                status = "Ok"
            });
        }
    }
}
