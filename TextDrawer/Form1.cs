using System;
using System.Drawing;
using System.Windows.Forms;

namespace TextDrawer
{
    public sealed class Form1 : Form
    {
        private string SourceText = "No text was added!";
        private Font DrawingFont = new Font("Arial", 45);
        private readonly Panel panel1 = new DrawingPanel();
        public Form1()
        {
            Text = "TextDrawer";
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            ControlBox = false;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(200, 300);
            ClientSize = new Size(600, 300);
            MinimumSize = new Size(240, 140);
            MenuStrip menu = new MenuStrip();
            ToolStripMenuItem font = new ToolStripMenuItem("Font");
            font.Click += SelectFont;
            menu.Items.Add(font);
            MainMenuStrip = menu;
            panel1.Dock = DockStyle.Fill;
            panel1.Paint += Panel1_Paint;
            Controls.Add(panel1);
            Controls.Add(menu);
            Paint += delegate { panel1.Invalidate(); };
        }
        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            if (panel1.ClientSize.Width <= 0 || panel1.ClientSize.Height <= 0) return;
            using (Bitmap image = new Bitmap(panel1.ClientSize.Width, panel1.ClientSize.Height))
            using (Graphics graphics = Graphics.FromImage(image))
            using (StringFormat format = new StringFormat(StringFormatFlags.NoFontFallback))
            {
                graphics.Clear(panel1.BackColor);
                graphics.DrawString(SourceText, DrawingFont, Brushes.Brown, panel1.ClientRectangle, format);
                e.Graphics.DrawImageUnscaled(image, 0, 0);
            }
        }
        private void SelectFont(object sender, EventArgs e)
        {
            using (FontDialog dialog = new FontDialog { Font = DrawingFont })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                Font previous = DrawingFont;
                DrawingFont = (Font)dialog.Font.Clone();
                previous.Dispose();
                panel1.Invalidate();
            }
        }
        public void SetText(string text) { SourceText = text ?? string.Empty; panel1.Invalidate(); }
        public new void Move(Point newLocation, int width) { Location = newLocation; Width = width; }
        protected override void Dispose(bool disposing)
        {
            if (disposing) DrawingFont.Dispose();
            base.Dispose(disposing);
        }
        private sealed class DrawingPanel : Panel
        {
            public DrawingPanel() { DoubleBuffered = true; ResizeRedraw = true; }
        }
    }
}
