using System;
using System.Windows.Forms;
using TextDrawer;

namespace TextWindow
{
    public sealed class InputApplication : MarshalByRefObject
    {
        public override object InitializeLifetimeService() { return null; }
        public void Run(DrawerApplication drawer)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (Form1 window = new Form1(drawer)) Application.Run(window);
        }
    }
    internal static class Program
    {
        [STAThread]
        private static void Main() { new InputApplication().Run(null); }
    }
}
