using System.ComponentModel.DataAnnotations;

namespace Sugamta.Web.Models
{
    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public string CountryCode { get; set; }
    }
}
