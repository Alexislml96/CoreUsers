using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Users.Models
{
    public class LoginModel : BaseModel
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
    }

    public class LoginMinModel
    {
        public string Nick { get; set; }
        public string Pass { get; set; }
    }
}
