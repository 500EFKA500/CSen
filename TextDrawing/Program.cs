using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using TextDrawer;
using TextWindow;

namespace TextDrawing
{
    internal static class Program
    {
        [STAThread]
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private static void Main()
        {
            AppDomain drawerDomain = null;
            AppDomain inputDomain = null;
            DrawerApplication drawer = null;
            Thread drawerThread = null;
            try
            {
                AppDomainSetup setup = new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory };
                drawerDomain = AppDomain.CreateDomain("Drawer", null, setup);
                inputDomain = AppDomain.CreateDomain("TextWindow", null, setup);
                drawerDomain.DomainUnload += Drawer_DomainUnload;
                drawer = (DrawerApplication)drawerDomain.CreateInstanceFromAndUnwrap(
                    Path.Combine(setup.ApplicationBase, "TextDrawer.exe"), typeof(DrawerApplication).FullName);
                InputApplication input = (InputApplication)inputDomain.CreateInstanceFromAndUnwrap(
                    Path.Combine(setup.ApplicationBase, "TextWindow.exe"), typeof(InputApplication).FullName);
                drawerThread = new Thread(drawer.Run);
                drawerThread.SetApartmentState(ApartmentState.STA);
                drawerThread.Start();
                drawer.WaitUntilReady();
                Exception inputError = null;
                Thread inputThread = new Thread(() =>
                {
                    try { input.Run(drawer); }
                    catch (Exception ex) { inputError = ex; }
                });
                inputThread.SetApartmentState(ApartmentState.STA);
                inputThread.Start();
                inputThread.Join();
                if (inputError != null) throw new InvalidOperationException("Ошибка окна ввода.", inputError);
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "TextDrawing", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally
            {
                if (drawer != null) drawer.Close();
                if (drawerThread != null && drawerThread.IsAlive) drawerThread.Join();
                if (inputDomain != null) AppDomain.Unload(inputDomain);
                if (drawerDomain != null) AppDomain.Unload(drawerDomain);
            }
        }
        private static void Drawer_DomainUnload(object sender, EventArgs e)
        {
            MessageBox.Show("Domain with name " + AppDomain.CurrentDomain.FriendlyName +
                " has been successfully unloaded!", "DomainUnload");
        }
    }
}
