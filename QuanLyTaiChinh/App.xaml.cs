using System;
using System.IO;
using System.Windows;
using dotenv.net;

namespace QuanLyTaiChinh
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            string? dir = AppContext.BaseDirectory;
            string? envPath = null;

            while (dir != null)
            {
                string candidate = Path.Combine(dir, ".env");

                if (File.Exists(candidate))
                {
                    envPath = candidate;
                    break;
                }

                dir = Directory.GetParent(dir)?.FullName;
            }

            if (envPath == null)
            {
                MessageBox.Show("Không tìm thấy file .env");
            }
            else
            {
                DotEnv.Load(new DotEnvOptions(
                    envFilePaths: new[] { envPath }
                ));
            }

            base.OnStartup(e);
        }
    }
}