using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Text;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Configuration;

namespace XJTU.Common
{
    public class HttpWebAsk
    {
        public class ResponseObject
        {
            public HttpWebResponse Resp;
            public object State;
            public byte[] Content;
        }

        public delegate void ResponseCallback(ResponseObject ro);

        #region
        private class AsynResp
        {
            internal WebRequest Request;
            internal ResponseCallback Callback;
            internal ResponseObject Response;
            internal Stream RespStream;
            internal ArrayList Contents;
            internal byte[] Current;
        }

        private class AsynRequestStream
        {
            internal AsynResp Resp;
            internal byte[] Data;
        }

        private static void DoUserCallback(AsynResp ar)
        {
            if (ar.Callback != null)
                ThreadPool.QueueUserWorkItem(new WaitCallback(UserCallback), ar);
        }

        private static void RequestStreamCallback(IAsyncResult ir)
        {
            AsynRequestStream rs = (AsynRequestStream)ir.AsyncState;
            try
            {
                Stream requestStream = rs.Resp.Request.EndGetRequestStream(ir);
                requestStream.Write(rs.Data, 0, rs.Data.Length);
                requestStream.Close();
                rs.Data = null;
                rs.Resp.Request.BeginGetResponse(new AsyncCallback(RespCallback), rs.Resp);
            }
            catch (Exception e)
            {
                // LogHelper.WriteLog(typeof(HttpWebAsk), LogStatusLevel.ERROR, e.Message);
                DoUserCallback(rs.Resp);
            }
        }

        private static void RespCallback(IAsyncResult ir)
        {
            AsynResp ar = (AsynResp)ir.AsyncState;
            try
            {
                HttpWebResponse resp = (HttpWebResponse)ar.Request.EndGetResponse(ir);
                ar.Request = null;
                if (ar.Callback != null)
                {
                    ar.Response.Resp = resp;
                    ar.RespStream = resp.GetResponseStream();
                    ar.Contents = new ArrayList();
                    ar.Current = new byte[1024];
                    ar.RespStream.BeginRead(ar.Current, 0, ar.Current.Length,
                        new AsyncCallback(RespReadCallback), ar);
                }
            }
            catch (Exception e)
            {
                //LogHelper.WriteLog(typeof(HttpWebAsk), LogStatusLevel.ERROR, e.Message);
                DoUserCallback(ar);
            }
        }

        private static void RespReadCallback(IAsyncResult ir)
        {
            AsynResp ar = (AsynResp)ir.AsyncState;
            try
            {
                int readed = ar.RespStream.EndRead(ir);
                if (readed >= ar.Current.Length)
                {
                    ar.Contents.Add(ar.Current);
                    ar.Current = new byte[1024];
                    ar.RespStream.BeginRead(ar.Current, 0, ar.Current.Length,
                        new AsyncCallback(RespReadCallback), ar);
                    return;
                }

                int length = ar.Contents.Count * 1024 + readed;
                if (length > 0)
                {
                    ar.Response.Content = new byte[length];
                    int index = 0;
                    foreach (byte[] bs in ar.Contents)
                    {
                        Array.Copy(bs, 0, ar.Response.Content, index, bs.Length);
                        index += bs.Length;
                    }

                    if (readed > 0)
                        Array.Copy(ar.Current, 0, ar.Response.Content, index, readed);
                }
                ar.Contents = null;
                ar.Current = null;
                ThreadPool.QueueUserWorkItem(new WaitCallback(UserCallback), ar);
            }
            catch (Exception e)
            {
                //LogHelper.WriteLog(typeof(HttpWebAsk), LogStatusLevel.ERROR, e.Message);
                DoUserCallback(ar);
            }
        }

        private static void UserCallback(object o)
        {
            AsynResp ar = (AsynResp)o;
            try
            {
                ar.Callback(ar.Response);
            }
            catch (Exception e)
            {
                //LogHelper.WriteLog(typeof(HttpWebAsk), LogStatusLevel.ERROR, e.Message);
            }
            if (ar.Response.Resp != null)
                ar.Response.Resp.Close();
        }
        #endregion

        static public HttpWebResponse Get(string url, CookieCollection cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (cookie != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookie);
            }
            return (HttpWebResponse)request.GetResponse();
        }

        static public HttpWebResponse Post(string url, byte[] data, CookieCollection cookie)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                if (cookie != null)
                {
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(cookie);
                }

                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                //https检查
                request = SetHttpsCertifications(request);

                Stream stream = request.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                return (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {

                throw;
            }
        }

        static public void AsynGet(string url, ResponseCallback callback, object state, CookieCollection cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (cookie != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookie);
            }
            AsynResp resp = new AsynResp
            {
                Request = request,
                Callback = callback,
                Response = new ResponseObject
                {
                    State = state
                }
            };
            request.BeginGetResponse(new AsyncCallback(RespCallback), resp);
        }

        static public void AsynPost(string url, byte[] data, ResponseCallback callback, object state, CookieCollection cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (cookie != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookie);
            }
            request.AllowAutoRedirect = true;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            AsynResp resp = new AsynResp
            {
                Request = request,
                Callback = callback,
                Response = new ResponseObject
                {
                    State = state
                }

            };
            AsynRequestStream ars = new AsynRequestStream
            {
                Data = data,
                Resp = resp
            };
            request.BeginGetRequestStream(new AsyncCallback(RequestStreamCallback), ars);
        }

        static public HttpWebResponse Get(string url)
        {
            return Get(url, null);
        }

        static public HttpWebResponse Post(string url, byte[] data)
        {
            return Post(url, data, null);
        }

        static public void AsynGet(string url, ResponseCallback callback, object state)
        {
            AsynGet(url, callback, state, null);
        }

        static public void AsynPost(string url, byte[] data, ResponseCallback callback, object state)
        {
            AsynPost(url, data, callback, state, null);
        }
        public static int Post(string url, string strData, out string ret)
        {
            byte[] data = Encoding.UTF8.GetBytes(strData);
            ret = "";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            //https检查
            if (IsHttpsUrl(url))
            {
                request = SetHttpsCertifications(request);
            }

            var stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();
            var response = (HttpWebResponse)request.GetResponse();
            if (request.HaveResponse)
            {
                var responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    var reader = new StreamReader(responseStream, Encoding.UTF8);
                    ret = reader.ReadToEnd();
                    reader.Close();
                    responseStream.Close();
                }
                response.Close();
            }

            return 0;
        }

        public static HttpWebRequest SetHttpsCertifications(HttpWebRequest request)
        {
            string AppSettings_CertFilePath = ConfigurationManager.AppSettings["CertFilePath"];
            string AppSettings_CertPassword = ConfigurationManager.AppSettings["CertPassword"];
            bool isUseHttps = !string.IsNullOrWhiteSpace(AppSettings_CertFilePath)
                                        && !string.IsNullOrEmpty(AppSettings_CertPassword);
            if (isUseHttps && null != request)
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object s, X509Certificate cert, X509Chain chain, SslPolicyErrors errors) => { return true; });
                request.KeepAlive = true;
                X509Certificate2 certificate = new X509Certificate2(System.Web.HttpContext.Current.Server.MapPath(@"~/" + AppSettings_CertFilePath), AppSettings_CertPassword);//"abc123!@#"
                request.ClientCertificates.Add(certificate);
                request.UseDefaultCredentials = true;
            }
            return request;
        }

        private static bool IsHttpsUrl(string url)
        {
            bool result = false;
            if (!string.IsNullOrWhiteSpace(url))
            {
                if (url.Length > 5)
                {
                    if (url.Substring(0, 5).ToLower() == "https")
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
    }
}