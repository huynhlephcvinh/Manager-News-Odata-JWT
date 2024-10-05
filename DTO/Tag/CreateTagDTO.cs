using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Tag
{
    public class CreateTagDTO
    {
        [Required(ErrorMessage = "Nhập tên tag")]
        public string? TagName { get; set; }

        [Required(ErrorMessage = "Nhập note")]
        public string? Note { get; set; }
    }
}
