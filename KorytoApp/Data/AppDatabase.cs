using SQLite;

namespace KorytoApp.Data
{
    public static class AppDatabase
    {
        private static SQLiteAsyncConnection? _database;

        public static SQLiteAsyncConnection GetConnection()
        {
            if (_database == null)
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "meals.db");
                _database = new SQLiteAsyncConnection(dbPath);
            }

            return _database;
        }
    }
}
