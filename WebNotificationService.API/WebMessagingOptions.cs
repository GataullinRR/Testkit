using System;
using System.ComponentModel.DataAnnotations;

namespace WebNotificationService.API
{
    public class WebMessagingOptions
    {
        [Required]
        public Uri HubAddress { get; set; }
    }
}
