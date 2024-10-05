using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Category
{
    public class CreateCategoryDTO
    {
        [Required(ErrorMessage = "Nhập tên danh mục")]
        public string? CategoryName { get; set; }
        [Required(ErrorMessage = "Nhập miêu tả danh mục")]
        public string? CategoryDesciption { get; set; }

        public short? ParentCategoryId { get; set; }

    }
}
