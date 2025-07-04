﻿using SQLite;
namespace KorytoApp.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string UserName { get; set; }
        public string UserGender { get; set; } // lub enum
        public int UserAge { get; set; } // lata
        public int UserHeight { get; set; } // cm
        public int UserWeight { get; set; } // kg

        public double UserActivity { get; set; } // lub enum

    }
}
