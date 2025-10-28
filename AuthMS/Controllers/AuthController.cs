using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Exceptions;
using Application.Interfaces.IServices.IAuthServices;
using Application.UseCase.AuthServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IPasswordResetService _passwordResetService;
        private readonly ILoginService _loginService;
        private readonly ILogoutService _logoutService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IEmailVerificationService _emailVerificationService;

        public AuthController(
            IPasswordResetService passwordResetService, 
            ILoginService loginService, 
            ILogoutService logoutService, 
            IRefreshTokenService refreshTokenService, 
            IEmailVerificationService emailVerificationService
        )
        {
            _passwordResetService = passwordResetService;
            _loginService = loginService;
            _logoutService = logoutService;
            _refreshTokenService = refreshTokenService;
            _emailVerificationService = emailVerificationService;
        }



        /// <summary>
        /// Login the user with email and password.
        /// </summary>
        /// <param name="request"> The request object containing the email and password.</param>
        /// <response code="200">Success</response>
        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(typeof(GenericResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var result = await _loginService.Login(request);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError { Message = ex.Message });
            }
        }


        /// <summary>
        /// Logout the user by invalidating the refresh token.
        /// </summary>
        /// <param name="request"> The request object containing the refresh token.</param>
        /// <response code="200">Success</response>
        [Authorize]
        [HttpPost("Logout")]
        [ProducesResponseType(typeof(GenericResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public async Task<IActionResult> Logout(LogoutRequest request)
        {
            try
            {
                var result = await _logoutService.Logout(request);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError { Message = ex.Message });
            }
        }



        /// <summary>
        /// Refresh the access token using the refresh token.
        /// </summary>
        /// <param name="request"> The request object containing the refresh token.</param>
        /// <response code="200">Success</response>
        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        [ProducesResponseType(typeof(GenericResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            try
            {
                var result = await _refreshTokenService.RefreshAccessToken(request);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError { Message = ex.Message });
            }
        }


        /// <summary>
        /// Change the password of the authenticated user.
        /// </summary> 
        /// <param name="request">The request object containing the current and new password.</param>
        /// <response code="200">Success</response> 
        [Authorize]
        [HttpPost("ChangePassword")]
        [ProducesResponseType(typeof(GenericResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public async Task<IActionResult> ChangePassword(PasswordChangeRequest request)
        {
            try
            {
                var result = await _passwordResetService.ChangePassword(request);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError { Message = ex.Message });
            }
        }


        /// <summary>
        /// Sends a password reset code to the specified email.
        /// </summary> 
        /// <param name="request"> The request object containing the email.</param>
        /// <response code="200">Success</response>
        [AllowAnonymous]
        [HttpPost("PasswordResetRequest")]
        [ProducesResponseType(typeof(GenericResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public async Task<IActionResult> PasswordResetRequest(PasswordResetRequest request)
        {
            try
            {
                var result = await _passwordResetService.GenerateResetCode(request.Email);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError { Message = ex.Message });
            }
        }

        /// <summary>
        /// Confirms the password reset by validating the provided reset code and updating the user's password.
        /// </summary>
        /// <param name="request"> The request object containing the email, reset code, and new password.</param>
        /// <response code="200">Success</response>
        [AllowAnonymous]
        [HttpPost("PasswordResetConfirm")]
        [ProducesResponseType(typeof(GenericResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public async Task<IActionResult> PasswordResetConfirm(PasswordResetConfirmRequest request)
        {
            try
            {
                var result = await _passwordResetService.ValidateResetCode(request);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError { Message = ex.Message });
            }
        }


        /// <summary>
        /// Verifies the account by validating the email verification code.
        /// </summary>
        /// <param name="request">The request object containing the email and verification code.</param>
        /// <response code="200">Success</response>        
        [AllowAnonymous]
        [HttpPost("VerifyEmail")]
        [ProducesResponseType(typeof(GenericResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public async Task<IActionResult> VerifyEmail(EmailVerificationRequest request)
        {
            try
            {
                var result = await _emailVerificationService.ValidateVerificationCode(request);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError { Message = ex.Message });
            }
        }

        /// <summary>
        /// Resends the email verification code.
        /// </summary>
        /// <param name="request">The request object containing the email.</param>
        /// <response code="200">Success</response>
        [AllowAnonymous]
        [HttpPost("ResendVerificationEmail")]
        [ProducesResponseType(typeof(GenericResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public async Task<IActionResult> ResendVerificationEmail(EmailResendVerificationRequest request)
        {
            try
            {
                // Suponiendo que tienes un método en el servicio de verificación para enviar el email
                var result = await _emailVerificationService.SendVerificationEmail(request.Email);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError { Message = ex.Message });
            }
        }

    }
}
