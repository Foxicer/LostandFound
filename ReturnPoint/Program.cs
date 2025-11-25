using System;
using System.Windows.Forms;

namespace ReturnPoint
{
    static class Program
    {
        public static Models.User? CurrentUser; // set at registration/login

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            // ensure relative paths (e.g. "users.json") resolve to the app's build output folder
            // typically: ...\ReturnPoint\bin\Debug\net6.0-windows
            try
            {
                Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
            catch
            {
                // if setting the current directory fails, continue — relative paths may still work
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // show login dialog — only continue when user successfully logs in
            using (var login = new FormLogin())
            {
                var dr = login.ShowDialog();
                if (dr != DialogResult.OK || login.AuthenticatedUser == null)
                {
                    // user closed the dialog or pressed Cancel — exit application
                    return;
                }

                var auth = login.AuthenticatedUser;
                if (string.Equals(auth.Role ?? "user", "admin", StringComparison.OrdinalIgnoreCase))
                {
                    Application.Run(new FormGalleryAdmin());
                }
                else
                {
                    // UI design synchronized with index.html (visual parity only)
                    Application.Run(new FormGallery());
                }
            }
        }
    }
}
