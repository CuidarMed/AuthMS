using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum NotificationType
    {
        // Turnos médicos
        AppointmentCreated,
        AppointmentConfirmed,
        AppointmentCancelled,
        AppointmentRescheduled,
        AppointmentReminder,
        AppointmentStartingSoon,
        
        // Consultas médicas
        ConsultationStarted,
        ConsultationEnded,
        ConsultationCancelled,
        
        // Recetas y documentos
        PrescriptionReady,
        MedicalOrderReady,
        DocumentGenerated,
        
        // Recordatorios médicos
        MedicationReminder,
        FollowUpReminder,
        TestResultsReady,
        
        // Sistema general
        AccountActivated,
        PasswordReset,
        EmailVerification,
        Custom
    }
}
