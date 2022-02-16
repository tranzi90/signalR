using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Hubs
{
    [Authorize]
    public class MessageHub : Hub<IMessageClient>
    {
        [Authorize(Policy = "MyPolicy")]
        public Task SendToOthers(Message message)
        {
            var messageForClient = NewMessage.Create(Context.UserIdentifier, message);
            return Clients.Others.Send(messageForClient);
        }
    }
}
