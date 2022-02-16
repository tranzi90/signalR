using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class AuthModel
    {
        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 5)]
        public string Login { get; set; }
        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Password { get; set; }
    }
}
