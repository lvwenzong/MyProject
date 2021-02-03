using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Web;

namespace WebApplication1.Utils
{
    public class HttpUtils
    {
        /// <summary>
        /// 执行HTTP GET请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="pars">请求参数</param>
        /// <returns>HTTP响应</returns>
        public static string DoGet(string url, IDictionary<string, string> pars)
        {
            if (pars != null && pars.Count > 0) {
                if (url.Contains("?")) {
                    url = url + "&" + BuildPostData(pars);
                } else {
                    url = url + "?" + BuildPostData(pars);
                }
            }

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.Expect100Continue = false;
            req.Method = "GET";
            req.KeepAlive = true;
            req.UserAgent = "FSLB";
            req.Proxy = null;
            req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

            HttpWebResponse rsp = null;
            try {
                rsp = (HttpWebResponse)req.GetResponse();
            } catch (WebException webEx) {
                if (webEx.Status == WebExceptionStatus.Timeout) {
                    rsp = null;
                }
            }

            if (rsp != null) {
                string Encod = rsp.CharacterSet;
                if (string.IsNullOrEmpty(Encod))
                    Encod = "utf-8";
                if (!string.IsNullOrEmpty(Encod)) {
                    Encoding encoding = Encoding.GetEncoding(Encod);
                    return GetResponseAsString(rsp, encoding);
                } else {
                    return string.Empty;
                }
            } else {
                return string.Empty;
            }
        }

        /// <summary>
        /// GET请求(Tip:URL不要过长)
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public static string WebRestGetData(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Proxy = null; // 不设置代理
            HttpWebResponse httpWebResponse;
            try
            {
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpWebResponse = (HttpWebResponse)ex.Response;
            }
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
            string result = streamReader.ReadToEnd();

            streamReader.Close();
            httpWebResponse.Close();

            return result;
        }

        /// <summary>  
        /// GET请求与获取结果  
        /// </summary>  
        public static string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }


        /// <summary>
        /// 参数json格式化
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string PostData(string url, object parameters)
        {
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "Post";
                request.ContentType = "application/json;charset=utf-8";
                using (var writer = new StreamWriter(request.GetRequestStream())) {
                    writer.Write(JsonObject.SerializeObject(parameters));
                }
                using (var reader = new StreamReader(request.GetResponse().GetResponseStream())) {
                    return reader.ReadToEnd();
                }
            } catch (Exception e) {
                return e.StackTrace;
            }
        }

        /// <summary>
        /// 功能描述：以POST方式模拟
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parms">参数字典(key：文件名称，value：字节码)</param>
        /// <param name="files">需上传文件路径字典</param>
        /// <returns></returns>
        public static string doPost(string url, Dictionary<string, string> parms, Dictionary<string, byte[]> files)
        {
            string backStr = string.Empty;

            // 创建链接 请求设置
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.99 Safari/537.36";

            // 处理参数
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(memoryStream);
            if (files == null || files.Count == 0) {
                httpRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                StringBuilder sb = new StringBuilder();
                foreach (var key in parms.Keys) {
                    sb.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(parms[key]));
                }
                if (sb.Length > 0) sb.Length--;
                writer.Write(sb.ToString());
            } else {
                string boundary = $"WebKitFormBoundary{Guid.NewGuid().ToString().Replace("-", "")}";
                httpRequest.ContentType = $"multipart/form-data; boundary={boundary}";
                httpRequest.KeepAlive = true;
                httpRequest.Accept = "*/*";
                httpRequest.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                httpRequest.AllowAutoRedirect = false;
                httpRequest.CookieContainer = new CookieContainer();

                string newLine = "\r\n";
                string splitStr = "--";
                // 表单文本参数
                if (parms != null && parms.Count > 0) {
                    foreach (var key in parms.Keys) {
                        writer.Write($"{splitStr}{boundary}{newLine}");
                        writer.Write($"Content-Disposition: form-data; name=\"{key}\"{newLine}{newLine}");
                        writer.Write($"{parms[key]}{newLine}");
                    }
                }

                // 表单附件
                foreach (var key in files.Keys) {
                    writer.Write($"{splitStr}{boundary}{newLine}");
                    writer.Write($"Content-Disposition: form-data; name=\"file\"; filename=\"{key}\"{newLine}");
                    writer.Write($"Content-Type: application/octet-stream{newLine}{newLine}");
                    writer.Flush(); // 清理

                    byte[] bytes = files[key];
                    memoryStream.Write(bytes, 0, bytes.Length);

                    writer.Write(newLine);
                    writer.Write($"{splitStr}{boundary}{splitStr}{newLine}");
                }
            }
            writer.Flush();

            // 请求返回结果
            HttpWebResponse httpResponse = null;
            StreamReader reader = null;
            try {
                // 将数据流写入HttpRequest请求流中
                using (Stream stream = httpRequest.GetRequestStream()) {
                    memoryStream.WriteTo(stream);
                }
                // 获取响应
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                reader = new StreamReader(httpResponse.GetResponseStream());
                backStr = reader.ReadToEnd();
            } catch (WebException ex) {
                backStr = ex.Message; // 记录错误
            } finally {
                if (reader != null) reader.Close();
                if (httpResponse != null) httpResponse.Close();
            }

            return backStr;
        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        private static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            StringBuilder result = new StringBuilder();
            Stream stream = null;
            StreamReader reader = null;

            try {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);

                // 每次读取不大于256个字符，并写入字符串
                char[] buffer = new char[256];
                int readBytes = 0;
                while ((readBytes = reader.Read(buffer, 0, buffer.Length)) > 0) {
                    result.Append(buffer, 0, readBytes);
                }
            } catch (WebException webEx) {
                if (webEx.Status == WebExceptionStatus.Timeout) {
                    result = new StringBuilder();
                }
            } finally {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }

            return result.ToString();
        }

        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典。</param>
        /// <returns>URL编码后的请求数据。</returns>
        private static string BuildPostData(IDictionary<string, string> parameters)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext()) {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value)) {
                    if (hasParam) {
                        postData.Append("&");
                    }

                    postData.Append(name);
                    postData.Append("=");
                    postData.Append(Uri.EscapeDataString(value));
                    hasParam = true;
                }
            }

            return postData.ToString();
        }

        /// <summary>
        /// 获取客户端IP(http客户端)
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            string ip = string.Empty;
            try {
                //if (!string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
                //    ip = Convert.ToString(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
                //if (string.IsNullOrEmpty(ip))
                //    ip = Convert.ToString(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
                ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]?.ToString();
                if (string.IsNullOrEmpty(ip)) {
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]?.ToString();
                }
            } catch { }
            return ip;
        }
    }
}
