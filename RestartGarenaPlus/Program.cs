using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace RestartGarenaPlus
{
    class Program
    {
        const string processName = "Garena";
        const string processPath = @"C:\Program Files (x86)\Garena\Garena\Garena.exe";
        const string serviceName = "GarenaPlatform";

        static void Main(string[] args)
        {
            KillProccess("LeagueClientUxRender");
            KillProccess("LeagueClientUx");
            KillProccess("LeagueClient");
            KillProccess("Garena");
            StopService(serviceName);
            StartProccess(processPath);
            Thread.Sleep(2000);
        }

        private static bool KillProccess(string processName)
        {
            try
            {
                foreach (var process in Process.GetProcessesByName(processName))
                {
                    process.Kill();
                    Console.WriteLine("Kill " + processName + ".exe... OK");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Kill " + processName + ".exe... Fail" + Environment.NewLine + ex.Message);
                return false;
            }
        }

        private static bool StartProccess(string exePath)
        {
            try
            {
                Process.Start(exePath);
                Console.WriteLine("Start " + processName + ".exe... OK");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Start " + processName + ".exe... Fail" + Environment.NewLine + ex.Message);
                return false;
            }
        }

        private static bool StartService(string serviceName)
        {
            try
            {
                // 已知的 Service Name
                string m_ServiceName = serviceName;

                // 建立 ServiceController 物件實體
                ServiceController service = new ServiceController(m_ServiceName);

                // 設定一個 Timeout 時間，若超過 10 秒啟動不成功就宣告失敗!
                TimeSpan timeout = TimeSpan.FromMilliseconds(1000 * 10);

                // 啟動服務
                service.Start();

                // 設定該服務必須在等待 timeout 時間內將狀態改變至「已啟動(Running)」的狀態
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);

                Console.WriteLine("Start " + serviceName + " ... OK");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Start " + serviceName + " ... Fail" + Environment.NewLine + ex.Message);
                return false;
            }
        }

        private static bool StopService(string serviceName)
        {
            try
            {
                string m_ServiceName = serviceName;
                ServiceController service = new ServiceController(m_ServiceName);

                // 設定一個 Timeout 時間，若超過 10 秒啟動不成功就宣告失敗!
                TimeSpan timeout = TimeSpan.FromMilliseconds(1000 * 10);

                // 若該服務不是「停用」的狀態，才將其停止運作，否則會引發 Exception
                if (service.Status != ServiceControllerStatus.Stopped &&
                    service.Status != ServiceControllerStatus.StopPending)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
                Console.WriteLine("Stop " + serviceName + " ... OK");
                return true;
            }
            catch (Exception ex)
            {
                // 如果無法停用服務會引發 Exception，也會讓反安裝自動中斷
                Console.WriteLine("Stop " + serviceName + " ... Fail" + Environment.NewLine + ex.Message);
                //throw new InstallException("服務無法停用，建議您可以先利用「工作管理員」將 Service1.exe 程序結束，再進行解除安裝。");
                return false;
            }
        }
    }
}
