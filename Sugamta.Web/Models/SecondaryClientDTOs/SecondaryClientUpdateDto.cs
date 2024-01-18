namespace Sugamta.Web.Models.SecondaryClientDTOs
{
    public class SecondaryClientUpdateDto
    {
        public string SecondaryClientEmail { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

       
        public int RoleId { get; set; }
       
        public string PrimaryClientEmail { get; set; }
    }
}
