using HTTPServer.Model;
using HTTPServer.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace HTTPServer.Controllers
{
    public class PersonsController : ControllerBase
    {
        private readonly IPersonViewModelService modelService;

        public PersonsController(HttpListenerContext context, IPersonViewModelService modelService) : base(context)
        {
            this.modelService = modelService;
        }

        public async Task Get()
        {
            var persons = await modelService.GetPersonsAsync();

            var json = JsonSerializer.Serialize(persons);

            await JsonResult(json);
        }

        public async Task Add(PersonViewModel person)
        {
            var validateErrors = ValidateViewModel(person);
            if (validateErrors.Count > 0)
            {
                await StatusCodeResult(HttpStatusCode.BadRequest, validateErrors.Aggregate((a, b) => a + "\n" + b));
                return;
            }

            person.Id = await modelService.AddPersonAsync(person);

            if (person.Id != 0)
            {
                var json = JsonSerializer.Serialize(person);
                await JsonResult(json);
            }
            else
                await StatusCodeResult(HttpStatusCode.BadRequest, "Error while adding record.");
        }

        public async Task Update(PersonViewModel person)
        {
            var validateErrors = ValidateViewModel(person);
            if (validateErrors.Count > 0)
            {
                await StatusCodeResult(HttpStatusCode.BadRequest, validateErrors.Aggregate((a, b) => a + "\n" + b));
                return;
            }

            if (await modelService.UpdatePersonAsync(person))
            {
                await StatusCodeResult(HttpStatusCode.OK);
            }
            else
                await StatusCodeResult(HttpStatusCode.BadRequest, "Error while updating record.");
        }

        private static List<string> ValidateViewModel(PersonViewModel person)
        {
            var errors = new List<string>();
            if (person is null)
            {
                errors.Add($"Argument {nameof(person)} cannot be null.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(person.FirstName))
                {
                    errors.Add($"Argument {nameof(person.FirstName)} cannot be null or white spaces.");
                }
                if (string.IsNullOrWhiteSpace(person.MiddleName))
                {
                    errors.Add($"Argument {nameof(person.MiddleName)} cannot be null or white spaces.");
                }
                if (string.IsNullOrWhiteSpace(person.LastName))
                {
                    errors.Add($"Argument {nameof(person.LastName)} cannot be null or white spaces.");
                }
                if (person.DateBirth.Date > DateTime.Now.Date)
                {
                    errors.Add($"{nameof(person.DateBirth)} cannot be from the future.");
                }
            }

            return errors;
        }
    }
}
