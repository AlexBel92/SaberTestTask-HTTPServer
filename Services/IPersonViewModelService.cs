using DataStore.Core;
using HTTPServer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HTTPServer.Services
{
    public interface IPersonViewModelService
    {
        Task<int> AddPersonAsync(PersonViewModel personViewModel);
        Task<IEnumerable<PersonViewModel>> GetPersonsAsync();
        Task<bool> UpdatePersonAsync(PersonViewModel personViewModel);
    }
}