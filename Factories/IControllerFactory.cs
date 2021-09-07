using HTTPServer.Controllers;
using System.Net;

namespace HTTPServer.Factories
{
    public interface IControllerFactory
    {
        public PersonsController GetPersonsController(HttpListenerContext context);
    }
}
