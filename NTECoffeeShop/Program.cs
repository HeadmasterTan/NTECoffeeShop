using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using NLog;

namespace NTECoffeeShop
{
    internal static class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 重定向兼容DLL加载，解决在某些环境下缺少System.Runtime.CompilerServices.Unsafe.dll导致的运行时错误
        /// </summary>
        private static void RedirectCompatibleDll()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                if (args.Name.Contains("System.Runtime.CompilerServices.Unsafe"))
                {
                    string folderPath = AppDomain.CurrentDomain.BaseDirectory;
                    string assemblyPath = Path.Combine(folderPath, "System.Runtime.CompilerServices.Unsafe.dll");

                    if (File.Exists(assemblyPath))
                    {
                        return Assembly.LoadFrom(assemblyPath);
                    }
                }

                return null;
            };
        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                RedirectCompatibleDll();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // 1. 设置处理异常的模式：全部强制通过注册的事件处理
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                // 2. 捕获 UI 线程异常
                Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
                // 3. 捕获非 UI 线程异常（如后台线程、Task中的异常）
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                Application.Run(new Form1());
            }
            catch (Exception e)
            {
                Log.Error(e, "入口函数异常");
                throw;
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Log.Error(e.Exception, "UI线程未处理异常");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(e.ExceptionObject as Exception, "非UI线程/全局未处理异常");
        }
    }
}
