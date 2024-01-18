using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Sugamta.Web.Models.PrimaryClientDTOs
{
    public class PrimaryClientRegisterDto
    {
        public string PrimaryClientEmail { get; set; }
        public string PrimaryClientName { get; set; }
        public string Password { get; set; }
        public DateTime CreationDate { get; set; }
        public int RoleId { get; set; }
        public string AgencyEmail { get; set; }
        public string? AgencyName { get; set; }
        public string? OTP { get; set; }
        public int IsDeleted { get; set; } = 0;
    }
}
