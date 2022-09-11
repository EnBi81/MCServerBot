using MCWebServer.Log;
using MCWebServer.MinecraftServer.Enums;
using System.Management;
using System.Threading;
using System;
using System.Diagnostics;

namespace MCWebServer.MinecraftServer.Util
{
    public class ProcessPerformanceReporter
    {
        private readonly Process _mcProcess;
        private volatile bool _isRunning;
        private readonly Thread _measurementThread;

        public ProcessPerformanceReporter(int processId)
        {
            _mcProcess = Process.GetProcessById(processId);
            _isRunning = true;
            _measurementThread = new Thread(PerformanceReporter);
        }

        public void Start()
        {
            _measurementThread.Start();
        }

        public void Stop()
        {
            _isRunning = false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private void PerformanceReporter()
        {
            var objQuery = new ObjectQuery("select * from Win32_Process WHERE ProcessID = " + _mcProcess.Id);
            var moSearcher = new ManagementObjectSearcher(objQuery);
            DateTime firstSample = DateTime.MinValue, secondSample;

            double OldProcessorUsage = 0;
            double ProcessorUsage = 20;
            double msPassed;
            ulong u_OldCPU = 0;
            string cpu = "";
            string memory = "";
            while (_isRunning)
            {
                var gets = moSearcher.Get();
                foreach (ManagementObject mObj in gets)
                {
                    try
                    {
                        if (firstSample == DateTime.MinValue)
                        {
                            firstSample = DateTime.Now;
                            mObj.Get();
                            u_OldCPU = (ulong)mObj["UserModeTime"] + (ulong)mObj["KernelModeTime"];
                        }
                        else
                        {
                            secondSample = DateTime.Now;
                            mObj.Get();
                            ulong u_newCPU = (ulong)mObj["UserModeTime"] + (ulong)mObj["KernelModeTime"];

                            msPassed = (secondSample - firstSample).TotalMilliseconds;
                            OldProcessorUsage = ProcessorUsage;
                            ProcessorUsage = (u_newCPU - u_OldCPU) / (msPassed * 100.0 * Environment.ProcessorCount);

                            u_OldCPU = u_newCPU;
                            firstSample = secondSample;
                        }


                        _mcProcess.Refresh();
                        memory = (_mcProcess.WorkingSet64 / (1024 * 1024)) + " MB";
                        cpu = ProcessorUsage.ToString("0.00") + "%";

                        RaiseEvent(PerformanceMeasured, (cpu, memory));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + ex.StackTrace);
                        Console.WriteLine(ex.InnerException.Message);
                    }
                }

                int waitTime = ProcessorUsage > 10 ? 1 : 5;
                double cpuDifference = ProcessorUsage - OldProcessorUsage;

                if (Math.Abs(cpuDifference) < 6)
                    waitTime *= 2;


                Thread.Sleep(waitTime * 1000);
            }


            moSearcher.Dispose();
            RaiseEvent(PerformanceMeasured, ("0%", "0 MB"));
        }

        public event EventHandler<(string CPU, string Memory)> PerformanceMeasured;

        protected void RaiseEvent<T>(EventHandler<T> evt, T param)
        {
            //Console.WriteLine($"Event raised: {evt.Method.Name} with data: {param}");
            evt?.Invoke(this, param);
        }
    }
}
