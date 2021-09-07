using HTTPServer.Model;
using System;
using System.IO;
using System.Net;
using System.Text.Json;

namespace HTTPServer.Routers
{
    public static class HttpRequestReader
    {
        public static bool TryGetBodyStringFrom(HttpListenerRequest request, out string bodyString)
        {
            if (request is null)            
                throw new ArgumentNullException(nameof(request));

            bodyString = string.Empty;
            if (request.HasEntityBody)
            {
                try
                {
                    using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                    bodyString = reader.ReadToEnd();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return bodyString != string.Empty;
        }

        public static bool TryGetPersonFrom(HttpListenerRequest request, out PersonViewModel person)
        {
            person = null;            

            if (TryGetBodyStringFrom(request, out string bodyString))
            {
                try
                {
                    person = JsonSerializer.Deserialize<PersonViewModel>(bodyString);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return person is not null;
        }
    }
}
