using Microsoft.AspNetCore.SignalR;
namespace ChatApp.ChatHubs
{
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            Console.WriteLine($"{userId} connected.");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            Console.WriteLine($"{userId} disconnected.");
            return base.OnDisconnectedAsync(exception);
        }
    }

}