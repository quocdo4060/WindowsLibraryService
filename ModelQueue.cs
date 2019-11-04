using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using Powell.ENG.Log.Performance;
using Powell.Mfg.LibraryWinService.LibraryServiceReference;
using Powell.Mfg.LibraryWinService.PartServiceReference;
using Powell.Mfg.LibraryWinService.ResourceServiceReference;
using aejw.Network;
using log4net;
using System.Collections.ObjectModel;

namespace Powell.Mfg.LibraryWinService
{
    public static class ModelQueue
    {
        #if _PERFORMANCE_LOG_
                private static readonly IPerfLog PerfLog = PerfLogManager.GetLog(typeof(ModelQueue));
        #endif
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ModelQueue));



        /// <summary>
        ///     Gets the next record in the queue for either any resource (ALL) or the resource that is passed
        /// </summary>
        /// <param name="resource">Work Center Name or ALL</param>
        /// <param name="numRecsToReturn">Number of Records to Return</param>
        /// <returns>oldest Queue Record to process</returns>
        public static List<QueueRecord> GetNextQueueRec(string resource, int numRecsToReturn)
        {
            //Changed By: Sandeep Kumar
            //Changed Date: 03/26/2015
            //Change Description: Wrap the code inside using block. eCR #3075.
            using (var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService"))
            {
                List<QueueRecord> retRec = new List<QueueRecord>();
                try
                {
                    //var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService");
                    retRec = myProxy.GetNextQueueRecord(resource, numRecsToReturn);
                }
                catch (Exception e)
                {
                    myProxy.Abort();
                    #if _PERFORMANCE_LOG_
                        //PerfLog.Enter("GetNextQueueRec " + resource + " :" + e.Message);
                        PerfLog.Enter("Method: GetNextQueueRec, Parameters: Resource - " + resource + ", MaxQtyToProcess - " + numRecsToReturn + ", Exception:" + e.Message);
                        PerfLog.Exit();
                    #endif
                }

                return retRec;
            }
        }

        public static List<QueueRecord> GetProcessingRecords(string currentPost)
        {
            #if _PERFORMANCE_LOG_
                PerfLog.Enter();
            #endif

            using (var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService"))
            {
                try
                {
                    List<QueueRecord> retRecs = null;
                    try
                    {
                        #if _PERFORMANCE_LOG_
                            PerfLog.Enter("Line92-");
                            PerfLog.Exit();
                        #endif

                        //var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService");
                        retRecs = myProxy.GetQueueRecords(0, "ALL", "PROCESSING", currentPost);
                    }
                    catch (Exception e)
                    {
                        myProxy.Abort();
                        #if _PERFORMANCE_LOG_
                            PerfLog.Enter("Line101-");
                            PerfLog.Exit();
                        #endif

                        Logger.Warn("Exception is going to be IGNORED.", e);
                    }

                    return retRecs;
                }
                catch (Exception e)
                {
                    myProxy.Abort();
                    #if _PERFORMANCE_LOG_
                        PerfLog.Enter("Line102-");
                        PerfLog.Exit();
                    #endif

                    Logger.Error("Unexpected Exception.", e);
                    throw;
                }
                finally
                {
                    #if _PERFORMANCE_LOG_
                        PerfLog.Exit();
                    #endif
                }
            }
        }

        public static string ResetQueueRecord(int id)
        {
            #if _PERFORMANCE_LOG_
                PerfLog.Enter(id);
                PerfLog.Exit();
            #endif
            using (var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService"))
            {
                try
                {
                    string retVal = "ok";
                    try
                    {
                        //var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService");
                        retVal = myProxy.ResetQueueRecord(id);
                    }
                    catch (Exception e)
                    {
                        myProxy.Abort();
                        Logger.Warn("Exception is going to be IGNORED.", e);
                    }

                    return retVal;
                }
                catch (Exception e)
                {
                    myProxy.Abort();
                    Logger.Error("Unexpected Exception.", e);
                    throw;
                }
                #if _PERFORMANCE_LOG_
                finally
                {
                    PerfLog.Exit();
                }
                #endif
            }
        }

        public static string StartQueueRecord(int id)
        {
            string retVal = "ok";
            using (var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService"))
            {
                try
                {
                    //var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService");
                    retVal = myProxy.StartQueueRecord(id);
                    #if _PERFORMANCE_LOG_
                        PerfLog.Enter("StartQueueRecord for ID# " + id + "result = " + retVal);
                    #endif
                }
                catch (Exception e)
                {
                    myProxy.Abort();
                    #if _PERFORMANCE_LOG_
                        PerfLog.Enter("StartQueueRecord Exception ignored for ID# " + id + ": " + e.Message);
                        PerfLog.Exit();
                    #endif
                }

                return retVal;
            }

        }

        //Changed By: Sandeep Kumar
        //Changed Date: 03/26/2015
        //Change Description: Wrap the code inside using block. eCR #3075.        
        public static void ResetQueueAll()
        {
            using (var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService"))
            {
                try
                {
                    //var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService");
                    myProxy.ResetQueue(0);
                }
                catch (Exception e)
                {
                    myProxy.Abort();
                    #if _PERFORMANCE_LOG_
                        PerfLog.Enter("ResetQueueAll Exception ignored: " + e.Message);
                        PerfLog.Exit();
                    #endif
                }
            }
        }

        public static void CompleteQueueRecord(int id, string resultStatus)
        {
            //Changed By: Sandeep Kumar
            //Changed Date: 03/26/2015
            //Change Description: Wrap the code inside using block. eCR #3075.
            using (var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService"))
            {
                try
                {
                    //var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService");
                    string result = myProxy.FinishQueueRecord(id, resultStatus);

                    #if _PERFORMANCE_LOG_
                        PerfLog.Enter("CompleteQueueRecord result ID# " + id + ": " + result);
                        PerfLog.Exit();
                    #endif
                }
                catch (Exception e)
                {
                    myProxy.Abort();
                    #if _PERFORMANCE_LOG_
                        PerfLog.Enter("CompleteQueueRecord Exception ignored for ID# " + id + ": " + e.Message);
                        PerfLog.Exit();
                    #endif
                }
            }
        }

        public static void UpdateLibraryResults(QueueRecord updateData)
        {
            //Changed By: Sandeep Kumar
            //Changed Date: 03/26/2015
            //Change Description: Wrap the code inside using block. eCR #3075.
            using (var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService"))
            {
                try
                {
                    //var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService");
                    string result = myProxy.InsertLibraryResult(updateData);

                    #if _PERFORMANCE_LOG_
                        PerfLog.Enter("UpdateLibraryResults result ID# " + updateData.ID + ": " + result);
                        PerfLog.Exit();
                    #endif
                }
                catch (Exception ex)
                {
                    #if _PERFORMANCE_LOG_
                        PerfLog.Enter("UpdateLibraryResults failed- " + updateData.ID + "-" + ex.Message);
                        PerfLog.Exit();
                    #endif
                }
            }
        }


        /// <summary>
        ///     Main processing if a queue record is returned.  This processing will direct
        ///     Starting the queue record
        ///     Getting the DXF
        ///     Placing DXF into correct path
        ///     CAlling the library workcenter to process
        ///     Checking for a returned value
        ///     finishing the queue record
        ///     Updating Partinfo service with result
        /// </summary>
        /// <param name="theState"></param>
        /// <returns></returns>
        public static void ProcessFile(string activePost, int syncPost)
        {
            string optimationBasePath;
            string radanBasePath = ConfigurationManager.AppSettings["RadanBasePath"];
            int libraryTimeoutSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["LibraryTimeoutSeconds"]);
            string DestPathName = "";
            string sourceDxfPath = "";
            string launchPath = "";
            bool working = true;

            if (ConfigurationManager.AppSettings["debugmode"].ToUpper() == "TRUE")
                optimationBasePath = ConfigurationManager.AppSettings["LibraryBasePath"];
            else
                optimationBasePath = ConfigurationManager.AppSettings["ServerBasePath"];

            ModelQueue.LogToOracle(0, "ProcessFile", activePost,  " process starting", null, false);

            while (working)
            {
                List<QueueRecord> recsReturned = ModelQueue.GetNextQueueRec(activePost, Convert.ToInt32(ConfigurationManager.AppSettings["MaxQtyToProcess"]));
                if (recsReturned.Count > 0)
                {
                    //Changed By: Sandeep Kumar
                    //Changed Date: 03/26/2015
                    //Change Description: Added log information. eCR #3075.
                    #if _PERFORMANCE_LOG_                    
                    PerfLog.Enter("Total queue records returned from nestlibrary_queue" + recsReturned.Count.ToString());
                    PerfLog.Exit();
                    #endif

                    ModelQueue.LogToOracle(0, "ProcessFile", activePost, recsReturned.Count.ToString() + " queue records returned from nestlibrary_queue", null, false);
                    ModelQueue.ClearLibraryLocks(recsReturned[0]);
                    ModelQueue.LogToOracle(0, "ProcessFile", activePost, "Locks cleared", null, false);

                    for (int i = 0; i < recsReturned.Count; i++)
                    {
                        QueueRecord theRecord = recsReturned[i];

                        try
                        {
                            StartQueueRecord(theRecord.ID);
                            ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, " record started", null, false);
                            GetBasePart(ref theRecord);
                            ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "base part returned", null, false);
                            ParseRecordPath(ref theRecord);
                            ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "version=" + theRecord.VERSION + "  post=" + theRecord.POST, null, false);

                            if (theRecord.ACTIONNAME == "SYNCH" || theRecord.MATERIALCODE == "SYNCH")
                            {
                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "Locking post for Sync record", null, false);
                                HandleZLocks(theRecord, "LOCK");
                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "Syncing post", null, false);
                                SyncPost(theRecord);
                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "UNLocking post for Sync record", null, false);
                                HandleZLocks(theRecord, "UNLOCK");
                            }
                            else if (theRecord.ACTIONNAME == "SYNC1" || theRecord.MATERIALCODE == "SYNC1")
                            {
                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "Locking post for Sync1 part:" + theRecord.PARTNUMBER, null, false);
                                HandleZLocks(theRecord, "LOCK");
                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "Syncing part:" + theRecord.PARTNUMBER, null, false);
                                SyncPart(ConfigurationManager.AppSettings["LibraryBasePath"] + theRecord.LIBRARYPATH.Replace('/', '\\'), theRecord);
                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "UNLocking post for Sync1 part:" + theRecord.PARTNUMBER, null, false);
                                HandleZLocks(theRecord, "UNLOCK");
                            }
                            else if (theRecord.VERSION == "RADAN")
                            {
                                DestPathName = radanBasePath + theRecord.LIBRARYPATH + @"\" + theRecord.PARTNUMBER + ".DXF";
                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "Getting DXF file from " + DestPathName, null, false);
                                sourceDxfPath = GetDXFFile(ref theRecord, DestPathName);
                                theRecord.RESULT_STATUS = "SUCCESS";
                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "result_status = SUCCESS", null, false);
                            }
                            else //nest type = OPTIMATION
                            {
                                RemoveCopperSuffix(ref theRecord);
                                DestPathName = optimationBasePath + "optimation\\" + theRecord.VERSION + "\\" + theRecord.POST + @"\parts2do\" + theRecord.BASEPART + ".DXF";
                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "Getting DXF file from " + DestPathName, null, false);
                                sourceDxfPath = GetDXFFile(ref theRecord, DestPathName);
                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "DXF file found:" + sourceDxfPath, null, false);
                                //UpdateDXFLayers(ref theRecord, sourceDxfPath, theState.StateLayers); NO LONGER NEEDED
                                CreateStandardPPIFile(DestPathName.Replace(".DXF", ".PPI"), theRecord);
                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "PPI file created:" + DestPathName.Replace(".DXF", ".PPI"), null, false);

                                if (ConfigurationManager.AppSettings["useZDrive"].ToUpper() == "TRUE")
                                    launchPath = optimationBasePath + @"optimation\" + theRecord.VERSION + @"\utils\library_" + theRecord.POST + ".bat";
                                else
                                    launchPath = optimationBasePath + @"optimation\" + theRecord.VERSION + @"\utils\library_" + theRecord.POST + "local.bat";

                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "Launching:" + launchPath, null, false);
                                LaunchAndWait(launchPath, ref theRecord, libraryTimeoutSeconds);

                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "Reading result from:" + optimationBasePath + theRecord.LIBRARYPATH, null, false); ReadLibraryResult(optimationBasePath + theRecord.LIBRARYPATH, ref theRecord, Convert.ToBoolean(syncPost));
                                ReadLibraryResult(optimationBasePath + theRecord.LIBRARYPATH, ref theRecord, Convert.ToBoolean(syncPost));

                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "Unlocking post:" + theRecord.LIBRARYPATH, null, false);
                                HandleZLocks(theRecord, "UNLOCK");
                            }

                            ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "completing:" + theRecord.RESULT_STATUS);
                            UpdateLibraryResults(theRecord);
                            CompleteQueueRecord(theRecord.ID, theRecord.RESULT_STATUS);
                            ModelQueue.VerifyQueueRecordComplete(theRecord.ID);

                            #if _PERFORMANCE_LOG_
                                PerfLog.Enter("ProcessComplete: " + theRecord.ID + "---" + theRecord.RESULT_STATUS);
                                PerfLog.Exit();
                            #endif
                        }
                        catch (Exception e)
                        {
                            if (theRecord.RESULT_STATUS.Length < 1)
                            {
                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFileException", activePost, e.Message);
                                theRecord.RESULT_STATUS = e.Message.Substring(0, 10);
                            }

                            if (theRecord.VERSION != "RADAN" && e.GetType().Name != "InvalidPathException" && e.GetType().Name != "DestinationPathException")
                            {
                                ModelQueue.LogToOracle(theRecord.ID, "ProcessFileException", activePost, e.Message);                                
								try
								{
                                	HandleZLocks(theRecord, "UNLOCK");
                                	ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "Cleaning up files:" + optimationBasePath + @"Optimation\");
                                	CleanUp(optimationBasePath + @"Optimation\", theRecord);
								}
								catch{}
                            }

                            #if _PERFORMANCE_LOG_
                                PerfLog.Enter("ProcessFileException: " + theRecord.ID + "---" + theRecord.RESULT_STATUS + ": " + e.Message);
                                PerfLog.Exit();
                            #endif

                            ModelQueue.LogToOracle(theRecord.ID, "ProcessFile", activePost, "completing:" + theRecord.RESULT_STATUS);
                            UpdateLibraryResults(theRecord);
                            CompleteQueueRecord(theRecord.ID, theRecord.RESULT_STATUS);
                            ModelQueue.VerifyQueueRecordComplete(theRecord.ID);

                            #if _PERFORMANCE_LOG_
                                PerfLog.Enter("ProcessComplete: " + theRecord.ID + "---" + theRecord.RESULT_STATUS);
                                PerfLog.Exit();
                            #endif

                            continue;
                        }

                        //Thread.Sleep(30000);
                    }                    
                }

                Thread.Sleep(60000);
            }
        }

        public static void ParseRecordPath(ref QueueRecord thisRecord)
        {
            //TODO: THIS IS A BUG!!! Nest_Version in resources directory is not consistent; should be split into separate fields
            //notice that sometimes theRecord.VERSION == "" and OPTIMATION is not always at index 0

            //get the post and version from the library path
            thisRecord.LIBRARYPATH = thisRecord.LIBRARYPATH.Replace(@"\", @"\/");
            string[] libraryFull = thisRecord.LIBRARYPATH.Split('/');

            if (libraryFull.Length > 0)
            {
                if (libraryFull[0].ToUpper() == "OPTIMATION")
                {
                    thisRecord.POST = libraryFull[2];
                    thisRecord.VERSION = libraryFull[1];
                }
                else
                {
                    thisRecord.VERSION = "RADAN";
                    thisRecord.POST = "RADAN";
                }
            }
            else // no path found
            {
                thisRecord.RESULT_STATUS = "INVALID_PATH";
                throw new InvalidPathException();
            }
        }

        /// <summary>
        /// Searchs and Replaces config Layer Names with new Layer Names
        /// </summary>
        /// <param name="filename">file to search, replace and update</param>
        /// <param name="layersList">the list of layers to change from to</param>
        public static void UpdateDXFLayers(ref QueueRecord thisRecord,  string filename, List<DxfLayers> layersList)
        {
            try
            {
                string text = File.ReadAllText(filename);

                foreach (DxfLayers layer in layersList)
                {
                    text = text.Replace(layer.LayerIn,layer.LayerOut);
                }
                File.WriteAllText(filename, text);
            }
            catch (Exception e)
            {
                thisRecord.RESULT_STATUS = "DXFLAYER_EXCEPTION";
                throw new DxfLayerException(filename + ": " + e.Message);
            } 
        }

        public static void ProcessSynchRequest(QueueRecord theSynchRec, string OptimationBasePath)
        {
            theSynchRec.RESULT_STATUS = "STARTED";
            
            string launchPath = OptimationBasePath + theSynchRec.LIBRARYPATH.Replace('/', '\\') + @"\work\" + theSynchRec.WORKCENTER + "SYNCH.bat";
            string SynchTimeoutSeconds = ConfigurationManager.AppSettings["SynchTimeoutSeconds"];
            LaunchAndWait(launchPath, ref theSynchRec, Convert.ToInt32(SynchTimeoutSeconds));
            if (theSynchRec.RESULT_STATUS == "STARTED")
                theSynchRec.RESULT_STATUS = "FINISHED";
        }


        /// <summary>
        /// tested for exception handling
        /// </summary>
        /// <param name="thisRecord"></param>
        public static void GetBasePart(ref QueueRecord thisRecord)
        {
            if (thisRecord.PARTNUMBER != null)
            {
                string fullpart = thisRecord.PARTNUMBER;
                //TODO: read variant flag instead of using dash to determine if variant
                if (fullpart.IndexOf('-') > 0)
                    thisRecord.BASEPART = fullpart.Substring(0, fullpart.IndexOf('-'));
                else
                    thisRecord.BASEPART = fullpart;
            }
            else
                throw new InvalidPartException();
        }


        /// <summary>
        ///     checks to see if file is found in partsdon or partserr
        /// </summary>
        /// <param name="libraryPath">base path to library area</param>
        /// <param name="thisRecord">part name</param>
        /// <returns>SUCCESS or error string</returns>
        public static void ReadLibraryResult(string libraryPath, ref QueueRecord thisRecord, bool syncPart)
        {
            string cleanedPath = libraryPath.Replace('/', '\\');
            Logger.Error(cleanedPath + @"\partsdon\" + thisRecord.BASEPART + ".DXF");

            if (File.Exists(cleanedPath + @"\partsdon\" + thisRecord.BASEPART + ".DXF"))
            {
                thisRecord.RESULT_STATUS = "SUCCESS";
                if (syncPart)
                {
                    SyncPart(cleanedPath, thisRecord);
                }
                
                ExtractCostInfo(cleanedPath, ref thisRecord);
            }
            else if (File.Exists(cleanedPath + @"\partserr\" + thisRecord.BASEPART + ".DXF"))
            {
                thisRecord.RESULT_STATUS = "ERROR";
                ExtractErrorInfo(cleanedPath, ref thisRecord);
            }
            else
                thisRecord.RESULT_STATUS = "NOT_FND";
        }

        private static void SyncPost(QueueRecord currentRecord)
        {
            string adjustedResourcePath = currentRecord.LIBRARYPATH.ToLower().Replace(@"/", @"\");
            string adjustedOrgServer = ConfigurationManager.AppSettings["NestingBasePath"] + ConfigurationManager.AppSettings["db"] + @"\" + adjustedResourcePath.Replace(@"optimation\", "") + @"\work\";
            string adjustedOrgServer2 = adjustedOrgServer.Insert(adjustedOrgServer.ToLower().IndexOf(@"\powell_90"), "2");
            string adjustedOrgServer3 = adjustedOrgServer.Insert(adjustedOrgServer.ToLower().IndexOf(@"\powell_90"), "3");
            string adjustedLibraryPath = ConfigurationManager.AppSettings["LibraryBasePath"] + @"optimation\" + adjustedResourcePath.Replace(@"optimation\", "");

            switch (currentRecord.ORGID)
            {
                case 90:
                    adjustedOrgServer = adjustedOrgServer.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["DLTServer"]);
                    adjustedOrgServer2 = adjustedOrgServer2.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["DLTServer"]);
                    adjustedOrgServer3 = adjustedOrgServer3.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["DLTServer"]);
                    break;
                case 92:
                    adjustedOrgServer = adjustedOrgServer.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["NCDServer"]);
                    adjustedOrgServer2 = adjustedOrgServer2.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["NCDServer"]);
                    adjustedOrgServer3 = adjustedOrgServer3.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["NCDServer"]);
                    break;
                case 325:
                    adjustedOrgServer = adjustedOrgServer.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["EDEServer"]);
                    adjustedOrgServer2 = adjustedOrgServer2.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["EDEServer"]);
                    adjustedOrgServer3 = adjustedOrgServer3.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["EDEServer"]);
                    break;
                default:
                    break;
            }

            try
            {
                //copy the geo files from the library server to the nesting server -- ie Z to B drive
                DirectoryInfo di = new DirectoryInfo(adjustedLibraryPath + "\\work");
                DirectoryInfo[] subdirs = di.GetDirectories();
                foreach (DirectoryInfo subdir in subdirs)
                {
                    if (subdir.Name.StartsWith("geo") || subdir.Name.StartsWith("mach"))
                    {
                        string destinationDir = adjustedOrgServer + subdir.Name;
                        CopyDirectory(subdir.FullName, destinationDir);           
                    }
                }
                //back up the old master file from the nesting server to the library server, but as a txt file
                File.Copy(adjustedOrgServer.ToLower() + "PEQMASTR.DAT", adjustedLibraryPath + @"\work\PEQMASTR.txt", true);
                //copy the new master file from the library server to the nesting server
                File.Copy(adjustedLibraryPath + @"\work\PEQMASTR.DAT", adjustedOrgServer.ToLower() + "PEQMASTR.DAT", true);
                //back up the old PEQFILNM file from the nesting server to the library server as a txt file
                File.Copy(adjustedOrgServer.ToLower() + "PEQFILNM.DAT", adjustedLibraryPath + @"\work\PEQFILNM.txt", true);
                UpdatePEQFILNM(adjustedOrgServer.ToLower() + "PEQFILNM.DAT", adjustedLibraryPath + @"\work\PEQFILNM.DAT");
                //just for the twins, sync to 2nd prod nesting server also
                if (currentRecord.IS_TWIN > 0)
                {
                    UpdatePEQFILNM(adjustedOrgServer2 + "PEQFILNM.DAT", adjustedLibraryPath + @"\work\PEQFILNM.DAT");
                    UpdatePEQFILNM(adjustedOrgServer3 + "PEQFILNM.DAT", adjustedLibraryPath + @"\work\PEQFILNM.DAT");
                }

                if (currentRecord.MATERIALCODE.IndexOf("SYNC") >= 0)
                {
                    currentRecord.RESULT_STATUS = "FINISHED";
                }
            }
            catch (Exception ex)
            {
                currentRecord.RESULT_STATUS = "SYNC_FAILED";
                throw new InvalidSyncException(adjustedOrgServer + " SYNC failed: " + ex.Message);
            }
        }

        private static void SyncPart(string postPath, QueueRecord currentRecord)
        {
            string MASTERfileName = postPath + @"\work\PEQMASTR.DAT";
            string FILNMfile = postPath + @"\work\PEQFILNM.DAT";
            string PEFfileName = "";
            string GEOfolderName = "";
            string PEQfileName = "";

            try
            {
                string[] masterLines = File.ReadAllLines(MASTERfileName);
                for (int i = masterLines.Length - 2; i > 0; i--)
                {
                    int indexAfterPartNumber = masterLines[i].IndexOf(" ", 11);
                    string currentPart = masterLines[i].Substring(11, indexAfterPartNumber - 11);
                    if (String.Compare(currentPart, currentRecord.PARTNUMBER) < 0)
                    {
                        PEFfileName = masterLines[i].Substring(indexAfterPartNumber).Trim() + ".DAT";
                        break;
                    }
                }
                ModelQueue.LogToOracle(currentRecord.ID, "SyncPart", currentRecord.POST, "PEFfileName:" + PEFfileName, null, false);

                if (PEFfileName.Length > 0)
                {
                    string[] pefFileLines = File.ReadAllLines(postPath + @"\work\geo1\" + PEFfileName);
                    for (int y = 0; y < pefFileLines.Length; y++)
                    {
                        if (pefFileLines[y].StartsWith("*" + currentRecord.PARTNUMBER))
                        {
                            PEQfileName = "PEQ" + pefFileLines[y].Replace("*" + currentRecord.PARTNUMBER, "").TrimStart(' ').Substring(0, 5) + ".DAT";
                            break;
                        }
                    }
                    
                    if(PEQfileName.Length < 1)
                    {
                        ModelQueue.LogToOracle(currentRecord.ID, "SyncPart", currentRecord.POST, "PEQfileName:" + PEQfileName, null, false);
                        currentRecord.RESULT_STATUS = "PEQ_NOTFND";
                        throw new InvalidSyncException(postPath + @"\work\" + @"\" + GEOfolderName + @"\" + PEQfileName + " SYNC failed");
                    }

                    GEOfolderName = FindGeoFolder(postPath, PEQfileName);
                    ModelQueue.LogToOracle(currentRecord.ID, "SyncPart", currentRecord.POST, "GEOfolderName:" + GEOfolderName, null, false);
                    if (GEOfolderName.Length < 1)
                    {
                        currentRecord.RESULT_STATUS = "BAD_GEOMAP";
                        throw new GEOMappingException("Invalid GEO mapping ID#" + currentRecord.ID);
                    }
                    else
                    {
                        try
                        {
                            CopySyncFiles(postPath, GEOfolderName, PEFfileName, PEQfileName, currentRecord);                            
                        }
                        catch(Exception ex)
                        {
                            currentRecord.RESULT_STATUS = "SYNC_FAILED";
                            throw new InvalidSyncException(postPath + @"\work\" + @"\" + GEOfolderName + @"\" + PEFfileName + " SYNC failed" + ex.Message);
                        }                                       
                    }

                    if (currentRecord.MATERIALCODE.IndexOf("SYNC") >= 0)
                    {
                        currentRecord.RESULT_STATUS = "FINISHED";
                    }
                }
                else
                {
                    currentRecord.RESULT_STATUS = "BAD_PEFMAP";
                    throw new PEFMappingException("Invalid PEF mapping ID#" + currentRecord.ID);
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name == "PEFMappingException" || ex.GetType().Name == "GEOMappingException" || ex.GetType().Name == "InvalidSyncException")
                    throw;
                else
                {
                    currentRecord.RESULT_STATUS = "SYNC_ERROR";
                    throw new AutoSyncException(currentRecord.LIBRARYPATH + ": " + ex.Message);
                }
            }            
        }

        private static void CopyDirectory(string sourcePath, string destinationpath)
        {
            //Changed By: Sandeep Kumar
            //Changed Date: 03/26/2015
            //Change Description: Wrap the code inside using block. eCR #3075.
            using (var myProcess = new Process())
            {
                //var myProcess = new Process();
                myProcess.StartInfo.FileName = "robocopy";
                myProcess.StartInfo.Arguments = sourcePath + " " + destinationpath + " " + @"/w:1 /r:1"; // /c /r /k /y";
                myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                myProcess.StartInfo.CreateNoWindow = false;
                myProcess.Start();
                myProcess.WaitForExit();
            }
        }
        private static void CopySyncFiles(string batPath, string geoFolder, string pefFile, string peqFile, QueueRecord thisRecord)
        {
            string adjustedResourcePath = thisRecord.LIBRARYPATH.ToLower().Replace(@"optimation/", "").Replace(@"/", @"\");
            string adjustedOrgServer = ConfigurationManager.AppSettings["NestingBasePath"] + ConfigurationManager.AppSettings["db"] + @"\" + adjustedResourcePath + @"\work\" + geoFolder + @"\";
            string adjustedOrgServer2 = adjustedOrgServer.Insert(adjustedOrgServer.ToLower().IndexOf(@"\powell_90"), "2");
            string adjustedOrgServer3 = adjustedOrgServer.Insert(adjustedOrgServer.ToLower().IndexOf(@"\powell_90"), "3");
            string pefPath = batPath + @"\work\geo1\" + pefFile;
            string peqPath = batPath + @"\work\" + geoFolder + @"\" + peqFile;
            bool success = true;

            switch (thisRecord.ORGID)
            {
                case 90:
                    adjustedOrgServer = adjustedOrgServer.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["DLTServer"]);
                    adjustedOrgServer2 = adjustedOrgServer2.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["DLTServer"]);
                    adjustedOrgServer3 = adjustedOrgServer3.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["DLTServer"]);
                    break;
                case 92:
                    adjustedOrgServer = adjustedOrgServer.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["NCDServer"]);
                    adjustedOrgServer2 = adjustedOrgServer2.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["NCDServer"]);
                    adjustedOrgServer3 = adjustedOrgServer3.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["NCDServer"]);
                    break;
                case 325:
                    adjustedOrgServer = adjustedOrgServer.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["EDEServer"]);
                    adjustedOrgServer2 = adjustedOrgServer2.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["EDEServer"]);
                    adjustedOrgServer3 = adjustedOrgServer3.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["EDEServer"]);
                    break;
                default:
                    break;
            }

            if (File.Exists(pefPath))
            {
                //copy the PEF file from the library server to the nesting server -- ie Z to B drive
                ModelQueue.LogToOracle(thisRecord.ID, "CopySyncFiles", batPath, "origPefPath= " + pefPath + "; destPefPath= " + adjustedOrgServer.Substring(0, adjustedOrgServer.IndexOf("geo")) + "geo1\\" + pefFile);
                File.Copy(pefPath, adjustedOrgServer.Substring(0, adjustedOrgServer.IndexOf("geo")) + "geo1\\" + pefFile, true);
                if (File.GetLastWriteTime(pefPath) != File.GetLastWriteTime(adjustedOrgServer.Substring(0, adjustedOrgServer.IndexOf("geo")) + "geo1\\" + pefFile))
                {
                    success = false;
                    ModelQueue.LogToOracle(thisRecord.ID, "CopySyncFiles", thisRecord.POST, "Copy failed for:" + pefPath, null, true);
                }
                //copy the PEQ file from the library server to the nesting server -- ie Z to B drive
                ModelQueue.LogToOracle(thisRecord.ID, "CopySyncFiles", batPath, "origPeQPath= " + peqPath + "; destPeqPath= " + adjustedOrgServer + peqFile);
                File.Copy(peqPath, adjustedOrgServer + peqFile, true);
                if (File.GetLastWriteTime(peqPath) != File.GetLastWriteTime(adjustedOrgServer + peqFile))
                {
                    success = false;
                    ModelQueue.LogToOracle(thisRecord.ID, "CopySyncFiles", thisRecord.POST, "Copy failed for:" + peqPath, null, true);
                }
                //back up the old master file from the nesting server to the library server, but as a txt file
                File.Copy(adjustedOrgServer.ToLower().Remove(adjustedOrgServer.IndexOf("geo")) + "PEQMASTR.DAT", pefPath.ToLower().Remove(pefPath.IndexOf("geo")) + "PEQMASTR.txt", true);
                //copy the new master file from the library server to the nesting server
                File.Copy(pefPath.ToLower().Remove(pefPath.IndexOf("geo")) + "PEQMASTR.DAT", adjustedOrgServer.ToLower().Remove(adjustedOrgServer.IndexOf("geo")) + "PEQMASTR.DAT", true);
                //back up the old PEQFILNM file from the nesting server to the library server as a txt file
                File.Copy(adjustedOrgServer.ToLower().Remove(adjustedOrgServer.IndexOf("geo")) + "PEQFILNM.DAT", pefPath.ToLower().Remove(pefPath.IndexOf("geo")) + "PEQFILNM.txt", true);
                UpdatePEQFILNM(adjustedOrgServer.ToLower().Remove(adjustedOrgServer.IndexOf("geo")) + "PEQFILNM.DAT", pefPath.ToLower().Remove(pefPath.IndexOf("geo")) + "PEQFILNM.DAT");
                //just for the twins, sync to 2nd prod nesting server also
                if (thisRecord.IS_TWIN > 0)
                {
                    ModelQueue.LogToOracle(thisRecord.ID, "CopySyncFiles", batPath, "origPef2Path= " + pefPath + "; destPef2Path= " + adjustedOrgServer2.Substring(0, adjustedOrgServer2.IndexOf("geo")) + " geo1\\" + pefFile);
                    ModelQueue.LogToOracle(thisRecord.ID, "CopySyncFiles", batPath, "origPef2Path= " + pefPath + "; destPef2Path= " + adjustedOrgServer3.Substring(0, adjustedOrgServer3.IndexOf("geo")) + " geo1\\" + pefFile);

                    File.Copy(pefPath, adjustedOrgServer2.Substring(0, adjustedOrgServer2.IndexOf("geo")) + "geo1\\" + pefFile, true);
                    if (File.GetLastWriteTime(pefPath) != File.GetLastWriteTime(adjustedOrgServer2.Substring(0, adjustedOrgServer2.IndexOf("geo")) + "geo1\\" + pefFile))
                    {
                        success = false;
                        ModelQueue.LogToOracle(thisRecord.ID, "CopySyncFiles", thisRecord.POST, "Copy failed for:" + pefPath, null, true);
                    }

                    File.Copy(pefPath, adjustedOrgServer3.Substring(0, adjustedOrgServer3.IndexOf("geo")) + "geo1\\" + pefFile, true);
                    if (File.GetLastWriteTime(pefPath) != File.GetLastWriteTime(adjustedOrgServer3.Substring(0, adjustedOrgServer3.IndexOf("geo")) + "geo1\\" + pefFile))
                    {
                        success = false;
                        ModelQueue.LogToOracle(thisRecord.ID, "CopySyncFiles", thisRecord.POST, "Copy failed for:" + pefPath, null, true);
                    }

                    ModelQueue.LogToOracle(thisRecord.ID, "CopySyncFiles", batPath, "origPeQ2Path= " + peqPath + "; destPeq2Path= " + adjustedOrgServer2 + peqFile);
                    ModelQueue.LogToOracle(thisRecord.ID, "CopySyncFiles", batPath, "origPeQ2Path= " + peqPath + "; destPeq2Path= " + adjustedOrgServer3 + peqFile);

                    File.Copy(peqPath, adjustedOrgServer2 + peqFile, true);
                    if (File.GetLastWriteTime(peqPath) != File.GetLastWriteTime(adjustedOrgServer2 + peqFile))
                    {
                        success = false;
                        ModelQueue.LogToOracle(thisRecord.ID, "CopySyncFiles", thisRecord.POST, "Copy failed for:" + peqPath, null, true);
                    }

                    File.Copy(peqPath, adjustedOrgServer3 + peqFile, true);
                    if (File.GetLastWriteTime(peqPath) != File.GetLastWriteTime(adjustedOrgServer3 + peqFile))
                    {
                        success = false;
                        ModelQueue.LogToOracle(thisRecord.ID, "CopySyncFiles", thisRecord.POST, "Copy failed for:" + peqPath, null, true);
                    }


                    UpdatePEQFILNM(adjustedOrgServer2.ToLower().Remove(adjustedOrgServer2.IndexOf("geo")) + "PEQFILNM.DAT", pefPath.ToLower().Remove(pefPath.IndexOf("geo")) + "PEQFILNM.DAT");
                    UpdatePEQFILNM(adjustedOrgServer3.ToLower().Remove(adjustedOrgServer3.IndexOf("geo")) + "PEQFILNM.DAT", pefPath.ToLower().Remove(pefPath.IndexOf("geo")) + "PEQFILNM.DAT");
                }

                if (!success)
                    throw new UnsuccessfulSyncException();
            }
            else
            {
                ModelQueue.LogToOracle(thisRecord.ID, "CopySyncFiles", "pefPath does not exist", "origPefPath= " + pefPath + "; destPefPath= " + adjustedOrgServer.Substring(0, adjustedOrgServer.IndexOf("geo")) + "geo1\\" + pefFile);
            }

        }

        private static void UpdatePEQFILNM(string nestingPEQ, string libraryPEQ)
        {
            string line1_LASTGEO;
            string line5_LASTMASTERSUB;

            //get the 2 new lines from recent libraried part that need to go into the local nesting PEQFILNM
            using (StreamReader r = new StreamReader(libraryPEQ))
            {
                line1_LASTGEO = r.ReadLine();
                for (int i = 0; i < 3; i++)
                {
                    r.ReadLine();
                }
                line5_LASTMASTERSUB = r.ReadLine();
            }
            //read the old PEQFILNM into memory
            string[] oldLines = File.ReadAllLines(nestingPEQ);
            //replace the two old lines with the two new lines
            oldLines[0] = oldLines[0].Replace(oldLines[0], line1_LASTGEO);
            oldLines[4] = oldLines[4].Replace(oldLines[4], line5_LASTMASTERSUB);
            //write the edited file from memory back out to the file
            using (StreamWriter w = new StreamWriter(nestingPEQ))
            {
                foreach (string s in oldLines)
                    w.WriteLine(s);
            }
        }

        private static string FindGeoFolder(string postPath, string PEQname)
        {
            string geoName = "";
            string AVLfile = postPath + @"\work1\AVLDISK.DAT";
            var allLines = File.ReadLines(AVLfile);
            List<string> geoLines = new List<string>(allLines).FindAll(g => g.IndexOf("geo") > 0);
            foreach (string s in geoLines)
            {
                string bookends = s.Substring(s.LastIndexOf('/') + 1).TrimStart();
                string firstBook = bookends.Substring(0, bookends.IndexOf(' '));
                string lastBook = bookends.Substring(bookends.IndexOf(' ') + 1).Trim();
                if (String.Compare(PEQname, lastBook) < 0)
                {
                    geoName = s.Substring(s.IndexOf("geo"), 4);
                    break;
                }
            }

            return geoName;
        }

        private static void ExtractCostInfo(string postPath, ref QueueRecord thisRecord)
        {
            string costFilePath = postPath + @"\cost\";
            try
            {
                string[] fileArray = Directory.GetFiles(costFilePath, thisRecord.PARTNUMBER + "*");
                foreach (string costFile in fileArray)
                {
                    List<string> lines = new List<string>(File.ReadLines(costFile));
                    thisRecord.HITS = ParseLineForCostValue("Hits", lines);
                    thisRecord.PIERCES = ParseLineForCostValue("Pierces", lines);
                    thisRecord.TOOLCHANGES = ParseLineForCostValue("Tool Changes", lines);
                    thisRecord.TOOLPATH = ParseLineForCostValue("Tool Path", lines);
                }
            }
            catch (Exception ex)
            {               
                //throw;
                #if _PERFORMANCE_LOG_                
                    PerfLog.Enter("Method: ExtractCostInfo, Parameters: PostPath - " + postPath + ", Exception:" + ex.Message);
                    PerfLog.Exit();
                #endif
            }
        }

        private static int ParseLineForCostValue(string costItem, List<string> fileLines)
        {
            int i = 0;

            try
            {
                string foundLine = fileLines.Find(l => l.StartsWith(costItem));

                if (foundLine != null)
                {
                    i = (int?)(Convert.ToInt32(foundLine.Substring(foundLine.IndexOf("=")).Trim())) ?? 0;
                }
            }
            catch { }
            return i;
        }

        private static void ExtractErrorInfo(string postPath, ref QueueRecord thisRecord)
        {
            string errorFilesPath = postPath + @"\work1\Inputerrors\";
            string formattedFinishTime = "err_comp_";

            try
            {
                formattedFinishTime += Directory.GetLastWriteTime(errorFilesPath).ToString("MM-dd-yy_HH-mm");

                string[] fileArray = Directory.GetFiles(errorFilesPath, formattedFinishTime + "*");
                foreach (string datFile in fileArray)
                {
                    string[] linesArray = File.ReadAllLines(datFile);
                    string fileContent = File.ReadAllText(datFile).Replace(" ", "");
                    thisRecord.ERRORLOG = fileContent.Length <= 499 ? fileContent : fileContent.Substring(0, 499);
                }
            }
            catch(Exception e)
            {
                thisRecord.ERRORLOG = "error file not found";
                #if _PERFORMANCE_LOG_                
                    PerfLog.Enter("Method: ExtractErrorInfo, Parameters: postPath - " + postPath + ", Exception:" + e.Message);
                    PerfLog.Exit();
                #endif
            } 
        }

        /// <summary>
        ///     Launch Applicaiton and wait for that process to end
        /// </summary>
        /// <param name="filetolaunch">
        ///     Application file to launch full path
        /// </param>
        /// <param name="parameters">
        ///     Any parameters to call the applicaiton with ie.  notepad c:\powell\help.txt where c:\powell\help.txt would be the parameter
        /// </param>
        /// <param name="iTimeOut">
        ///     How long in seconds to wait for the applicaiton to end before stopping the application
        /// </param>
        /// <returns>Returns "" if no errors otherwise a error description is returned</returns>
        private static void LaunchAndWait(string filetolaunch, ref QueueRecord thisRecord, int iTimeOut)
        {
            iTimeOut = iTimeOut * 1000;
            bool myResult = false;


            if (!File.Exists(filetolaunch))
            {
                ModelQueue.LogToOracle(thisRecord.ID, "LaunchAndWait", filetolaunch, "missing launch file");
                thisRecord.RESULT_STATUS = "INVALID_OPTIBAT";
                throw new InvalidOptimationBatFileException(filetolaunch);
            }
            else
            {
                try
                {
                    ModelQueue.LogToOracle(thisRecord.ID, "LaunchAndWait", filetolaunch, "locking post");
                    HandleZLocks(thisRecord, "LOCK");
                    var myProcess = new Process();
                    myProcess.StartInfo.FileName = filetolaunch;
                    myProcess.StartInfo.Arguments = thisRecord.ID.ToString();
                    myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    myProcess.StartInfo.CreateNoWindow = false;
                    Thread t1 = new Thread(() => StartProcessLaunch(ref  myProcess, iTimeOut, out  myResult));
                    t1.Start();
                    t1.Join();
                    if (!myResult)
                    {
                        ModelQueue.LogToOracle(thisRecord.ID, "LaunchAndWait", filetolaunch, "TIME OUT EXCEPTION");
                        thisRecord.RESULT_STATUS = "TIME_OUT";
                        myProcess.Kill();
                        #if _PERFORMANCE_LOG_
                            PerfLog.Enter(thisRecord.ID + ": " + thisRecord.RESULT_STATUS + " Launchfile=" + filetolaunch);
                            PerfLog.Exit();
                        #endif
                        throw new OptimationTimeoutException(thisRecord.ID + " timeout");                      

                    }
                    ModelQueue.LogToOracle(thisRecord.ID, "LaunchAndWait", filetolaunch, "Optimation launch concluded");
                }
                catch (Exception ex)
                {
                    string errType = ex.GetType().Name;
                    if (errType != "OptimationTimeoutException")
                    {
                        thisRecord.RESULT_STATUS = "LAUNCH_ERROR";
                        ModelQueue.LogToOracle(thisRecord.ID, "LaunchAndWait", thisRecord.PARTNUMBER, "Optimation Launch Exception: " + ex.Message);
                        throw new LaunchProcessException(ex.Message);
                    }
                    else
                    {
                        ModelQueue.LogToOracle(thisRecord.ID, "LaunchAndWait", thisRecord.PARTNUMBER, "Optimation TimeOut: " + ex.Message);
                    }
                }
            }
        }

        public static void ClearLibraryLocks(QueueRecord theRec)
        {
            try
            {
                ModelQueue.HandleZLocks(theRec, "UNLOCK");
            }
            catch (Exception ex)
            {
                #if _PERFORMANCE_LOG_                
                PerfLog.Enter("Method: ClearLibraryLocks, Exception:" + ex.Message);
                PerfLog.Exit();
                #endif
            }
        }

        private static void HandleZLocks(QueueRecord currentRecord, string lockStatus)
        {
            string adjustedResourcePath = currentRecord.LIBRARYPATH.ToLower().Replace(@"optimation/", "").Replace(@"/", @"\");
            string adjustedNestServer = ConfigurationManager.AppSettings["NestingBasePath"] + ConfigurationManager.AppSettings["db"] + @"\" + adjustedResourcePath + @"\work\";
            string adjustedNestServer2 = ConfigurationManager.AppSettings["NestingBasePath"] + ConfigurationManager.AppSettings["db"] + @"2\" + adjustedResourcePath + @"\work\";
            string adjustedNestServer3 = ConfigurationManager.AppSettings["NestingBasePath"] + ConfigurationManager.AppSettings["db"] + @"3\" + adjustedResourcePath + @"\work\";
            string adjustedLibraryServer = ConfigurationManager.AppSettings["LibraryBasePath"] + @"optimation\" + adjustedResourcePath + @"\work\";

            switch (currentRecord.ORGID)
            {
                case 90:
                    adjustedNestServer = adjustedNestServer.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["DLTServer"]);
                    adjustedNestServer2 = adjustedNestServer2.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["DLTServer"]);
                    adjustedNestServer3 = adjustedNestServer3.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["DLTServer"]);
                    break;
                case 92:
                    adjustedNestServer = adjustedNestServer.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["NCDServer"]);
                    adjustedNestServer2 = adjustedNestServer2.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["NCDServer"]);
                    adjustedNestServer3 = adjustedNestServer3.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["NCDServer"]);
                    break;
                case 325:
                    adjustedNestServer = adjustedNestServer.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["EDEServer"]);
                    adjustedNestServer2 = adjustedNestServer2.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["EDEServer"]);
                    adjustedNestServer3 = adjustedNestServer3.Replace(ConfigurationManager.AppSettings["EDHServer"], ConfigurationManager.AppSettings["EDEServer"]);
                    break;
                default:
                    break;
            }

            try
            {
                if (lockStatus == "LOCK")
                {
                    string[] libraryingLock = Directory.GetFiles(adjustedLibraryServer, "Z*"); // get any lock file on the library server post
                    string[] nestingLock = Directory.GetFiles(adjustedNestServer, "Z9*"); // get any lock file on the nesting server post
                    
                    //if it's a twin resource
                    if (currentRecord.IS_TWIN > 0)
                    {
                        ModelQueue.LogToOracle(currentRecord.ID, "HandleZLocks", "nestPath=" + adjustedNestServer + ";nestPath2=" + adjustedNestServer2 + ";nestPath3=" + adjustedNestServer3 + "; libraryPath=" + adjustedLibraryServer, "locking post", "SYNC");
                        string[] nesting2Lock = Directory.GetFiles(adjustedNestServer2, "Z9*");
                        string[] nesting3Lock = Directory.GetFiles(adjustedNestServer3, "Z9*");
                        while (nestingLock.Length > 0 || libraryingLock.Length > 0 || nesting2Lock.Length > 0 || nesting3Lock.Length > 0 )
                        {
                            Thread.Sleep(15000);
                            nestingLock = Directory.GetFiles(adjustedNestServer, "Z9*");
                            libraryingLock = Directory.GetFiles(adjustedLibraryServer, "Z*");
                            nesting2Lock = Directory.GetFiles(adjustedNestServer2, "Z9*");
                            nesting3Lock = Directory.GetFiles(adjustedNestServer3, "Z9*");
                        }
                        using(StreamWriter s = new StreamWriter(adjustedNestServer + "Z9LIBLOCK.txt"));
                        using(StreamWriter s = new StreamWriter(adjustedNestServer2 + "Z9LIBLOCK.txt"));
                        using (StreamWriter s = new StreamWriter(adjustedNestServer3 + "Z9LIBLOCK.txt")) ;
                        using (StreamWriter s = new StreamWriter(adjustedLibraryServer + "Z1LIBLOCK.txt"));
                        ModelQueue.LogToOracle(currentRecord.ID, "HandleZLocks", "nestPath=" + adjustedNestServer + "; libraryPath=" + adjustedLibraryServer, "lock completed", "SYNC");      
                    }
                    else //if it's not a twin -- ie VIPROS or FPL
                    {
                        ModelQueue.LogToOracle(currentRecord.ID, "HandleZLocks", "nestPath=" + adjustedNestServer + "; libraryPath=" + adjustedLibraryServer, "locking post","SYNC");
                        while (nestingLock.Length > 0 || libraryingLock.Length > 0)
                        {
                            Thread.Sleep(15000);
                            nestingLock = Directory.GetFiles(adjustedNestServer, "Z9*");
                            libraryingLock = Directory.GetFiles(adjustedLibraryServer, "Z*");
                        }
                        using(StreamWriter s = new StreamWriter(adjustedNestServer + "Z9LIBLOCK.txt"));
                        using(StreamWriter s = new StreamWriter(adjustedLibraryServer + "ZLIBLOCK.txt"));
                        ModelQueue.LogToOracle(currentRecord.ID, "HandleZLocks", "nestPath=" + adjustedNestServer + ";nestPath2=" + adjustedNestServer2 + ";nestPath3=" + adjustedNestServer3 + "; libraryPath=" + adjustedLibraryServer, "lock completed","SYNC");      
                    }
                }
                else //unlock post by deleting lock files
                {
                    ModelQueue.LogToOracle(currentRecord.ID, "HandleZLocks", "Unlocking " + currentRecord.LIBRARYPATH,"starting Unlocks", "SYNC");
                    string[] nestingLocks = Directory.GetFiles(adjustedNestServer, "Z*");
                    ModelQueue.LogToOracle(currentRecord.ID, "HandleZLocks", "nestPath=" + adjustedNestServer, nestingLocks.Length.ToString() + " locks found", "SYNC");
                    string[] libraryingLocks = Directory.GetFiles(adjustedLibraryServer, "Z*");
                    ModelQueue.LogToOracle(currentRecord.ID, "HandleZLocks", "libraryPath=" + adjustedLibraryServer, libraryingLocks.Length.ToString() + " locks found", "SYNC");
                    string[] nesting2Locks = new string[0];
                    string[] nesting3Locks = new string[0];

                    if (currentRecord.IS_TWIN > 0)
                    {
                        nesting2Locks = Directory.GetFiles(adjustedNestServer2, "Z9*");
                        ModelQueue.LogToOracle(currentRecord.ID, "HandleZLocks", "nestPath2=" + adjustedNestServer2, nesting2Locks + " locks found", "SYNC");

                        nesting3Locks = Directory.GetFiles(adjustedNestServer3, "Z9*");
                        ModelQueue.LogToOracle(currentRecord.ID, "HandleZLocks", "nestPath3=" + adjustedNestServer3, nesting2Locks + " locks found", "SYNC");
                    }

                    foreach (string lockFile in nestingLocks)
                    {
                        //string user = System.IO.File.GetAccessControl(lockFile).GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();
                        try
                        {
                            if (lockFile.IndexOf("LIBLOCK") > 0)
                                File.Delete(lockFile);
                        }
                        catch (Exception ex)
                        {
                            ModelQueue.LogToOracle(currentRecord.ID, "HandleZLocksException", "DELETE:" + lockFile.Substring(0, 999), ex.Message);
                        }
                    }
                    if (currentRecord.IS_TWIN > 0)
                    {
                        foreach (string lockFile in nesting2Locks)
                        {
                            try
                            {
                                if (lockFile.IndexOf("LIBLOCK") > 0)
                                    File.Delete(lockFile);
                            }
                            catch (Exception ex)
                            {
                                ModelQueue.LogToOracle(currentRecord.ID, "HandleZLocksException", "DELETE:" + lockFile.Substring(0, 999), ex.Message);
                            }
                        }

                        foreach (string lockFile3 in nesting3Locks)
                        {
                            try
                            {
                                if (lockFile3.IndexOf("LIBLOCK") > 0)
                                    File.Delete(lockFile3);
                            }
                            catch (Exception ex)
                            {
                                ModelQueue.LogToOracle(currentRecord.ID, "HandleZLocksException", "DELETE:" + lockFile3.Substring(0, 999), ex.Message);
                            }
                        }
                    }
                    foreach (string lockFile in libraryingLocks)
                    {
                        try
                        {
                            if (lockFile.IndexOf("LIBLOCK") > 0)
                                File.Delete(lockFile);
                        }
                        catch (Exception ex)
                        {
                            ModelQueue.LogToOracle(currentRecord.ID, "HandleZLocksException", "DELETE:" + lockFile.Substring(0, 999), ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelQueue.LogToOracle(currentRecord.ID, "HandleZLocksException", " ", ex.Message);
                currentRecord.RESULT_STATUS = "INVALID_PATH";
                //Changed By: Sandeep Kumar
                //Changed Date: 03/31/2015
                //Change Description: Added log information. eCR #3075.
                //throw new InvalidPathException(ex.Message);  
                #if _PERFORMANCE_LOG_
                PerfLog.Enter("HandleZLocks: " + currentRecord.ID + "---, Exception: " + ex.Message);
                PerfLog.Exit();
                #endif
            }
        }

        private static void StartProcessLaunch(ref Process currentProcess, int timeLimit, out bool result)
        {
            ModelQueue.LogToOracle(Convert.ToInt32(currentProcess.StartInfo.Arguments), "StartProcessLaunch",currentProcess.StartInfo.FileName , " Optimation process starting at: " + System.DateTime.Now.ToLongTimeString(), "SYNC" );
            result = false;
            var watch = new Stopwatch();
            watch.Start();
            currentProcess.Start();
            while (!currentProcess.HasExited && watch.ElapsedMilliseconds < timeLimit)
            {
                ModelQueue.LogToOracle(Convert.ToInt32(currentProcess.StartInfo.Arguments), "StartProcessLaunch", currentProcess.StartInfo.FileName, "Optimation sleeping" + watch.ElapsedMilliseconds.ToString() + " elapsed; " + timeLimit.ToString() + " timeout", "SYNC");
                Thread.Sleep(15000);
                if (currentProcess.HasExited)
                {
                    result = true;
                    ModelQueue.LogToOracle(Convert.ToInt32(currentProcess.StartInfo.Arguments), "StartProcessLaunch", currentProcess.StartInfo.FileName, "Optimation process has exited", "SYNC");
                }
            }
        }

        /// <summary>
        ///     cleans up all files needed when calling optimation
        ///     deletes part dxf in parts2do, ppi file in parts2do
        ///     deletes part dxf in partsdon, part dxf file in partserr
        /// </summary>
        /// <param name="theRec">partnumber and librarypath of part to cleanup</param>
        /// <param name="OptimationBasePath">base path for server</param>
        /// <returns>error message is returned, returns null if no error</returns>
        public static void CleanUp(string OptimationBasePath, QueueRecord qRec)
        {
            string version = qRec.VERSION;
            string post = qRec.POST;

            try
            {
                Array.ForEach(Directory.GetFiles(OptimationBasePath + @"\" + version + @"\" + post + @"\parts2do\"), File.Delete);
            }
            catch { }

            try
            {
                Array.ForEach(Directory.GetFiles(OptimationBasePath + @"\" + version + @"\" + post + @"\partsdon\"), File.Delete);
            }
            catch { }

            try
            {
                Array.ForEach(Directory.GetFiles(OptimationBasePath + @"\" + version + @"\" + post + @"\partserr\"), File.Delete);
            }
            catch { }
        }

        public static void CreateStandardPPIFile(string filepath, QueueRecord theRec)
        {
            //Changed By: Sandeep Kumar
            //Changed Date: 03/30/2015
            //Change Description: Wrap the code inside using block. eCR #3075.
            using (StreamWriter sr = File.CreateText(filepath))
            {
                try
                {
                    //StreamWriter sr = File.CreateText(filepath);
                    sr.WriteLine("(+PARTNO " + theRec.BASEPART + "+)");
                    sr.WriteLine("(+RAWMAT " + theRec.MATERIALCODE.ToUpper() + "+)");
                    string extraLine = GetWorkCenterSpecificLine(theRec);
                    if (extraLine != "")
                        sr.WriteLine(extraLine);
                    sr.Close();
                }
                catch (Exception ex)
                {
                    sr.Close();
                    theRec.RESULT_STATUS = "INVALID_PPI_PATH";
                    theRec.ERRORLOG = ex.Message;
                    throw new PPIFileException(ex.Message);
                }
            }
        }

        public static void VerifyQueueRecordComplete(int qRecID)
        {
            //Changed By: Sandeep Kumar
            //Changed Date: 03/30/2015
            //Change Description: Wrap the code inside using block. eCR #3075.
            using (var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService"))
            {
                try
                {
                    //var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService");
                    myProxy.ScrubQueueResult(qRecID);
                }
                catch (Exception e)
                {
                    myProxy.Abort();
                    #if _PERFORMANCE_LOG_                    
                        PerfLog.Enter("Method: VerifyQueueRecordComplete, Parameters: Queue record id - " + qRecID + ", Exception:" + e.Message);
                        PerfLog.Exit();
                    #endif
                }
            }
        }

        public static void LogToOracle(int qRecID, string methodName, string paramsToSave, string whatHappened, string callType = "SYNC", bool isUrgent = true)
        {
            bool doLogging = Convert.ToBoolean(ConfigurationManager.AppSettings["oracleLogging"].ToUpper());
            bool urgentsOnly = Convert.ToBoolean(ConfigurationManager.AppSettings["urgentLogsOnly"].ToUpper());
            bool debugging = Convert.ToBoolean(ConfigurationManager.AppSettings["debugmode"].ToUpper());

            string thisModule;
            if (debugging)
                thisModule = "DEBUGGING_WINLIBSVC";
            else
                thisModule = "WINLIBRARYSERVICE";

            //Changed By: Sandeep Kumar
            //Changed Date: 03/26/2015
            //Change Description: Wrap the code inside using block. eCR #3075.
            using (var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService"))
            {
                if (doLogging && !urgentsOnly) //write all logs; urgent or not
                {
                    try
                    {
                        //var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService");
                        if (callType == "ASYNC")
                            myProxy.LogErrorAsync(qRecID.ToString(), methodName, paramsToSave, thisModule, whatHappened);
                        else
                            myProxy.LogError(qRecID.ToString(), methodName, paramsToSave, thisModule, whatHappened);
                    }
                    catch (Exception ex)
                    {
                        myProxy.Abort();
                        #if _PERFORMANCE_LOG_                        
                        PerfLog.Enter("Method: LogToOracle, Calling Method Name: " + methodName + "Exception:" + ex.Message);
                        PerfLog.Exit();
                        #endif
                    }
                }
                else if (doLogging && isUrgent) //write only urgent logs
                {
                    try
                    {
                        //var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService");
                        if (callType == "ASYNC")
                            myProxy.LogErrorAsync(qRecID.ToString(), methodName, paramsToSave, thisModule, whatHappened);
                        else
                            myProxy.LogError(qRecID.ToString(), methodName, paramsToSave, thisModule, whatHappened);
                    }
                    catch (Exception ex)
                    {
                        myProxy.Abort();
                        #if _PERFORMANCE_LOG_
                        PerfLog.Enter("Method: LogToOracle, Calling Method Name: " + methodName + "Exception:" + ex.Message);
                        PerfLog.Exit();
                        #endif
                    }
                }
            }
        }

        /// <summary>
        /// Gets any WorkCenter Specific data needed for Library process to Optimation
        /// </summary>
        /// <param name="qRec">the object QueueRecord</param>
        /// <returns>string needed for Optimation</returns>
        public static string GetWorkCenterSpecificLine(QueueRecord qRec)
        {
            string specialLine = "";
            if (qRec.WORKCENTER == "PIVATIC")
            {
                //Changed By: Sandeep Kumar
                //Changed Date: 03/30/2015
                //Change Description: Wrap the code inside using block. eCR #3075.
                using (var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService"))
                {
                    //var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService");
                    RollFormCnc r = myProxy.GetRollformerCode(ConfigurationManager.AppSettings["db"].ToString(), qRec.PARTNUMBER);
                    if (r.CNC_CODE.Length > 0)
                        specialLine = @"(+MASUDT01 " + r.CNC_CODE + @"+)";
                }
            }

            return specialLine;
        }

        /// <summary>
        /// Copies partnumber from Vault to the destinationPath
        /// </summary>
        /// <param name="partnumber">Base Part Number</param>
        /// <param name="destinationPath">Destination including file name and extension</param>
        /// <returns></returns>
        public static string GetDXFFile(ref QueueRecord thisRecord, string destinationPath)
        {
            string retVal = "";

            string VaultfilePath = ConfigurationManager.AppSettings["DxfVaultPath"];
            string dxfUrl = VaultfilePath + thisRecord.BASEPART + ".dxf";
            ModelQueue.LogToOracle(thisRecord.ID, "GetDXFFile", dxfUrl, " getting DXF file for part#: " + thisRecord.PARTNUMBER, null, false);
            try
            {
                //var fStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
                using (var fStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    try
                    {
                        WebAppClient.Download(new Uri(dxfUrl), fStream);
                        
                        if (fStream.Length < 2)
                        {
                            thisRecord.RESULT_STATUS = "0KB_DXF";
                            ModelQueue.LogToOracle(thisRecord.ID, "GetDXFFile", thisRecord.PARTNUMBER, thisRecord.RESULT_STATUS);

                            #if _PERFORMANCE_LOG_
                                 PerfLog.Enter("GetDXFFile:Download part# " + thisRecord.BASEPART + ": " + "0kb DXF file");
                                 PerfLog.Exit();
                                 PerfLog.Enter("DXF url used= " + dxfUrl);
                                 PerfLog.Exit();
                            #endif

                            throw new ZeroLengthDXFException();
                        }
                        else
                            retVal = destinationPath;
                    }
                    catch (Exception e)
                    {
                        string errType = e.GetType().Name;
                        if (errType != "ZeroLengthDXFException")
                        {
                            thisRecord.RESULT_STATUS = "NO_DXF";
                            ModelQueue.LogToOracle(thisRecord.ID, "GetDXFFile", thisRecord.PARTNUMBER, "Missing DXF Exception: " + e.Message);

                            #if _PERFORMANCE_LOG_
                                PerfLog.Enter("GetDXFFile:Download part# " + thisRecord.BASEPART + ": " + e.Message);
                                PerfLog.Exit();
                            #endif

                            throw new MissingDXFException();
                        }
                        else
                            throw;
                    }
                    finally
                    {
                        fStream.Close();
                    }
                }

                return retVal;
            }
            catch (Exception e)
            {
                string errType = e.GetType().Name;

                if (errType != "MissingDXFException" && errType != "ZeroLengthDXFException")
                {
                    ModelQueue.LogToOracle(thisRecord.ID, "GetDXFFile", thisRecord.PARTNUMBER, e.Message);
                    thisRecord.RESULT_STATUS = "INVALID_DESTINATIONPATH";
                    throw new DestinationPathException(destinationPath + ":" + e.Message);
                }
                else
                    throw;
            }
            finally
            {
                #if _PERFORMANCE_LOG_
                    PerfLog.Exit();
                #endif
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisRecord"></param>
        public static void RemoveCopperSuffix(ref QueueRecord thisRecord)
        {
            string currentMaterial = thisRecord.MATERIALCODE;
            string currentDB = ConfigurationManager.AppSettings["db"];

            if (currentMaterial.EndsWith("B") || currentMaterial.EndsWith("S") || currentMaterial.EndsWith("T"))
            {
                try
                {
                    var myProxy = new PartServiceClient("CustomBinding_IPartService"); //updated
                    AgilePart partdetail = myProxy.GetAgilePartDetail(currentDB, currentMaterial, thisRecord.ORGID);
                    if (partdetail.CONFIGURATION.ToUpper() == "BAR" || partdetail.CONFIGURATION.ToUpper() == "BUS BAR")
                    {
                        thisRecord.MATERIALCODE = currentMaterial.Substring(0, currentMaterial.Length - 1);
                    }
                    myProxy.Close();
                }
                catch (Exception ex)
                {
                    thisRecord.RESULT_STATUS = "AGILE_ERROR";
                    throw new AgileConfigurationException(ex.Message);                    
                }
            } 
        }

        /// <summary>
        ///     Gets the next record in the queue for either any resource (ALL) or the resource that is passed
        /// </summary>
        /// <param name="resource">Work Center Name or ALL</param>
        /// <param name="numRecsToReturn">Number of Records to Return</param>
        /// <returns>oldest Queue Record to process</returns>
        public static List<NestPost> GetAllPosts()
        {
            string currentDB = ConfigurationManager.AppSettings["db"];
            List<NestPost> returnedPosts = new List<NestPost>();

            //Changed By: Sandeep Kumar
            //Changed Date: 03/26/2015
            //Change Description: Wrap the code inside using block. eCR #3075.
            using (var myProxy = new LibraryServiceClient("BasicHttpBinding_ILibraryService"))
            {
                try
                {
                    returnedPosts = myProxy.GetNestPostsList(currentDB);
                    NestPost RadanPost = new NestPost();
                    RadanPost.POSTNAME = "RADAN";
                    RadanPost.SYNCPOST = 0;
                    returnedPosts.Insert(0, RadanPost);
                }
                catch (Exception e)
                {
                    myProxy.Abort();
                    #if _PERFORMANCE_LOG_
                        PerfLog.Enter("GetNextQueueRec Error ignored:" + e.Message);
                        PerfLog.Exit();
                    #endif
                }                
            }

            return returnedPosts;
        }

    }
}