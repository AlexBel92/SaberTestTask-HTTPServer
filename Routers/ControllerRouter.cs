using HTTPServer.Factories;
using HTTPServer.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace HTTPServer.Routers
{
    public class ControllerRouter
    {
        private readonly Dictionary<Uri, Dictionary<HttpMethod, Action<HttpListenerContext>>> routes;
        private readonly Uri personsEndPoint;

        public ControllerRouter(Uri baseUri, IControllerFactory controllerFactory)
        {
            personsEndPoint = new Uri(baseUri, "/api/persons");

            routes = new Dictionary<Uri, Dictionary<HttpMethod, Action<HttpListenerContext>>>
            {
                { personsEndPoint, new Dictionary<HttpMethod, Action<HttpListenerContext>>() }
            };

            routes[personsEndPoint].Add(
                HttpMethod.Get,
                async (context) =>
                {
                    var controller = controllerFactory.GetPersonsController(context);
                    await controller.Get();
                });

            routes[personsEndPoint].Add(
                HttpMethod.Post,
                async (context) =>
                {
                    var controller = controllerFactory.GetPersonsController(context);
                    HttpRequestReader.TryGetPersonFrom(context.Request, out PersonViewModel person);
                    await controller.Add(person);
                });

            routes[personsEndPoint].Add(
                HttpMethod.Put,
                async (context) =>
                {
                    var controller = controllerFactory.GetPersonsController(context);
                    HttpRequestReader.TryGetPersonFrom(context.Request, out PersonViewModel person);
                    await controller.Update(person);
                });
        }

        public IEnumerable<Uri> GetEndPoints()
        {
            return routes.Keys;
        }

        public bool TryGetEndPointFor(HttpListenerRequest request, out Action<HttpListenerContext> endPoint)
        {
            var result = false;
            endPoint = null;
  
            var httpMethod = new HttpMethod(request.HttpMethod);
            if (routes.TryGetValue(request.Url, out Dictionary<HttpMethod, Action<HttpListenerContext>> methods)
                && methods.TryGetValue(httpMethod, out endPoint))
            {
                result = true;
            }

            return result;
        }
    }
}