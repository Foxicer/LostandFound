using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ReturnPoint
{
    public static class Theme
    {
        // Palette
        public static Color SoftWhite => ColorTranslator.FromHtml("#FFFEFE");
        public static Color OuterRing => ColorTranslator.FromHtml("#0EC1A9");
        public static Color StrongAqua => ColorTranslator.FromHtml("#00C0A5");
        public static Color MutedTurquoise => ColorTranslator.FromHtml("#26B295");
        public static Color MediumAqua => ColorTranslator.FromHtml("#00B69A");
        public static Color DeepTeal => ColorTranslator.FromHtml("#00A08A");
        public static Color SlightlyDarkerTeal => ColorTranslator.FromHtml("#00A089");
        public static Color DarkGray => ColorTranslator.FromHtml("#505346");

        public static void Apply(Form form)
        {
            if (form == null) return;

            // Form background only
            form.BackColor = MutedTurquoise;

            // Walk controls
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

            if (c is Panel || c is GroupBox || c is FlowLayoutPanel || c is TableLayoutPanel)
            {
                c.BackColor = SlightlyDarkerTeal;
                c.ForeColor = SoftWhite;
            }
            else if (c is Button btn)
            {
                // keep action buttons teal by default; if background was light, prefer white button w/ black text
                if (bgIsLight)
                {
                    btn.BackColor = Color.White;
                    btn.ForeColor = Color.Black;
                }
                else
                {
                    btn.BackColor = StrongAqua;
                    btn.ForeColor = SoftWhite;
                }
                btn.FlatStyle = FlatStyle.Flat;
                try { btn.FlatAppearance.BorderSize = 0; } catch { }
            }
            else if (c is TextBox || c is MaskedTextBox || c is RichTextBox)
            {
                c.BackColor = Color.White;
                c.ForeColor = Color.Black;
            }
            else if (c is ListBox || c is ComboBox)
            {
                c.BackColor = Color.White;
                c.ForeColor = Color.Black;
            }
            else if (c is Label)
            {
                c.ForeColor = bgIsLight ? Color.Black : SoftWhite;
                c.BackColor = Color.Transparent;
            }
            else if (c is PictureBox)
            {
                c.BackColor = Color.Transparent;
            }
            else
            {
                c.ForeColor = bgIsLight ? Color.Black : SoftWhite;
            }

            // recurse children
            foreach (Control child in c.Controls.Cast<Control>().ToArray())
                ApplyToControl(child);
        }
    }
}