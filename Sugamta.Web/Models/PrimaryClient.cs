namespace Sugamta.Web.Models
{
    public class PrimaryClient
    {
        public int PrimaryClientId { get; set; }

        public string PrimaryClientEmail { get; set; }
        public string PrimaryClientName { get; set; }
        public string Password { get; set; }
        public DateTime CreationDate { get; set; }
        public int RoleId { get; set; }
        public string AgencyEmail { get; set; }
        public string? OTP { get; set; }
        public DateTime OTPGeneratedDate { get; set; }
        public int IsDeleted { get; set; } = 0;
        public string AgencyName { get; set; }
    }
}
