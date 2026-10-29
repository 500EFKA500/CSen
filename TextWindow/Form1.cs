using System;
using System.Drawing;
using System.Windows.Forms;
using TextDrawer;

namespace TextWindow
{
    public sealed class Form1 : Form
    {
        private readonly DrawerApplication drawer;
        private readonly TextBox textBox1 = new TextBox();
        public Form1() : this(null) { }
        public Form1(DrawerApplication targetWindow)
        {
            drawer = targetWindow;
            Text = "TextWindow";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(200, 180);
            ClientSize = new Size(600, 75);
            Padding = new Padding(12);
            textBox1.Dock = DockStyle.Fill;
            textBox1.Multiline = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.TextChanged += delegate { if (drawer != null) drawer.SetText(textBox1.Text); };
            Controls.Add(textBox1);
            LocationChanged += UpdateDrawerLocation;
            SizeChanged += UpdateDrawerLocation;
            Shown += delegate { UpdateDrawerLocation(this, EventArgs.Empty); textBox1.Focus(); };
        }
        private void UpdateDrawerLocation(object sender, EventArgs e)
        {
            if (drawer != null) drawer.Move(new Point(Left, Bottom), Width);
        }
    }
}
