using KorytoApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KorytoApp.Services
{
   public class UserService
    {
        private readonly SQLiteAsyncConnection _db;

        public UserService(SQLiteAsyncConnection db)
        {
            _db = db;
            _db.CreateTableAsync<User>().Wait();
        }

        public Task<User?> GetUserAsync()
        {
            return _db.Table<User?>().FirstOrDefaultAsync();
        }

        public Task AddUser(User user)
        {
            return _db.InsertAsync(user);
        }

        public Task UpdateUser(User user)
        {
            return _db.UpdateAsync(user);
        }

        public Task DeleteUser(User user)
        {
            return _db.DeleteAsync(user);
        }
        //public Task DeleteUser(User meal) => _db.DeleteAsync(meal);
    }
}
