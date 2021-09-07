using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace HTTPServer.Routers
{
    public class StaticFilesRouter
    {
        private const string StaticFilesFolderName = "wwwroot";
        private const string DefaultFileName = "index.html";

        private readonly List<Uri> servedUri;
        private readonly Uri baseUri;

        public StaticFilesRouter(Uri baseUri)
        {
            this.baseUri = baseUri;
            this.servedUri = new List<Uri>
            {
                baseUri
            };

            AddStaticFilesEndPoints();
        }

        private void AddStaticFilesEndPoints()
        {
            var scripts = Directory.GetFiles(StaticFilesFolderName, "*.js", SearchOption.AllDirectories);
            var styles = Directory.GetFiles(StaticFilesFolderName, "*.css", SearchOption.AllDirectories);
            var htmls = Directory.GetFiles(StaticFilesFolderName, "*.html", SearchOption.AllDirectories);
            var ico = Directory.GetFiles(StaticFilesFolderName, "favicon.ico", SearchOption.AllDirectories);
            var staticFiles = scripts.Concat(styles).Concat(htmls).Concat(ico);

            foreach (var file in staticFiles)
            {
                servedUri.Add(new Uri(baseUri, file.Remove(0, StaticFilesFolderName.Length)));
            }
        }

        public IEnumerable<Uri> GetEndPoints() => servedUri;

        public bool TryGetEndPointFor(HttpListenerRequest request, out Action<HttpListenerContext> endPoint)
        {
            var result = false;
            endPoint = null;

            var requesHttpMethod = new HttpMethod(request.HttpMethod);
            if (requesHttpMethod != HttpMethod.Get)
            {
                return result;
            }

            var requestedUri = GetRequestedUri(request);
            if (requestedUri.IsFile || servedUri.Contains(requestedUri))
            {
                endPoint = async (context) =>
                {
                    var filePath = StaticFilesFolderName + "/" + requestedUri.LocalPath;
                    var file = File.ReadAllText(filePath);
                    var fileExtension = Path.GetExtension(filePath);

                    var buffer = Encoding.UTF8.GetBytes(file);
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.ContentType = fileExtension switch
                    {
                        ".html" => "text/html; charset=utf-8",
                        ".js" => "application/javascript; charset=utf-8",
                        ".css" => "text/css; charset=utf-8",
                        ".ico" => "image/x-icon",
                        _ => "text/plain; charset=utf-8"
                    };
                    await context.Response.OutputStream.WriteAsync(buffer.AsMemory(0, buffer.Length));

                };
                result = true;
            }

            return result;
        }

        private Uri GetRequestedUri(HttpListenerRequest request)
        {
            return request.Url == baseUri ? new Uri(baseUri, DefaultFileName) : request.Url;
        }
    }
}
