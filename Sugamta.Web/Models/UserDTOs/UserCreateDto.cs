namespace Sugamta.Web.Models.UserDTOs
{
    public class UserCreateDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public DateTime CreationDate { get; set; }
        public int IsDeleted { get; set; } = 0;
        public int RoleId { get; set; }
        public string? OTP { get; set; }
    }
}
