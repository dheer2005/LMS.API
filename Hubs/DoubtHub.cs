using Microsoft.AspNetCore.SignalR;

namespace LMS.API.Hubs
{
    public class DoubtHub: Hub
    {
        public override Task OnConnectedAsync()
        {
            if (!Context.User.Identity.IsAuthenticated)
            {
                Console.WriteLine("❌ User not authenticated!");
            }
            else
            {
                Console.WriteLine($"✅ Connected user: {Context.User.Identity.Name}");
            }

            return base.OnConnectedAsync();
        }
        public async Task SendDoubtNotification(int teacherId, object doubtData)
        {
            await Clients.User(teacherId.ToString()).SendAsync("ReceiveDoubt", doubtData);
        }

        public async Task SendDoubtReplyNotification(int studentId, object replyData)
        {
            await Clients.User(studentId.ToString()).SendAsync("ReceiveDoubtReply", replyData);
        }
    }
}
