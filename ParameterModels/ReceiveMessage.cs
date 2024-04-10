using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApplication2.ParameterModels
{
    public class ReceiveMessage
    { 
        public string ReceiverID { get; set; }
        public string Content { get; set; }

        // Navigation property
        //public ICollection<Message> Messages { get; set; }
    }
}
