using System.ComponentModel.DataAnnotations;

namespace ChatApplication2.DataAccessLayer.Models
{
    public class UserReg
    {
        [Key]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
}
