using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Users.Models
{
    public class RefreshRequest
    {
        [Required]
        public string refreshToken { get; set; }
    }
}
