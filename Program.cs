using log4net;
using Microsoft.Win32;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace WinFormsApp1
{
    internal static class Program
    {
        public static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static private NotifyIcon notifyIcon;
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();


        static private Icon LoadIconFromResource(string resourceName)
        {
            using (var stream = File.OpenRead(resourceName))
            {
                return new Icon(stream);
            }
        }

        static public void ShowNotification(string text, string title)
        {
            // ��ʾ���б�����ı���֪ͨ
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = text;
            notifyIcon.ShowBalloonTip(5000); // ��ʾ5��
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //����δ������쳣
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //����UI�߳��쳣
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            //�����UI�߳��쳣
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // ����һ������̨
            AllocConsole();

            ApplicationConfiguration.Initialize();
            // ��ʼ��NotifyIcon�ؼ�
            notifyIcon = new NotifyIcon
            {
                Icon = LoadIconFromResource("favicon.ico"),
                Text = "MyFiddler",
                Visible = true,
            };
            DisableSystemProxy();
            Application.Run(new Form1());
            FreeConsole();
            DisableSystemProxy();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            log.Error("Program Main ThreadException", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error("Program Main UnhandledException", e.ExceptionObject as Exception);
        }

        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        private const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        private const int INTERNET_OPTION_REFRESH = 37;

        private static void NotifyNetworkChange()
        {
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }

        public static void DisableSystemProxy()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true))
                {
                    if (key != null)
                    {
                        // ���ô���
                        key.SetValue("ProxyEnable", 0, RegistryValueKind.DWord);
                    }
                }

                // ֪ͨϵͳ�����Ѹ���
                NotifyNetworkChange();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"�ر�ϵͳ����ʱ��������: {ex.Message}");
            }
        }
    }
}