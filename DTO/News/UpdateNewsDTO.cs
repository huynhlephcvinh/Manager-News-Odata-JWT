using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.News
{
    public class UpdateNewsDTO
    {
        public string? NewsArticleId { get; set; }
        public string? NewsTitle { get; set; }
        public string Headline { get; set; }
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }

    }
}
