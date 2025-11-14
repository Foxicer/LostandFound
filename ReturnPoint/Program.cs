using System;
using System.Windows.Forms;

namespace ReturnPoint
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
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
                    Application.Run(new FormGallery());
                }
            }
        }
    }
}
