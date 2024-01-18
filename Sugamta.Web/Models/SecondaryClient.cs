using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text.Json.Serialization;

namespace Sugamta.Web.Models
{
    public class SecondaryClient
    {


        public string SecondaryClientEmail { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public DateTime? CreationDate { get; set; }
        public int? RoleId { get; set; }
        [ForeignKey("RoleId")]
        [JsonIgnore]
        [ValidateNever]
       
        public string? OTP { get; set; }
        public int? IsDeleted { get; set; } = 0;

        // public int IsDeleted { get; set; } = 0;
        [ForeignKey("PrimaryClient")]
        public string PrimaryClientEmail { get; set; }
        public string RoleType { get; set; }

    }
}
