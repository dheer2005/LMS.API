using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace LMS.API.Helpers  
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            //var userId = connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //// If null, try the exact claim name from your token
            //if (string.IsNullOrEmpty(userId))
            //{
            //    userId = connection.User?.FindFirst("nameid")?.Value;
            //}

            //if (string.IsNullOrEmpty(userId) && connection.GetHttpContext()?.Request.Query.ContainsKey("userId") == true)
            //{
            //    return connection.GetHttpContext()?.Request.Query["userId"];
            //}

            //return userId;

            var userId = connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }
    }
}
