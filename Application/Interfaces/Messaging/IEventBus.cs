using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Messaging
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T @event, string routingKey);
    }
}
