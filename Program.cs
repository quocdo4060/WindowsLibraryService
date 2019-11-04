using System;
using System.Configuration;
using System.Threading;
using Powell.ENG.Log.Performance;
using log4net;
using System.ServiceProcess;

namespace Powell.Mfg.LibraryWinService
{
    internal static class Program
    {
        #if _PERFORMANCE_LOG_
            private static readonly IPerfLog PerfLog = PerfLogManager.GetLog(typeof (Program));
        #endif
        
        private static readonly ILog Logger = LogManager.GetLogger(typeof (Program));


        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Main_UnhandledException);
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["debugmode"].ToString()))
                {
                    // Debug code: this allows the process to run as a non-service.
                    // It will kick off the service start point, but never kill it.
                    // Shut down the debugger to exit
                    #if _PERFORMANCE_LOG_
                        PerfLog.Enter("Program.InDebugBase");
                    #endif

                        ModelQueue.LogToOracle(0, "Main", "InDebugBase", "starting up");

                    var service = new Service1();
                    service.MainProcessor();
                    Thread.Sleep(Timeout.Infinite);
                }
                else
                {
                    #if _PERFORMANCE_LOG_
                        PerfLog.Enter("Program.InServiceBase");
                    #endif

                        //ModelQueue.LogToOracle(0, "Main", "InServiceBase", "starting up");

                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[] 
			        { 
				        new Service1() 
			        };
                    ServiceBase.Run(ServicesToRun);
                }
            }
            catch (Exception e)
            {
                #if _PERFORMANCE_LOG_
                    PerfLog.Enter("Program.Exception-" + e.Message);
                #endif

                    ModelQueue.LogToOracle(0, "Main", "fatal exception", e.Message);

                Logger.Fatal("Unhandled Exception.", e);
                throw;
            }
            finally
            {
            }

            #if _PERFORMANCE_LOG_
                PerfLog.Exit();
            #endif
        }

        private static void Main_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;

            ModelQueue.LogToOracle(0, "Main", "unhandled exception", ex.Message);

            #if _PERFORMANCE_LOG_
                PerfLog.Enter("UNHANDLED EXCEPTION:" + ex.Message);
            #endif
        }


    }
}