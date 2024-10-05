using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.News
{
    public class CreateNewsDTO
    {
        [Required(ErrorMessage = "Nhập title")]
        public string? NewsTitle { get; set; }
        [Required(ErrorMessage = "Nhập headline")]
        public string Headline { get; set; }
        [Required(ErrorMessage = "Nhập content")]
        public string? NewsContent { get; set; }
        [Required(ErrorMessage = "Nhập source")]
        public string? NewsSource { get; set; }

        public short? CategoryId { get; set; }

    }
}
