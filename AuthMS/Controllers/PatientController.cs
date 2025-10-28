using Application.Attributes;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Exceptions;
using Application.Interfaces.IServices;
using Application.Interfaces.IServices.IAuthServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [RequirePatient]
    public class PatientController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserGetServices _userGetService;

        public PatientController(IAuthorizationService authorizationService, IUserGetServices userGetService)
        {
            _authorizationService = authorizationService;
            _userGetService = userGetService;
        }

        /// <summary>
        /// Obtiene el perfil del paciente autenticado
        /// </summary>
        /// <response code="200">Success</response>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 401)]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = _authorizationService.GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new ApiError { Message = "Usuario no autenticado" });
                }

                var result = await _userGetService.GetUserById(userId.Value);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiError { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError { Message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza el perfil del paciente autenticado
        /// </summary>
        /// <param name="request">Datos actualizados del paciente</param>
        /// <response code="200">Success</response>
        [HttpPut("profile")]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        [ProducesResponseType(typeof(ApiError), 401)]
        public async Task<IActionResult> UpdateMyProfile(UserUpdateRequest request)
        {
            try
            {
                var userId = _authorizationService.GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new ApiError { Message = "Usuario no autenticado" });
                }

                // Los pacientes solo pueden actualizar ciertos campos
                // (implementar lógica específica según necesidades)
                var result = await _userGetService.GetUserById(userId.Value);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError { Message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene las citas del paciente autenticado
        /// </summary>
        /// <response code="200">Success</response>
        [HttpGet("appointments")]
        [ProducesResponseType(typeof(GenericResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 401)]
        public async Task<IActionResult> GetMyAppointments()
        {
            try
            {
                var userId = _authorizationService.GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new ApiError { Message = "Usuario no autenticado" });
                }

                // Implementar lógica para obtener citas del paciente
                return Ok(new GenericResponse { Message = "Lista de citas del paciente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError { Message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el historial médico del paciente autenticado
        /// </summary>
        /// <response code="200">Success</response>
        [HttpGet("medical-history")]
        [ProducesResponseType(typeof(GenericResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 401)]
        public async Task<IActionResult> GetMyMedicalHistory()
        {
            try
            {
                var userId = _authorizationService.GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new ApiError { Message = "Usuario no autenticado" });
                }

                // Implementar lógica para obtener historial médico
                return Ok(new GenericResponse { Message = "Historial médico del paciente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError { Message = ex.Message });
            }
        }
    }
}
