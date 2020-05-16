﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UserService.API
{
    public class CSRegisterRequest
    {
        [Required]
        public virtual string UserName { get; set; }

        [Required, DataType(DataType.Password)]
        public virtual string Password { get; set; }

        [Required, Phone, DataType(DataType.PhoneNumber)]
        public virtual string Phone { get; set; }

        [Required, EmailAddress, DataType(DataType.EmailAddress)]
        public virtual string Email { get; set; }
    }
}
