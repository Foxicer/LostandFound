using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ReturnPoint
{
    public static class Theme
    {
        // Palette - ReturnPoint Theme
        public static Color SoftWhite => ColorTranslator.FromHtml("#FFFFFF");
        public static Color LightGray => ColorTranslator.FromHtml("#F5F5F5");
        public static Color MediumGray => ColorTranslator.FromHtml("#E0E0E0");
        public static Color DarkGray => ColorTranslator.FromHtml("#505050");
        public static Color TealGreen => ColorTranslator.FromHtml("#00C0A0");
        public static Color MediumTeal => ColorTranslator.FromHtml("#00B8A0");
        public static Color DarkTeal => ColorTranslator.FromHtml("#00A088");
        public static Color MutedTeal => ColorTranslator.FromHtml("#009880");
        public static Color OuterRing = ColorTranslator.FromHtml("#009880"); 

        public static Color NearBlack => ColorTranslator.FromHtml("#000000");
        public static Color DeepRed => ColorTranslator.FromHtml("#980000");
        public static Color CrimsonRed => ColorTranslator.FromHtml("#A00000");
        public static Color DarkRoseRed => ColorTranslator.FromHtml("#984040");
        public static Color GoldenYellow => ColorTranslator.FromHtml("#C09820");
        public static Color WarmGold => ColorTranslator.FromHtml("#B89028");
        public static Color AmberGold => ColorTranslator.FromHtml("#C08828");
        public static Color PaperWhite => ColorTranslator.FromHtml("#FFFFFF");
        public static Color DarkGrayBook => ColorTranslator.FromHtml("#2B2B2B");

        public static void Apply(Form form)
        {

            if (form == null) return;
            form.BackColor = MutedTeal;
            foreach (Control c in form.Controls)
                ApplyToControl(c);

        }

        private static bool IsLight(Color c)
        {
            double lum = (0.299 * c.R + 0.587 * c.G + 0.114 * c.B) / 255.0;
            return lum > 0.75;
        }

        private static void ApplyToControl(Control c)
        {
            if (c == null) return;

            bool bgIsLight = IsLight(c.BackColor);

            if (c is Panel || c is GroupBox || c is FlowLayoutPanel)
            {
                c.BackColor = MediumGray;
                c.ForeColor = NearBlack;
            }
            else if (c is TableLayoutPanel)
            {
                c.BackColor = MutedTeal;
                c.ForeColor = NearBlack;
            }
            else if (c is Button btn)
            {
                // Use teal green for buttons
                btn.BackColor = TealGreen;
                btn.ForeColor = SoftWhite;
                btn.FlatStyle = FlatStyle.Flat;
                try { btn.FlatAppearance.BorderSize = 0; } catch { }
            }
            else if (c is TextBox || c is MaskedTextBox || c is RichTextBox)
            {
                c.BackColor = SoftWhite;
                c.ForeColor = NearBlack;
            }
            else if (c is ListBox || c is ComboBox)
            {
                c.BackColor = SoftWhite;
                c.ForeColor = NearBlack;
            }
            else if (c is Label)
            {
                c.ForeColor = NearBlack;
                c.BackColor = Color.Transparent;
            }
            else if (c is PictureBox)
            {
                c.BackColor = Color.Transparent;
            }
            else
            {
                c.ForeColor = NearBlack;
            }

            // recurse children
            foreach (Control child in c.Controls.Cast<Control>().ToArray())
                ApplyToControl(child);
        }
    }
}