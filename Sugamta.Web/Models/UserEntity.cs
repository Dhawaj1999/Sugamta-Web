using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Sugamta.Web.Models
{
	public class UserEntity
	{
		public int UserID { get; set; }

		public string Email { get; set; }
		public string Name { get; set; }
		//public string Image { get; set; }
	
		public string Password { get; set; }

        public int RoleId { get; set; }
		public DateTime CreationDate { get; set; }
		public string CreatedBy { get; set; }
		public int IsDeleted { get; set; } = 0;
        public string? OTP { get; set; }
    }
}
