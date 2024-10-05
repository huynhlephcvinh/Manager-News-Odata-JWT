using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.News
{
    public class TagToNewsDTO
    {
        [Required]
        public int TagId { get; set; }
        [Required]
        public string? NewsArticleId { get; set; }
    }
}
