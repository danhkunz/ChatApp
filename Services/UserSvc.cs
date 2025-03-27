using ChatApp.DBContext;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
    public interface IUserService
    {
        Task<User> GetUser(Guid UserId);
        Task<User> GetUserByName (string UserName);
        Task<List<User>> GetListUser();
        Task<bool> AddUser(User user);
    }

    public class UserService : IUserService
    {
        private readonly ChatContext _chatContext = new ChatContext("ChatApp.db");

        public async Task<List<User>> GetListUser()
        {
            return await _chatContext.Users.ToListAsync();
        }

        public async Task<User> GetUser(Guid UserId)
        {
            return await _chatContext.Users.Where(x => x.Id == UserId).FirstOrDefaultAsync();
        }

        public async Task<bool> AddUser(User user)
        {
            try{

                _chatContext.Users.Add(user);

                await _chatContext.SaveChangesAsync();

                return true;

            }catch{
                return false;
            }
        }

        public async Task<User> GetUserByName(string UserName)
        {
           return await _chatContext.Users.Where(x => x.UserName == UserName).FirstOrDefaultAsync();
        }
    }
}