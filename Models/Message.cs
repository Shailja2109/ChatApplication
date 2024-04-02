using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Message
{
    public Guid? Sender { get; set; }

    public Guid Reciever { get; set; }

    public string? Message1 { get; set; }

    public Guid MessageId { get; set; }

    public TimeOnly? Date { get; set; }
}

public partial class MessageDTO
{
    public Guid Reciever { get; set; }
    public string? Message1 { get; set; }
    public Guid? Sender { get; set; }

}
