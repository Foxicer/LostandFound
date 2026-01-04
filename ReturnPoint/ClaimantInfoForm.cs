using System;
using System.Drawing;
using System.Windows.Forms;
namespace ReturnPoint
{
    public class ClaimantInfoForm : Form
    {
        public string ClaimantName => txtName.Text;
        public string Contact => txtContact.Text;
        public string Role => cmbRole.SelectedItem?.ToString();
        public string GradeSection => txtWhere.Text;
        public string WhenFound => txtWhen.Text;
        private TextBox txtName, txtContact, txtWhere, txtWhen;
        private ComboBox cmbRole;
        private Label lblWhere;
        private Button btnSubmit;
        public ClaimantInfoForm()
        {
            txtName = new TextBox();
            txtContact = new TextBox() { Text = "09** *** ****" };
            txtWhere = new TextBox();
            txtWhen = new TextBox();
            cmbRole = new ComboBox();
            btnSubmit = new Button() { Text = "Submit" };
            this.Text = "Claimant Info";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(420, 300);
            Label lblName = new Label { Text = "Name", Top = 12, Left = 12, AutoSize = true };
            txtName.Top = 32; txtName.Left = 12; txtName.Width = 360;
            Label lblContact = new Label { Text = "Contact", Top = 64, Left = 12, AutoSize = true };
            txtContact.Top = 84; txtContact.Left = 12; txtContact.Width = 360;
            btnSubmit.Top = 220; btnSubmit.Left = 12;
            btnSubmit.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtContact.Text) || txtContact.Text == "09** *** ****")
                {
                    MessageBox.Show("Please enter claimant contact number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblContact);
            this.Controls.Add(txtContact);
            this.Controls.Add(btnSubmit);
        }
    }
}

