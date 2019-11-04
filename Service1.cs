using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using Powell.ENG.Log.Performance;
using Powell.Mfg.LibraryWinService.LibraryServiceReference;
using aejw.Network;
using log4net;
using System.Collections.ObjectModel;
using System.Threading;

namespace Powell.Mfg.LibraryWinService
{
    public partial class Service1 : ServiceBase
    {
        #if _PERFORMANCE_LOG_
            private static readonly IPerfLog PerfLog = PerfLogManager.GetLog(typeof (Service1));
        #endif
        
        private static readonly ILog Logger = LogManager.GetLogger(typeof (Service1));
        public List<DxfLayers> theLayers;
        //private readonly Timer _timer = new Timer();
        public static double ServicePollInterval;
        public static string Nothreads;
        public static bool IsStarted = false;

        public Service1()
        {
            try
            {
                InitializeComponent();

                //ServicePollInterval = Convert.ToInt32(ConfigurationManager.AppSettings["TimerIntervalSeconds"]);
                //ServicePollInterval = ServicePollInterval*1000;

                //_timer.Elapsed += TimerElapsed;
                ////providing the time in miliseconds 
                //_timer.Interval = ServicePollInterval;
                //_timer.AutoReset = true;
                
                //#if _PERFORMANCE_LOG_
                //    PerfLog.Enter("Interval set to " + ServicePollInterval.ToString(CultureInfo.InvariantCulture));
                //#endif

                //StartTimer();

            }
            catch (Exception e)
            {
                #if _PERFORMANCE_LOG_
                    PerfLog.Enter("Service1 constructor: " + e.Message);
                #endif
                throw;
            }
            finally
            {
                #if _PERFORMANCE_LOG_                
                    PerfLog.Exit();
                #endif
            }
        }

        protected override void OnStart(string[] args)
        {
            #if _PERFORMANCE_LOG_
                PerfLog.Enter("Starting Service1");
            #endif

            try
            {
                ModelQueue.ResetQueueAll();
                #if _PERFORMANCE_LOG_
                    PerfLog.Enter("Starting Service1");
                    PerfLog.Enter("Resetting Queue");
                #endif
                //StartTimer();
                MainProcessor();
            }
            catch (Exception e)
            {
                Logger.Error("Unexpected Exception.", e);
                throw;
            }
            finally
            {
            }
            
            #if _PERFORMANCE_LOG_
                PerfLog.Exit();
            #endif
        }

        //public void StartTimer()
        //{
        //    _timer.Enabled = true;
        //    _timer.Start();

        //    #if _PERFORMANCE_LOG_
        //        PerfLog.Enter("Timer started");
        //    #endif
        //}

        protected override void OnContinue()
        {
            try
            {
                base.OnContinue();
                //_timer.Start();                
            }
            catch (Exception e)
            {
                Logger.Error("Unexpected Exception.", e);
                throw;
            }
            finally
            {
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                //_timer.Stop();
            }
            catch (Exception e)
            {
                Logger.Error("Unexpected Exception.", e);
                throw;
            }
            finally
            {
            }
        }

        protected override void OnShutdown()
        {
            #if _PERFORMANCE_LOG_
                PerfLog.Enter();
            #endif
            
            try
            {
                base.OnShutdown();
                //_timer.Stop();
            }
            catch (Exception e)
            {
                Logger.Error("Unexpected Exception.", e);
                throw;
            }
            finally
            {
            }

            #if _PERFORMANCE_LOG_
                PerfLog.Exit();
            #endif
        }

        protected override void OnStop()
        {
            #if _PERFORMANCE_LOG_
                PerfLog.Enter("Stopping Service1");
                PerfLog.Enter("Resetting Queue");
            #endif
            
            try
            {
                //EventLog.WriteEntry("Stopped the service");
                EventLog.WriteEntry("WINLIBSVC", "Stopped the service");
                //_timer.Stop();
                ModelQueue.ResetQueueAll();
            }
            catch (Exception e)
            {
                Logger.Error("Unexpected Exception.", e);
                throw;
            }
            finally
            {
            }

            #if _PERFORMANCE_LOG_
                PerfLog.Exit();
            #endif
        }


        //private void TimerElapsed(object sender, ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        MainProcessor();
        //    }
        //    catch (Exception ex)
        //    {
        //        #if _PERFORMANCE_LOG_
        //            PerfLog.Enter("FATAL_ERROR CAUGHT:" + ex.Message);
        //        #endif
        //    }
        //    finally
        //    {
        //    }

        //    #if _PERFORMANCE_LOG_
        //        PerfLog.Exit();
        //    #endif
        //}


        /// <summary>
        ///     Main Processing of the queue using library service
        /// </summary>
        //this method will complete before all the threads finish, freeing it up for the next timer_Elapsed
        public void MainProcessor()
        {
            try
            {
                List<NestPost> livePosts = ModelQueue.GetAllPosts();
                if (livePosts.Count > 0)
                {
                    foreach (NestPost post in livePosts)
                    {
                        //for testing only
                        if (post.POSTNAME == "ol2")
                        {
                            Task.Factory.StartNew(() => ModelQueue.ProcessFile(post.POSTNAME, post.SYNCPOST));
                        }

                        //Task.Factory.StartNew(() => ModelQueue.ProcessFile(post.POSTNAME, post.SYNCPOST));
                    }
                }

                ModelQueue.ResetQueueAll();
                ModelQueue.LogToOracle(0, "MainProcessor", "All Posts", "Queue reset");
            }
            catch (Exception e)
            {
                ModelQueue.LogToOracle(0, "MainProcessor", "Exception", e.Message);

                #if _PERFORMANCE_LOG_
                    PerfLog.Enter("MainProcessor: " + e.Message);
                #endif    
            }
            finally
            {
                ModelQueue.LogToOracle(0, "MainProcessor", "", "Main completing");

                #if _PERFORMANCE_LOG_
                    PerfLog.Exit();
                #endif
            }
        }
    }
}