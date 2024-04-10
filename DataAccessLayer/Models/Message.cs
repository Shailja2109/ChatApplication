using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApplication2.DataAccessLayer.Models
{
    public class Message
    {
        [Key]
        public int MessageID { get; set; }     
        public string Id { get; set; } //SenderID Of User
        public string ReceiverID { get; set; } //ReceiverID of User
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        //public UserRegistration Sender { get; set; }
        //public UserRegistration Receiver { get; set; }
    }
}
