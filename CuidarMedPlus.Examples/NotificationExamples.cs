using Application.Dtos.Notification;
using Application.UseCase.NotificationServices;
using System;

namespace CuidarMedPlus.Examples
{
    /// <summary>
    /// Ejemplos de cómo usar el sistema de notificaciones de CuidarMed+
    /// </summary>
    public class NotificationExamples
    {
        private readonly CuidarMedNotificationService _notificationService;

        public NotificationExamples(CuidarMedNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Ejemplo: Crear notificación cuando un paciente agenda un turno
        /// </summary>
        public async Task ExampleAppointmentCreated()
        {
            var appointmentData = new AppointmentPayload
            {
                AppointmentId = Guid.NewGuid(),
                PatientName = "Juan Pérez",
                DoctorName = "Dr. María González",
                Specialty = "Cardiología",
                AppointmentDate = DateTime.Now.AddDays(3),
                AppointmentTime = new TimeSpan(14, 30, 0), // 2:30 PM
                AppointmentType = "Virtual",
                MeetingLink = "https://meet.cuidarmed.com/consulta-12345",
                Notes = "Por favor, tenga a mano sus estudios de sangre recientes.",
                Status = "Confirmado"
            };

            // Crear notificación para el paciente (userId = 1)
            await _notificationService.CreateAppointmentCreatedNotification(1, appointmentData);
        }

        /// <summary>
        /// Ejemplo: Crear notificación de recordatorio 24 horas antes
        /// </summary>
        public async Task ExampleAppointmentReminder()
        {
            var appointmentData = new AppointmentPayload
            {
                AppointmentId = Guid.NewGuid(),
                PatientName = "Juan Pérez",
                DoctorName = "Dr. María González",
                Specialty = "Cardiología",
                AppointmentDate = DateTime.Now.AddDays(1),
                AppointmentTime = new TimeSpan(14, 30, 0),
                AppointmentType = "Virtual",
                MeetingLink = "https://meet.cuidarmed.com/consulta-12345",
                Notes = "Recuerde probar su conexión a internet antes de la consulta.",
                Status = "Confirmado"
            };

            await _notificationService.CreateAppointmentReminderNotification(1, appointmentData);
        }

        /// <summary>
        /// Ejemplo: Crear notificación cuando una receta está lista
        /// </summary>
        public async Task ExamplePrescriptionReady()
        {
            var prescriptionData = new PrescriptionPayload
            {
                PrescriptionId = Guid.NewGuid(),
                PatientName = "Juan Pérez",
                DoctorName = "Dr. María González",
                Specialty = "Cardiología",
                PrescriptionDate = DateTime.Now,
                PrescriptionNumber = "REC-2024-001234",
                DownloadUrl = "https://cuidarmed.com/prescriptions/download/12345",
                Notes = "Tomar con el desayuno. Evitar alcohol durante el tratamiento.",
                IsReady = true
            };

            await _notificationService.CreatePrescriptionReadyNotification(1, prescriptionData);
        }

        /// <summary>
        /// Ejemplo: Crear notificación cuando inicia una consulta virtual
        /// </summary>
        public async Task ExampleConsultationStarted()
        {
            var consultationData = new ConsultationPayload
            {
                ConsultationId = Guid.NewGuid(),
                PatientName = "Juan Pérez",
                DoctorName = "Dr. María González",
                Specialty = "Cardiología",
                ConsultationDate = DateTime.Now,
                ConsultationTime = new TimeSpan(14, 30, 0),
                MeetingLink = "https://meet.cuidarmed.com/consulta-12345",
                Status = "Iniciada",
                Diagnosis = "En evaluación",
                Treatment = "En evaluación",
                Notes = "Consulta de seguimiento cardiológico"
            };

            await _notificationService.CreateConsultationStartedNotification(1, consultationData);
        }

        /// <summary>
        /// Ejemplo: Crear notificación de recordatorio de medicación
        /// </summary>
        public async Task ExampleMedicationReminder()
        {
            var reminderData = new MedicalReminderPayload
            {
                ReminderId = Guid.NewGuid(),
                PatientName = "Juan Pérez",
                ReminderType = "Medicación",
                Title = "Recordatorio de Medicación",
                Description = "Es hora de tomar tu medicación",
                ReminderDate = DateTime.Now,
                ReminderTime = new TimeSpan(8, 0, 0), // 8:00 AM
                MedicationName = "Losartán 50mg",
                Dosage = "1 comprimido",
                Instructions = "Tomar con el desayuno, todos los días",
                IsImportant = true
            };

            await _notificationService.CreateMedicationReminderNotification(1, reminderData);
        }

        /// <summary>
        /// Ejemplo: Crear notificación cuando se cancela un turno
        /// </summary>
        public async Task ExampleAppointmentCancelled()
        {
            var appointmentData = new AppointmentPayload
            {
                AppointmentId = Guid.NewGuid(),
                PatientName = "Juan Pérez",
                DoctorName = "Dr. María González",
                Specialty = "Cardiología",
                AppointmentDate = DateTime.Now.AddDays(2),
                AppointmentTime = new TimeSpan(14, 30, 0),
                AppointmentType = "Virtual",
                MeetingLink = "",
                Notes = "Turno cancelado por emergencia médica del doctor.",
                Status = "Cancelado"
            };

            await _notificationService.CreateAppointmentCancelledNotification(1, appointmentData);
        }
    }
}
