using HTTPServer.Controllers;
using HTTPServer.Services;
using System.Net;

namespace HTTPServer.Factories
{
    public class ControllerFactory : IControllerFactory
    {
        private readonly TcpServerSettings tcpServerSettings;

        public ControllerFactory(TcpServerSettings tcpServerSettings)
        {
            this.tcpServerSettings = tcpServerSettings ?? throw new System.ArgumentNullException(nameof(tcpServerSettings));
        }

        public PersonsController GetPersonsController(HttpListenerContext context)
        {
            return new PersonsController(
                context,
                new PersonViewModelService(
                    new DataStoreProxy(tcpServerSettings)));
        }
    }
}
