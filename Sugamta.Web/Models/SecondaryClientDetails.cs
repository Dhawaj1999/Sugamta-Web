using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sugamta.Web.Models
{
    public class SecondaryClientDetails
    {
        public List<State> States { get; set; }
        public List<Country> Countries { get; set; }
        public string SecondaryClientEmail { get; set; }
        public IFormFile? formFile { get; set; }
      //  public string Name { get; set; }

        public string Address { get; set; }
       // public string Gender { get; set; }
        //public int Age { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string CreationDate { get; set; }
        public DateTime UpdationDate { get; set; }
        public byte[]? ImageUrl { get; set; }
       
    }
}
