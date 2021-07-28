using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using AutoMapper;
using AWSServerlessApplication.Authentication;
using AWSServerlessApplication.AWS;
using AWSServerlessApplication.Dto;
using AWSServerlessApplication.Models;
using AWSServerlessApplication.Services.Interfaces;
using AWSServerlessApplication.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
        private readonly IMapper _mapper;
        public UsersController(IUsersService userService,
            ICognitoService cognitoService,
            IOptions<AppSettings> options,
            IMapper mapper)
        {
            _userService = userService;
            _cognitoService = cognitoService;
            _options = options;
            _mapper = mapper;
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
            if (string.IsNullOrEmpty(credentials.Email) ||
                string.IsNullOrEmpty(credentials.Password)) return Unauthorized();

            var cognitoAuth = new CognitoAuthentication(_options);
            var result = await cognitoAuth.SignInAsync(credentials);

            if (!result.Success)
                return Unauthorized();

            var user = await _userService.GetAsync(result.Username);
            user.Token = result.CognitoToken.IdToken;
            user.AccessToken = result.CognitoToken.AccessToken;
            user.RefreshToken = result.CognitoToken.RefreshToken;

            return Ok(user);
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
            if (string.IsNullOrEmpty(credentials.Password)) return Unauthorized();

            var user = await _userService.SetPasswordAsync(credentials);

            if (user == null)
                return BadRequest();
            if (user.Deleted != null)
                return NotFound();
            var response = _mapper.Map<UserDto>(user);
            return Ok(response);
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
            var user = await _userService.GetAsync(id);

            if (user == null || user.Deleted != null)
                return NotFound();

            var response = _mapper.Map<UserDto>(user);
            return Ok(response);
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
            List<UserDto> users = new List<UserDto>();
            var data = await _userService.ListAsync();
            if (data.Count == 0)
                return NotFound();

            data.ForEach(u =>
            {
               var user= _mapper.Map<UserDto>(u);
                users.Add(user);
            });
            return users;
        }

        ///<summary>
        /// Create a new user
        ///</summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response> 
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateAsync(CreateUserRequest userRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(userRequest.Email) ||
                   string.IsNullOrEmpty(userRequest.Name) ||
                   string.IsNullOrEmpty(userRequest.Surname)) return BadRequest();

                var data = await _userService.CreateAsync(_mapper.Map<DynamoDBUser>(userRequest));

                if (data == null)
                    return BadRequest();

                var response = _mapper.Map<UserDto>(data);
                return Ok(response);
            }
            catch (UsernameExistsException)
            {
                return Conflict();
            }

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
        public async Task<ActionResult<UserDto>> Update(string id, [FromBody] ModifyUserRequest userRequest)
        {
            userRequest.Id = id;
            var data = await _userService.UpdateAsync(_mapper.Map<DynamoDBUser>(userRequest));

            if (data == null)
                return BadRequest();

            var response = _mapper.Map<UserDto>(data);
            return Ok(response);
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
            try
            {
                await _cognitoService.InitForgotPasswordAsync(email);
                return Ok();
            }
            catch (UserNotConfirmedException)
            {
                return NotFound(new { Message = $"User {email} not confirmed" });
            }
            catch (UserNotFoundException)
            {
                return NotFound(new { Message = $"User {email} not found" });
            }
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
            await _cognitoService.ChangeForgottenPassword(request);
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
            var user = await _userService.GetAsync(id);
            if (user != null)
            {
                await _cognitoService.AdminDisableUserAsync(user.Email);
                var result = await _userService.DeleteAsync(id);

                if (!result)
                    return BadRequest();

                return Ok();
            }
            else
                return BadRequest();
        }

    }
}
