namespace Sugamta.Web.Models.SecondaryClientDTOs
{
    public class SecondaryClientDetailsEditDtos
    {
        public string SecondaryClientEmail { get; set; }
       // public string? Name { get; set; }
        public string? Address { get; set; }
       // public string? Gender { get; set; }
        //public int? Age { get; set; }
        public string? City { get; set; }
        public int? StateId { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public int? CountryId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AlternatePhoneNumber { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? UpdationDate { get; set; }
        public byte[]? ImageUrl { get; set; }
        public IFormFile? formFile { get; set; }
    }
}
