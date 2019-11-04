using System;
using System.IO;
using System.Net;
using Powell.ENG.Log.Performance;
using log4net;

namespace Powell.Mfg.LibraryWinService
{
    public class WebAppClient
    {
#if _PERFORMANCE_LOG_
        private static readonly IPerfLog PerfLog = PerfLogManager.GetLog(typeof (WebAppClient));
#endif
        private static readonly ILog Logger = LogManager.GetLogger(typeof (WebAppClient));

        private const int BufferSize = 1024*1024;
        private const string UserAgent = "Powell.Mfg.LibraryWinService";

        private static void PreAuthenticate(Uri remoteUri)
        {
#if _PERFORMANCE_LOG_
            PerfLog.Enter(remoteUri);
#endif
            try
            {
                var httpRequest = (HttpWebRequest) WebRequest.Create(remoteUri);

                httpRequest.Credentials = new CredentialCache {{remoteUri, "NTLM", (NetworkCredential) CredentialCache.DefaultCredentials}};
                httpRequest.UnsafeAuthenticatedConnectionSharing = true;
                httpRequest.UserAgent = UserAgent;
                httpRequest.Method = "HEAD";
                httpRequest.Timeout = int.MaxValue;
                httpRequest.ContentLength = 0;

                var httpResponse = (HttpWebResponse) httpRequest.GetResponse();
                httpResponse.Close();
            }
            catch (Exception e)
            {
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

        public static void Upload(Uri remoteUri, Stream stream)
        {
#if _PERFORMANCE_LOG_
            PerfLog.Enter(remoteUri);
#endif
            try
            {
                PreAuthenticate(remoteUri);

                var httpRequest = (HttpWebRequest) WebRequest.Create(remoteUri);

                httpRequest.UnsafeAuthenticatedConnectionSharing = true;
                httpRequest.AllowWriteStreamBuffering = false;
                httpRequest.UserAgent = UserAgent;
                httpRequest.Timeout = int.MaxValue;
                httpRequest.Method = "PUT";

                httpRequest.ContentLength = stream.Length;
                Stream requestStream = httpRequest.GetRequestStream();
                try
                {
                    int bytesRead;
                    var buffer = new byte[BufferSize];
                    while ((bytesRead = stream.Read(buffer, 0, BufferSize)) > 0)
                    {
                        requestStream.Write(buffer, 0, bytesRead);
                    }
                }
                finally
                {
                    requestStream.Close();
                }

                var httpResponse = (HttpWebResponse) httpRequest.GetResponse();

                try
                {
                    Stream responseStream = httpResponse.GetResponseStream();
                    if (responseStream != null)
                    {
                        var sr = new StreamReader(responseStream);

                        string response = sr.ReadToEnd();

                        if (response.Length > 0) throw new Exception(response);
                    }

                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception("Http Error, StatusCode: " + httpResponse.StatusCode + ", StatusDescription: " +
                                            httpResponse.StatusDescription);
                    }
                }
                finally
                {
                    httpResponse.Close();
                }
            }
            catch (Exception e)
            {
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

        public static Stream OpenStream(Uri remoteUri)
        {
#if _PERFORMANCE_LOG_
            PerfLog.Enter(remoteUri);
#endif
            try
            {
                //                remoteUri = _webUriString + remoteUri;

                //                PreAuthenticate();

                var httpRequest = (HttpWebRequest) WebRequest.Create(remoteUri);

                httpRequest.Credentials = new CredentialCache {{remoteUri, "NTLM", (NetworkCredential) CredentialCache.DefaultCredentials}};
                httpRequest.UnsafeAuthenticatedConnectionSharing = true;
                httpRequest.AllowWriteStreamBuffering = false;
                httpRequest.UserAgent = UserAgent;
                httpRequest.Timeout = int.MaxValue;

                var httpResponse = (HttpWebResponse) httpRequest.GetResponse();

                if (httpResponse.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Http Error, StatusCode: " + httpResponse.StatusCode + ", StatusDescription: " +
                                        httpResponse.StatusDescription);
                }

                //                PostAuthenticate();

                return httpResponse.GetResponseStream();
            }
            catch (Exception e)
            {
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

        public static void Download(Uri remoteUri, Stream localStream)
        {
            try
            {
                //                remoteUri = _webUriString + remoteUri;

                //                PreAuthenticate();

                var httpRequest = (HttpWebRequest)WebRequest.Create(remoteUri);
                httpRequest.Credentials = new CredentialCache {{remoteUri, "NTLM", (NetworkCredential) CredentialCache.DefaultCredentials}};
                httpRequest.UnsafeAuthenticatedConnectionSharing = true;
                httpRequest.AllowWriteStreamBuffering = false;
                httpRequest.UserAgent = UserAgent;
                httpRequest.Timeout = int.MaxValue;

                var httpResponse = (HttpWebResponse) httpRequest.GetResponse();

                try
                {
                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception("Http Error, StatusCode: " + httpResponse.StatusCode + ", StatusDescription: " +
                                            httpResponse.StatusDescription);
                    }

                    Stream remoteStream = httpResponse.GetResponseStream();

                    if (remoteStream != null)
                    {
                        try
                        {
                            int readCount;
                            var buffer = new byte[BufferSize];
                            while ((readCount = remoteStream.Read(buffer, 0, BufferSize)) > 0)
                            {
                                localStream.Write(buffer, 0, readCount);
                            }
                        }
                        finally
                        {
                            remoteStream.Close();
                        }
                    }
                }
                finally
                {
                    httpResponse.Close();
                }

                //                PostAuthenticate();
            }
            catch (Exception e)
            {
                #if _PERFORMANCE_LOG_
                    PerfLog.Enter(remoteUri + ": " + e.Message);
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
    }
}