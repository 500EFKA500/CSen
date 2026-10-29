using System;
using System.Threading;
using System.Windows.Forms;

namespace TextDrawer
{
    // The proxy lives in Drawer; every UI operation is dispatched to its STA thread.
    public sealed class DrawerApplication : MarshalByRefObject
    {
        private readonly ManualResetEvent ready = new ManualResetEvent(false);
        private Form1 window;
        private string startupError;
        public override object InitializeLifetimeService() { return null; }
        public void Run()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                using (window = new Form1())
                {
                    window.Shown += delegate { ready.Set(); };
                    Application.Run(window);
                }
            }
            catch (Exception ex) { startupError = ex.Message; }
            finally { ready.Set(); }
        }
        public void WaitUntilReady()
        {
            if (!ready.WaitOne(15000)) throw new TimeoutException("Окно отрисовки не запустилось.");
            if (startupError != null) throw new InvalidOperationException(startupError);
        }
        private void Dispatch(Action action)
        {
            if (window == null || window.IsDisposed || !window.IsHandleCreated) return;
            try { window.BeginInvoke(action); }
            catch (InvalidOperationException) { /* The window has already closed. */ }
        }
        public void SetText(string text) { Dispatch(() => window.SetText(text)); }
        public void Move(System.Drawing.Point location, int width) { Dispatch(() => window.Move(location, width)); }
        public void Close() { Dispatch(() => window.Close()); }
    }
    internal static class Program
    {
        [STAThread]
        private static void Main() { new DrawerApplication().Run(); }
    }
}
