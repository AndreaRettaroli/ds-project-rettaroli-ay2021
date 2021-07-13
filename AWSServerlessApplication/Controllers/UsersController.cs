using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AWSServerlessApplication.Authentication;
using AWSServerlessApplication.AWS;
using AWSServerlessApplication.Dto;
using AWSServerlessApplication.Models;
using AWSServerlessApplication.Services.Interfaces;
using AWSServerlessApplication.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AWSServerlessApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _userService;
        private readonly ICognitoService _cognitoService;
        private readonly IOptions<AppSettings> _options;
        public UsersController(IUsersService userService,
            ICognitoService cognitoService,
            IOptions<AppSettings> options) 
        {
            _userService = userService;
            _cognitoService = cognitoService;
            _options = options;
           // _mapper = mapper;
        }

        ///<summary>
        /// SignIn 
        ///</summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response> 
        /// <response code="401">Unauthorized</response>
        [HttpPost("signIn")]
        public async Task<ActionResult<UserDto>> SignInAsync(Credentials credentials)
        {
            return Ok();
        }

        ///<summary>
        /// Set the user password
        ///</summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response> 
        /// <response code="401">Unauthorized</response>
        [Authorize]
        [HttpPut("setPassword")]
        public async Task<ActionResult<UserDto>> SetPasswordAsync(Credentials credentials)
        {
            return Ok();
        }
        ///<summary>
        /// Get a user 
        ///</summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response> 
        /// <response code="401">Unauthorized</response>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetAsync(string id)
        {
            return Ok();
        }

        ///<summary>
        /// Get the list of users 
        ///</summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response> 
        /// <response code="401">Unauthorized</response>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> ListAsync()
        {
            return Ok();
        }

        ///<summary>
        /// Create a new user
        ///</summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response> 
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateAsync(CreateModifyUserRequest userRequest)
        {
            return Ok();
        }

        ///<summary>
        /// Update a user
        ///</summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response> 
        /// <response code="401">Unauthorized</response>
        [Authorize]
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<UserDto>> Update(string id, [FromBody] CreateModifyUserRequest userRequest)
        {
            return Ok();
        }



        ///<summary>
        /// Initiate forgot password request
        ///</summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response> 
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("forgotPassword/{email}")]
        public async Task<IActionResult> InitForgotPassword(string email)
        {
            return Ok();
        }

        ///<summary>
        /// Change the forgotten password
        ///</summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response> 
        /// <response code="401">Unauthorized</response>
        [HttpPut]
        [Route("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] Models.ForgotPasswordRequest request)
        {
            return Ok();
        }



        ///<summary>
        /// Delete a user
        ///</summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response> 
        /// <response code="401">Unauthorized</response>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return Ok();
        }

    }
}
