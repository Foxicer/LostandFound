using System;
using System.Windows.Forms;
using ReturnPoint.Models;

namespace ReturnPoint
{
    public class FormLoginAdmin : Form
    {
        private FormLogin _loginForm;
        private Label _errorLabel;

        public User? AuthenticatedUser { get; private set; }

        public FormLoginAdmin()
        {
            _loginForm = new FormLogin(isAdminLogin: true);
        }

        public new DialogResult ShowDialog()
        {
            _loginForm.StartPosition = FormStartPosition.CenterParent;
            var result = _loginForm.ShowDialog();
            
            // Validate that the authenticated user has an admin role
            if (result == DialogResult.OK && _loginForm.AuthenticatedUser != null)
            {
                string userRole = _loginForm.AuthenticatedUser.Role ?? "user";
                
                // Check if user has admin or headadmin role
                if (string.Equals(userRole, "admin", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(userRole, "headadmin", StringComparison.OrdinalIgnoreCase))
                {
                    AuthenticatedUser = _loginForm.AuthenticatedUser;
                }
                else
                {
                    // User is not an admin, show error and return Cancel
                    MessageBox.Show(
                        "Access denied. Only administrators can log in here.",
                        "Admin Access Required",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return DialogResult.Cancel;
                }
            }
            
            return result;
        }
    }
}
