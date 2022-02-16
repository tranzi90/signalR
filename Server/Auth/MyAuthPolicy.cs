using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Auth
{
    public class MyAuthPolicy : AuthorizationHandler<MyAuthPolicy, HubInvocationContext>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MyAuthPolicy requirement, HubInvocationContext resource)
        {
            if (context.User.Identity.Name.Any(symbol => Char.IsDigit(symbol)))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
