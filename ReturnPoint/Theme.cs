using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ReturnPoint
{
    public static class Theme
    {
        public static int BackgroundTealShade = 1;
        
        public static Color SoftWhite => ColorTranslator.FromHtml("#FFFFFF");
        public static Color LightGray => ColorTranslator.FromHtml("#F8F9FA");
        public static Color MediumGray => ColorTranslator.FromHtml("#E9ECEF");
        public static Color DarkGray => ColorTranslator.FromHtml("#495057");
        public static Color TealGreen => ColorTranslator.FromHtml("#00D9A3");
        public static Color MediumTeal => ColorTranslator.FromHtml("#00C896");
        public static Color DarkTeal => ColorTranslator.FromHtml("#009B7D");
        public static Color MutedTeal => ColorTranslator.FromHtml("#007C63");
        public static Color OuterRing = ColorTranslator.FromHtml("#00A080");
        
        public static Color GetBackgroundTeal()
        {
            return BackgroundTealShade switch
            {
                0 => MutedTeal,
                1 => MediumTeal,
                2 => DarkTeal,
                _ => MediumTeal
            };
        }

        public static Color GetDarkerTeal()
        {
            return BackgroundTealShade switch
            {
                0 => ColorTranslator.FromHtml("#005D4F"),
                1 => ColorTranslator.FromHtml("#009B7D"),
                2 => ColorTranslator.FromHtml("#007C63"),
                _ => ColorTranslator.FromHtml("#009B7D")
            };
        }

        public static Color GetLighterTeal()
        {
            return BackgroundTealShade switch
            {
                0 => ColorTranslator.FromHtml("#00C896"),
                1 => ColorTranslator.FromHtml("#00E8B3"),
                2 => ColorTranslator.FromHtml("#00C896"),
                _ => ColorTranslator.FromHtml("#00E8B3")
            };
        }

        public static Bitmap CreateGradientBitmap(int width, int height, bool vertical = true)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Color startColor = GetBackgroundTeal();
                Color endColor = GetDarkerTeal();
                
                using (var brush = vertical 
                    ? new LinearGradientBrush(
                        new Point(0, 0), 
                        new Point(0, height), 
                        startColor, 
                        endColor)
                    : new LinearGradientBrush(
                        new Point(0, 0), 
                        new Point(width, 0), 
                        startColor, 
                        endColor))
                {
                    g.FillRectangle(brush, 0, 0, width, height);
                }
            }
            return bmp;
        }

        public static Color NearBlack => ColorTranslator.FromHtml("#1A1A1A");
        public static Color DeepRed => ColorTranslator.FromHtml("#DC3545");
        public static Color CrimsonRed => ColorTranslator.FromHtml("#E74C3C");
        public static Color DarkRoseRed => ColorTranslator.FromHtml("#C0392B");
        public static Color GoldenYellow => ColorTranslator.FromHtml("#F39C12");
        public static Color WarmGold => ColorTranslator.FromHtml("#E67E22");
        public static Color AmberGold => ColorTranslator.FromHtml("#F0AD4E");
        public static Color PaperWhite => ColorTranslator.FromHtml("#FFFFFF");
        public static Color DarkGrayBook => ColorTranslator.FromHtml("#2C3E50");
        
        public static Color AccentBlue => ColorTranslator.FromHtml("#3498DB");
        public static Color LightAccent => ColorTranslator.FromHtml("#ECF0F1");
        public static Color Success => ColorTranslator.FromHtml("#27AE60");

        public static void Apply(Form form)
        {
            if (form == null) return;
            form.BackColor = GetBackgroundTeal();
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
                c.BackColor = MediumTeal;
                c.ForeColor = NearBlack;
            }
            else if (c is TableLayoutPanel)
            {
                c.BackColor = MutedTeal;
                c.ForeColor = NearBlack;
            }
            else if (c is Button btn)
            {
                
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

            
            foreach (Control child in c.Controls.Cast<Control>().ToArray())
                ApplyToControl(child);
        }
    }
}
