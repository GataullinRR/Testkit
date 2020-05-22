using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MessageHub
{
    public class MessageConsumerOptions
    {
        [Required]
        public string GroupId { get; set; }
    }
}
