namespace Sugamta.Web.Models.UserDTOs
{
    public class UserUpdateDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int IsDeleted { get; set; } = 0;
        public int RoleId { get; set; }
    }
}
