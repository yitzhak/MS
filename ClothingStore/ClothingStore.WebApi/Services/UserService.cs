using ClothingStore.WebApi.Helpers;
using ClothingStore.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<int> Creat(User user);
    }

    public class UserService : IUserService
    {
        private readonly StoreContext db;

        public UserService(StoreContext context)
        {
            db = context;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            User user = await db.Users.SingleOrDefaultAsync(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so return user details without password
            user.Password = null;
            return user;
        }

        public async Task<int> Creat(User user)
        {
            bool isUserExistInDb = await db.Users.AnyAsync(p => p.Email.ToLower() == user.Email.ToLower());
            if (isUserExistInDb)
            {
                throw new BadRequestException(Enum.InternalCode.AlreadyExist, "this email already have an account");
            }
            user.CreatedDate = DateTime.UtcNow;
            user.AccessLevel = Enum.AccessLevel.Salesman;
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            return user.Id;
        }
    }
}
