using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum NotificationType
    {
        ReservationCreated,
        ReservationConfirmed,
        ReservationUpdated,
        ReservationCancelled,        
        VehiclePickedUp,
        VehicleReturned,
        PaymentSucceeded,
        Reminder,
        Overdue,
        ReservationEndingSoon,
        Custom
    }
}
