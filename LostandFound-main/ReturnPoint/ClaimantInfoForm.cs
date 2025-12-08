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
            this.Text = "Claimant Info";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(400, 400);

            // Name
            Label lblName = new Label { Text = "Name:", Top = 20, Left = 20, Width = 120 };
            txtName = new TextBox { Top = 20, Left = 150, Width = 200, Text = "N/A" };
            txtName.GotFocus += (s, e) =>
            {
                if (txtName.Text == "N/A")
                {
                    txtName.Text = "";
                    txtName.ForeColor = Color.Black;
                }
            };
            txtName.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    txtName.Text = "N/A";
                    txtName.ForeColor = Color.Gray;
                }
            };

            // Contact
            Label lblContact = new Label { Text = "Contact:", Top = 60, Left = 20, Width = 120 };
            txtContact = new TextBox { Top = 60, Left = 150, Width = 200, Text = "09** *** ****" };
            {
                if (txtContact.Text == "09** *** ****")
                {
                    txtContact.Text = "";
                    txtContact.ForeColor = Color.Black;
                }
            };
            txtContact.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtContact.Text))
                {
                    txtContact.Text = "09** *** ****";
                    txtContact.ForeColor = Color.Gray;
                }
            };

            // Role
            Label lblRole = new Label { Text = "Role:", Top = 100, Left = 20, Width = 120 };
            cmbRole = new ComboBox { Top = 100, Left = 150, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new string[] { "Staff", "Teacher", "Student" });
            cmbRole.SelectedIndexChanged += CmbRole_SelectedIndexChanged;

            // Grade and Section(hidden muna)
            lblWhere = new Label { Text = "Grade and Section:", Top = 140, Left = 20, Width = 120, Visible = false };
            txtWhere = new TextBox { Top = 140, Left = 150, Width = 200, Visible = false, ForeColor = Color.Gray, Text = "N/A" };
            txtWhere.GotFocus += (s, e) =>
            {
                if (txtWhere.Text == "N/A")
                {
                    txtWhere.Text = "";
                    txtWhere.ForeColor = Color.Black;
                }
            };
            txtWhere.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtWhere.Text))
                {
                    txtWhere.Text = "N/A";
                    txtWhere.ForeColor = Color.Gray;
                }
            };

            // When Found
            Label lblWhen = new Label { Text = "When Found:", Top = 180, Left = 20, Width = 120 };
            txtWhen = new TextBox { Top = 180, Left = 150, Width = 200, Text = DateTime.Now.ToString("MM/dd/yyyy HH:mm") };

            // Submit
            btnSubmit = new Button
            {
                Text = "Submit",
                Top = 230,
                Left = 150,
                Width = 100,
                BackColor = Color.Aqua,
                ForeColor = Color.White
            };
            btnSubmit.Click += (s, e) => { this.DialogResult = DialogResult.OK; this.Close(); };

            // Add controls
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblContact);
            this.Controls.Add(txtContact);
            this.Controls.Add(lblRole);
            this.Controls.Add(cmbRole);
            this.Controls.Add(lblWhere);
            this.Controls.Add(txtWhere);
            this.Controls.Add(lblWhen);
            this.Controls.Add(txtWhen);
            this.Controls.Add(btnSubmit);
        }

        private void CmbRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRole.SelectedItem?.ToString() == "Student")
            {
                lblWhere.Visible = true;
                txtWhere.Visible = true;
            }
            else
            {
                lblWhere.Visible = false;
                txtWhere.Visible = false;
            }
        }
    }
}
