using System;
using System.IO;

namespace DAL
{
    public static class DAL_Settings
    {

        private static readonly string ConfigFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SeoulStay");

        private static readonly string ConfigFile = Path.Combine(ConfigFolder, "DatabaseConfig.txt");

        public static string ConnectionString
        {
            get
            {
                if (File.Exists(ConfigFile))
                {
                    return File.ReadAllText(ConfigFile);
                }

                return Properties.Settings.Default
                    .Seoul_StayConnectionString1;
            }
        }

        public static void SaveConnectionString(
            string connectionString)
        {
            if (!Directory.Exists(ConfigFolder))
            {
                Directory.CreateDirectory(ConfigFolder);
            }

            File.WriteAllText(
                ConfigFile,
                connectionString);
        }
    }
}