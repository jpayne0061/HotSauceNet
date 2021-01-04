using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace WebServer
{

    class Program
    {
        static string[] _prefixes = new string[] { "http://localhost:8080/" };

        static HttpListener _listener;

        static void Main(string[] args)
        {
            Start();
        }

        static void Start()
        {
            _listener = new HttpListener();

            foreach (string s in _prefixes)
            {
                _listener.Prefixes.Add(s);
            }

            StartHttpListener();

            _listener.Stop();
        }

        public static void StartHttpListener()
        {
            _listener.Start();

            Console.WriteLine("Listening...");

            ListenForRequest();

            StartHttpListener();
        }

        static void ListenForRequest()
        {
            HttpListenerContext context = _listener.GetContext();

            ProcessRequest(context);
        }

        static void ProcessRequest(HttpListenerContext context)
        {
            Socket mySocket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            HttpListenerResponse response = context.Response;

            try
            {
                string url = context.Request.RawUrl;

                string controllerName = ParseControllerName(url);

                byte[] faviconRequest = HandleFaviconRequest(controllerName);

                if (faviconRequest != null)
                {
                    WriteContentToBuffer(response, faviconRequest);

                    return;
                }

                string methodName = ParseMethodName(url);

                string urlParameters = ParseParameter(url);

                string[] actionParameters = urlParameters == null ? null : new string[] { urlParameters };

                var getHtml = RouteRequest(controllerName, methodName, actionParameters);

                getHtml = "<html><body> " + getHtml + "</body></html>";

                WriteContentToBuffer(response, Encoding.UTF8.GetBytes(getHtml));
            }
            catch (Exception ex)
            {
                WriteContentToBuffer(response, Encoding.UTF8.GetBytes(PrepareErrorMessage(ex.Message + "\r\n\r\n" + ex.StackTrace)));
            }
            
        }

        static void WriteContentToBuffer(HttpListenerResponse response, byte[] buffer)
        {
            response.StatusCode = 200;
            response.ProtocolVersion = HttpVersion.Version11;
            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;
            response.ContentEncoding = Encoding.UTF8;

            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        static string ParseControllerName(string url)
        {
            try
            {
                return url.Split('/')[1];
            }
            catch
            {
                throw new Exception("could not find controller name");
            }
        }

        static string ParseMethodName(string url)
        {
            try
            {
                return url.Split('/')[2];
            }
            catch
            {
                throw new Exception("could not find method name");
            }
        }

        static string ParseParameter(string url)
        {
            try
            {
                return url.Split('/')[3];
            }
            catch
            {
                return null;
            }
        }

        static string RouteRequest(string controllerName, string methodName, string[] parameters)
        {
            Type[] typelist = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "WebServer.Controllers");

            Type controllerClassType = typelist.Where(t => t.Name == controllerName + "Controller").First();

            var controllerInstance = Activator.CreateInstance(controllerClassType);

            MethodInfo method = controllerClassType.GetMethod(methodName);

            string response = (string)method.Invoke(controllerInstance, parameters);

            return response;
        }
        static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
              assembly.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                      .ToArray();
        }

        static byte[] GetFaviconBytes()
        {
            byte[] imageArray = System.IO.File.ReadAllBytes(@"C:\Users\evan.payne\Downloads\favicon_io\favicon.ico");
            return imageArray;
        }

        static byte[] HandleFaviconRequest(string controllerName)
        {
            if(controllerName == "favicon.ico")
            {
                return GetFaviconBytes();
            }

            return null;
        }

        static string PrepareErrorMessage(string exceptionMessage)
        {
            return "<div style='color: red'>" + exceptionMessage + "</div>";
        }

    }

    
}


