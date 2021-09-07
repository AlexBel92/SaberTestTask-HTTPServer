using System;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using HTTPServer.Routers;
using HTTPServer.Factories;
using System.Collections.Generic;

namespace HTTPServer
{
    public class HTTPServer
    {
        private readonly HttpListener listener;
        private readonly ControllerRouter controllerRouter;
        private readonly StaticFilesRouter staticFilesRouter;

        public HTTPServer(HttpListener listener, Uri baseUri, IControllerFactory controllerFactory)
        {
            if (baseUri is null)
                throw new ArgumentNullException(nameof(baseUri));
            if (controllerFactory is null)
                throw new ArgumentNullException(nameof(controllerFactory));

            this.listener = listener ?? throw new ArgumentNullException(nameof(listener));
            this.controllerRouter = new ControllerRouter(baseUri, controllerFactory);
            this.staticFilesRouter = new StaticFilesRouter(baseUri);

            AddPrefixesToListner(staticFilesRouter.GetEndPoints());
            AddPrefixesToListner(controllerRouter.GetEndPoints());
        }

        private void AddPrefixesToListner(IEnumerable<Uri> prefixes)
        {
            foreach (var uri in staticFilesRouter.GetEndPoints())
            {
                var prefix = uri.AbsoluteUri.EndsWith('/') ? uri.AbsoluteUri : uri.AbsoluteUri + '/';
                listener.Prefixes.Add(prefix);
            }
        }

        public async Task RunAsync()
        {
            try
            {
                listener.Start();
                Console.WriteLine($"HTTP server is now running");

                while (true)
                {
                    await AcceptClientsAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task AcceptClientsAsync()
        {
            var httpContext = await listener.GetContextAsync();
            new Task(async () => await ProcessAsync(httpContext)).Start();
        }

        private async Task ProcessAsync(HttpListenerContext context)
        {
            try
            {
                Action<HttpListenerContext> endPoint = async (context) 
                    => await SendStatusCodeResult(context.Response, HttpStatusCode.NotFound);

                if (staticFilesRouter.TryGetEndPointFor(context.Request, out Action<HttpListenerContext> staticFilesEndPoint))
                {
                    endPoint = staticFilesEndPoint;
                }
                else if (controllerRouter.TryGetEndPointFor(context.Request, out Action<HttpListenerContext> controllerEndPoint))
                {
                    endPoint = controllerEndPoint;
                }

                endPoint(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await SendStatusCodeResult(context.Response, HttpStatusCode.InternalServerError);
            }
        }

        private static async Task SendStatusCodeResult(HttpListenerResponse response, HttpStatusCode statusCode, string message = "")
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            response.ContentLength64 = buffer.Length;
            response.StatusCode = (int)statusCode;
            using var outputStream = response.OutputStream;
            await outputStream.WriteAsync(buffer.AsMemory(0, buffer.Length));
        }
    }
}