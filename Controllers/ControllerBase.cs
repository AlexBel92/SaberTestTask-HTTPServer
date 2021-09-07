using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;

namespace HTTPServer.Controllers
{
    public abstract class ControllerBase
    {
        protected readonly HttpListenerContext context;

        public ControllerBase(HttpListenerContext context)
        {
            this.context = context;
        }

        protected HttpListenerRequest Request => context.Request;
        protected HttpListenerResponse Response => context.Response;

        protected async Task JsonResult(string content)
        {        
            var buffer = Encoding.UTF8.GetBytes(content);
            Response.ContentLength64 = buffer.Length;
            Response.ContentType = "application/json; charset=utf-8";
            await WriteToOutputStream(buffer);
        }

        protected async Task StatusCodeResult(HttpStatusCode statusCode ,string message = "")
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            Response.ContentLength64 = buffer.Length;
            Response.StatusCode = (int)statusCode;
            await WriteToOutputStream(buffer);
        }

        private async Task WriteToOutputStream(byte[] buffer)
        {
            var outputStream = Response.OutputStream;
            await outputStream.WriteAsync(buffer.AsMemory(0, buffer.Length));
        }
    }
}
