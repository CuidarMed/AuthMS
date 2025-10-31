using Application.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Application.UseCase
{
    public class DirectoryService : IDirectoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DirectoryService> _logger;

        public DirectoryService(
            IHttpClientFactory httpClientFactory,
            ILogger<DirectoryService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("DirectoryMS");
            _logger = logger;
        }

        public async Task<bool> CreatePatientAsync(int userId, string firstName, string lastName, string dni)
        {
            try
            {
                var createPatientRequest = new
                {
                    UserId = userId,
                    FirstName = firstName,
                    LastName = lastName,
                    Dni = int.TryParse(dni, out var dniValue) ? dniValue : 0,
                    Adress = string.Empty,
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-25)), // Fecha por defecto (25 años atrás), se puede actualizar después
                    HealthPlan = "Pendiente",
                    MembershipNumber = $"TEMP-{userId}"
                };

                var response = await _httpClient.PostAsJsonAsync("/api/v1/Patient", createPatientRequest);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Paciente creado exitosamente en DirectoryMS para UserId: {UserId}", userId);
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Error al crear paciente en DirectoryMS. StatusCode: {StatusCode}, Response: {Response}", 
                        response.StatusCode, errorContent);
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al intentar crear paciente en DirectoryMS para UserId: {UserId}", userId);
                return false;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout al intentar crear paciente en DirectoryMS para UserId: {UserId}", userId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear paciente en DirectoryMS para UserId: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> CreateDoctorAsync(int userId, string firstName, string lastName)
        {
            try
            {
                var createDoctorRequest = new
                {
                    UserId = userId,
                    FirstName = firstName,
                    LastName = lastName,
                    LicenseNumber = "PENDING", // Valor por defecto, se puede actualizar después
                    Biography = (string)null // Se puede actualizar después
                };

                var response = await _httpClient.PostAsJsonAsync("/api/v1/Doctor", createDoctorRequest);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Doctor creado exitosamente en DirectoryMS para UserId: {UserId}", userId);
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Error al crear doctor en DirectoryMS. StatusCode: {StatusCode}, Response: {Response}", 
                        response.StatusCode, errorContent);
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al intentar crear doctor en DirectoryMS para UserId: {UserId}", userId);
                return false;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout al intentar crear doctor en DirectoryMS para UserId: {UserId}", userId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear doctor en DirectoryMS para UserId: {UserId}", userId);
                return false;
            }
        }
    }
}

