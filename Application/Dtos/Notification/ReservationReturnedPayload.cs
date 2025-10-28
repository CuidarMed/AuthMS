using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Notification
{
    public class ReservationReturnedPayload
    {
        public Guid ReservationId { get; set; }
        public string PickupBranchName { get; set; }
        public string DropOffBranchName { get; set; }
        public DateTime ActualPickupTime { get; set; }
        public DateTime ActualReturnTime { get; set; }

    }
}
