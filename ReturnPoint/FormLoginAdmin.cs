using System;
using System.Windows.Forms;
using ReturnPoint.Models;

namespace ReturnPoint
{
    public class FormLoginAdmin : Form
    {
        private FormLogin _loginForm;

        public User? AuthenticatedUser { get; private set; }

        public FormLoginAdmin()
        {
            _loginForm = new FormLogin(isAdminLogin: true);
        }

        public new DialogResult ShowDialog()
        {
            _loginForm.StartPosition = FormStartPosition.CenterParent;
            var result = _loginForm.ShowDialog();
            AuthenticatedUser = _loginForm.AuthenticatedUser;
            return result;
        }
    }
}
