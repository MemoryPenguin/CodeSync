using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MemoryPenguin.CodeSync
{
    /// <summary>
    /// Responsible for serving a <c>Project</c> object via a <c>LocalServer</c>.
    /// </summary>
    class ProjectServer
    {
        private Project project;
        private LocalServer server;

        public bool AllowExternalRequests
        {
            get
            {
                return server.AllowExternalRequests;
            }
            set
            {
                server.AllowExternalRequests = value;
            }
        }

        public bool IsRunning
        {
            get
            {
                return server.IsRunning;
            }
        }

        public ProjectServer(Project project, int port)
        {
            this.project = project;
            server = new LocalServer(port);
            server.AddHandler("list", SendFileList);
            server.AddHandler("changes", SendChanges);
            server.AddHandler("read", ReadFile);
            server.AddHandler("info", GetSyncDetails);
        }

        public void Start()
        {
            server.Start();
        }

        public void Stop()
        {
            server.Stop();
        }

        private string SendFileList(HttpListenerRequest request, HttpListenerResponse response, NameValueCollection args)
        {
            return JsonConvert.SerializeObject(project.GetFilesInProject());
        }

        private string SendChanges(HttpListenerRequest request, HttpListenerResponse response, NameValueCollection args)
        {
            return JsonConvert.SerializeObject(project.GetChanges());
        }

        private string ReadFile(HttpListenerRequest request, HttpListenerResponse response, NameValueCollection args)
        {
            string relPath = args["file"];
            string absPath = Path.Combine(project.RootPath, relPath);
            
            return File.ReadAllText(absPath);
        }

        private string GetSyncDetails(HttpListenerRequest request, HttpListenerResponse response, NameValueCollection args)
        {
            Dictionary<string, string> details = new Dictionary<string, string>();
            details["Target"] = project.RobloxStorageLocation;
            details["Source"] = project.RootPath;
            return JsonConvert.SerializeObject(details);
        }
    }
}
