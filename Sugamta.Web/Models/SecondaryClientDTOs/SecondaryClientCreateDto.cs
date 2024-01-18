using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Sugamta.Web.Models.SecondaryClientDTOs
{
    public class SecondaryClientCreateDto
    {
        public string SecondaryClientEmail { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public int RoleId { get; set; }

        public string PrimaryClientEmail { get; set; }


    }
}
