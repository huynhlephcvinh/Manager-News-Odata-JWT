using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICategoryService
    {
        IEnumerable<Category>? GetAllCategory();
        Category? GetCategoryById(short id);
        void CreateCategory(Category category);
        void UpdateCategory(Category category);
        int DeleteCategory(short id);

    }
    public class CategoryService : ICategoryService
    {
        public ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository) 
        {
            _categoryRepository = categoryRepository;
        }

        public IEnumerable<Category>? GetAllCategory()
        {
            IEnumerable<Category> categories = _categoryRepository.GetAllCategory().Where(x=>x.IsActive == true);
            return categories;
        }

        public Category? GetCategoryById(short id)
        {
            Category category = _categoryRepository.GetCategory(id);
            return category;
        }

        public void CreateCategory(Category category)
        {
             category.IsActive = true;
             _categoryRepository.AddCategory(category);       
        }

        public void UpdateCategory(Category category) { 
            _categoryRepository.UpdateCategory(category);
        }

        public int DeleteCategory(short id) { 
            return _categoryRepository.DeleteCategory(id);
        }


    }
}
