using SQLite;
using System;

namespace KorytoApp.Models
{
    public class Meal
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string? Name { get; set; }

        public int Calories { get; set; }

        public DateTime Time { get; set; }
    }
}