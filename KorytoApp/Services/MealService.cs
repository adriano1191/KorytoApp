using KorytoApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KorytoApp.Services
{
    public class MealService
    {
        private readonly SQLiteAsyncConnection _db;

        public MealService(SQLiteAsyncConnection db)
        {
            _db = db;
            _db.CreateTableAsync<Meal>().Wait();
        }

        public Task<List<Meal>> GetMealsForDate(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);
            return _db.Table<Meal>()
                      .Where(m => m.Time >= start && m.Time < end)
                      .ToListAsync();
        }

        public Task AddMeal(Meal meal) => _db.InsertAsync(meal);
        public Task DeleteMeal(Meal meal) => _db.DeleteAsync(meal);
    }
}
