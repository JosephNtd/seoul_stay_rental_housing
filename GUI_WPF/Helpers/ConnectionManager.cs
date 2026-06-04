using DAL;

namespace GUI_WPF.Helpers
{
    public static class ConnectionManager
    {
        public static string CurrentConnectionString
        {
            get
            {
                return DAL_Settings.ConnectionString;
            }
        }

        public static void SaveConnectionString(string connectionString)
        {
            DAL_Settings.SaveConnectionString(connectionString);
        }
    }
}