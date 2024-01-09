using System.ComponentModel.DataAnnotations;

namespace Sugamta.Web.Models
{
    public class LinkGeneration
    {
        public string RegistrationLink { get; set; }
        public DateTime LinkGenerationDate { get; set; }
        public int IsActive { get; set; } = 1;
    }
}
