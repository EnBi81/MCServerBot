using MCWebServer.Log;
using MCWebServer.MinecraftServer.Enums;
using System.Management;
using System.Threading;
using System;
using System.Diagnostics;

namespace MCWebServer.MinecraftServer.Util
{
    /// <summary>
    /// Class responsible for measuring the cpu and memory usage of a process.
    /// </summary>
    public class ProcessPerformanceReporter
    {
        private readonly Process _mcProcess; // process to measure
        private volatile bool _isRunning; 
        private readonly Thread _measurementThread;


        /// <summary>
        /// Initializes the ProcessPerformanceReporter object
        /// </summary>
        /// <param name="processId">id of the process to measure.</param>
        public ProcessPerformanceReporter(int processId)
        {
            _mcProcess = Process.GetProcessById(processId);
            _measurementThread = new Thread(PerformanceReporter);
        }

        /// <summary>
        /// Starts the measurement on a separate thread.
        /// </summary>
        public void Start()
        {
            _isRunning = true;
            _measurementThread.Start();
        }

        /// <summary>
        /// Stops the measurement thread.
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
        }

        /// <summary>
        /// Measures the cpu and memory usage time by time, and invokes the <see cref="PerformanceMeasured"/> event.
        /// </summary>
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
                        cpu = ProcessorUsage.ToString("0.00") + " %";

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
            RaiseEvent(PerformanceMeasured, ("0 %", "0 MB"));
        }

        /// <summary>
        /// Raised when a measurement result is ready.
        /// </summary>
        public event EventHandler<(string CPU, string Memory)> PerformanceMeasured;

        /// <summary>
        /// Helper method for raising events.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="evt"></param>
        /// <param name="param"></param>
        protected void RaiseEvent<T>(EventHandler<T> evt, T param)
        {
            //Console.WriteLine($"Event raised: {evt.Method.Name} with data: {param}");
            evt?.Invoke(this, param);
        }
    }
}
