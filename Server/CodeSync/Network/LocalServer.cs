using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace MemoryPenguin.CodeSync.Network
{
    class LocalServer : IDisposable
    {
        /// <summary>
        /// Whether to allow requests that did not originate from the local machine.
        /// Enabling this allows exposure of the file system to remote systems. Use with caution.
        /// </summary>
        public bool AllowExternalRequests { get; set; }

        /// <summary>
        /// The port the server is running on.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Whether the server is running or not.
        /// </summary>
        public bool IsRunning { get; private set; }

        private HttpListener listener;
        private Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, NameValueCollection, string>> handlers;

        public LocalServer(int port)
        {
            // theoretically not an issue, but never hurts to make sure
            if (!HttpListener.IsSupported)
                throw new PlatformNotSupportedException("HttpListener is not supported on this platform");

            handlers = new Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, NameValueCollection, string>>();

            listener = new HttpListener();
            // add local prefixes
            listener.Prefixes.Add($"http://localhost:{port}/");
            listener.Prefixes.Add($"http://127.0.0.1:{port}/");
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

            // by default
            AllowExternalRequests = false;
            Port = port;
        }

        /// <summary>
        /// Adds a handler to the server that is invoked when a request is made to a specific URL.
        /// </summary>
        /// <param name="method">The method for which the handler should respond to</param>
        /// <param name="handler">The handler</param>
        public void AddHandler(string method, Func<HttpListenerRequest, HttpListenerResponse, NameValueCollection, string> handler)
        {
            handlers.Add(method, handler);
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Start()
        {
            listener.Start();
            IsRunning = true;
            
            Console.WriteLine($"Server has begun listening on port {Port}.");

            while (listener.IsListening)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                Stream responseStream = response.OutputStream;

                try
                {
                    // LocalPath starts with a /; we don't want that
                    string target = request.Url.LocalPath.Substring(1);
                        
                    if (request.IsLocal || AllowExternalRequests)
                    {
                        // much cleaner than regexes
                        NameValueCollection args = HttpUtility.ParseQueryString(request.Url.Query);

                        if (handlers.ContainsKey(target))
                        {
                            // whee
                            try
                            {
                                string responseText = handlers[target](request, response, args);
                                byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);
                                responseStream.Write(responseBytes, 0, responseBytes.Length);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Exception invoking handler for {target}: {e.Message}");
                                response.StatusCode = 500;
                            }
                        }
                        else
                        {
                            // oh no
                            Console.WriteLine($"No handler registered for {target}!");
                            response.StatusCode = 404;
                        }
                    }
                    // non-local request and we don't allow those, go away
                    else
                    {
                        // tattle
                        Console.WriteLine($"Got non-local request from {request.RemoteEndPoint.ToString()}.");
                        // gooby you aren't allowed
                        response.StatusCode = 403;
                    }
                }
                catch (Exception e)
                {
                    // oh no something went wrong
                    Console.WriteLine($"Exception while listening: {e.Message}");
                    response.StatusCode = 500;
                }
                finally
                {
                    responseStream.Close();
                    response.Close();
                }
            }
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
            listener.Stop();
        }

        public void Dispose()
        {
            listener.Close();
        }
    }
}
