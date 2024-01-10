using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Sugamta.Web.Models
{
    public class UserDetailsCreateOrUpdateDto
    {
        public string Email { get; set; }
        public byte[]? ImageUrl { get; set; }
        public IFormFile? formFile { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdationDate { get; set; }
        public List<State> States { get; set; }
        public List<Country> Countries { get; set; }
    }
}
