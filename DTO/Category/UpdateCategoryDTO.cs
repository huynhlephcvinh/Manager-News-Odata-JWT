using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Category
{
    public class UpdateCategoryDTO
    {
        public short? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public string? CategoryDesciption { get; set; }

        public short? ParentCategoryId { get; set; }
    }
}
