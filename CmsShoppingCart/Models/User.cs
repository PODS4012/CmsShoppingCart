﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.Models
{
    public class User
    {
        [Required, MinLength(3, ErrorMessage = "Minimum length is 3")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password), MinLength(4, ErrorMessage = "Minimum length is 4")]
        public string Password { get; set; }
    }
}
