using DataStore.Core;
using HTTPServer.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HTTPServer.Services
{
    public class PersonViewModelService : IPersonViewModelService
    {
        private readonly IRepository repository;

        public PersonViewModelService(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<PersonViewModel>> GetPersonsAsync()
        {
            var corePersons = await repository.GetPersonsAsync();

            return corePersons.Select(p => new PersonViewModel()
            {
                Id = p.Id,
                FirstName = p.FirstName,
                MiddleName = p.MiddleName,
                LastName = p.LastName,
                DateBirth = p.DateBirth.Date
            });
        }

        public async Task<int> AddPersonAsync(PersonViewModel personViewModel)
        {
            var corePerson = MapPersonFrom(personViewModel);
            corePerson.Id = 0;

            return await repository.AddPersonAsync(corePerson);
        }

        private static Person MapPersonFrom(PersonViewModel personViewModel)
        {
            return new Person()
            {
                Id = personViewModel.Id,
                FirstName = personViewModel.FirstName,
                MiddleName = personViewModel.MiddleName,
                LastName = personViewModel.LastName,
                DateBirth = personViewModel.DateBirth.Date
            };
        }

        public async Task<bool> UpdatePersonAsync(PersonViewModel personViewModel)
        {
            var corePerson = MapPersonFrom(personViewModel);
            var result = false;
            try
            {
                await repository.UpdatePersonAsync(corePerson);
                result = true;
            }
            catch (UpdatePersonException)
            {

            }

            return result;
        }

    }
}
