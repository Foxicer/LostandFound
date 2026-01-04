using System;
using System.Linq;
using System.Windows.Forms;
using ReturnPoint.Models;

namespace ReturnPoint
{
    static class Program
    {
        public static Models.User? CurrentUser;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            try
            {
                Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
            catch
            {
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var login = new FormLogin())
            {
                var dr = login.ShowDialog();
                if (dr != DialogResult.OK || login.AuthenticatedUser == null)
                {
                    return;
                }

                var auth = login.AuthenticatedUser;
                if (string.IsNullOrWhiteSpace(auth.FirstName) && !string.IsNullOrWhiteSpace(auth.Name))
                {
                    var parts = auth.Name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 1)
                        auth.FirstName = parts[0];
                    if (parts.Length >= 2)
                        auth.MiddleName = parts[1];
                    if (parts.Length >= 3)
                        auth.LastName = string.Join(" ", parts.Skip(2));
                }
                
                CurrentUser = auth;
                
                if (string.Equals(auth.Role ?? "user", "admin", StringComparison.OrdinalIgnoreCase))
                {
                    Application.Run(new FormGalleryAdmin());
                }
                else
                {
                    Application.Run(new FormGallery());
                }
            }
        }
    }
}
