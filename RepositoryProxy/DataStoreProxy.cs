using DataStore.Core;
using System;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace HTTPServer
{
    class DataStoreProxy : IRepository
    {
        private readonly TcpServerSettings settings;

        public DataStoreProxy(TcpServerSettings settings)
        {
            this.settings = settings;
        }

        public async Task<int> AddPersonAsync(Person person)
        {
            using var serverClient = CreateServerClient();
            var addedPersonId = 0;

            try
            {
                serverClient.Connect(settings.GetIPAddress(), settings.AddPortNumber);

                var personJson = JsonSerializer.Serialize(person);
                await serverClient.WriteAsync(personJson);
                var stringReceived = await ReadStringFrom(serverClient);

                var response = JsonSerializer.Deserialize<DataStoreServerResponse>(stringReceived);
                if (response.IsSuccess && int.TryParse(response.Answer, out int receivedId))
                    addedPersonId = receivedId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return addedPersonId;
        }

        private static ServerClient CreateServerClient()
        {
            return new ServerClient();
        }

        private async Task<string> ReadStringFrom(ServerClient client)
        {
            var sb = new StringBuilder();
            await foreach (var partOfString in client.ReadAsync())
                sb.Append(partOfString);
            var stringReceived = sb.ToString();

            Console.WriteLine($"String received: {stringReceived}");

            return stringReceived;
        }

        public async Task<IEnumerable<Person>> GetPersonsAsync()
        {
            using var serverClient = CreateServerClient();
            var receivedPersons = Enumerable.Empty<Person>();

            try
            {
                serverClient.Connect(settings.GetIPAddress(), settings.GetPortNumber);

                await serverClient.WriteAsync("\n");
                var stringReceived = await ReadStringFrom(serverClient);

                var response = JsonSerializer.Deserialize<DataStoreServerResponse>(stringReceived);
                receivedPersons = JsonSerializer.Deserialize<IEnumerable<Person>>(response.Answer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return receivedPersons;
        }

        public async Task UpdatePersonAsync(Person person)
        {
            using var serverClient = CreateServerClient();

            try
            {
                serverClient.Connect(settings.GetIPAddress(), settings.UpdatePortNumber);

                var personJson = JsonSerializer.Serialize(person);
                await serverClient.WriteAsync(personJson);
                var stringReceived = await ReadStringFrom(serverClient);
                
                var response = JsonSerializer.Deserialize<DataStoreServerResponse>(stringReceived);

                if (!response.IsSuccess)
                    throw new UpdatePersonException(response.Error);
            }
            catch (UpdatePersonException)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Dispose()
        {
            
        }
    }
}
