using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Yêu cầu nhập email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập password")]
        public string Password { get; set; }
    }
}
