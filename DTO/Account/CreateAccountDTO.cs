using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Account
{
    public class CreateAccountDTO
    {
        [Required]
        public string? AccountName { get; set; }
        [Required]
        [EmailAddress]
        public string? AccountEmail { get; set; }
        [Required]
        public int? AccountRole { get; set; }
        [Required]
        public string? AccountPassword { get; set; }
    }
}
