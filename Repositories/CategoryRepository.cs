using BusinessObject;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICategoryRepository
    {
        public Category GetCategory(short id);
        public IEnumerable<Category> GetAllCategory();
        public void AddCategory(Category category);
        public void UpdateCategory(Category category);
        public int DeleteCategory(short id);
        public bool CategoryExists(short id);

    }
    public class CategoryRepository : ICategoryRepository
    {
        public Category GetCategory(short id) => CategoryManager.Instance.GetCategory(id);

        public IEnumerable<Category> GetAllCategory() => CategoryManager.Instance.GetAllCategory();
        public void AddCategory(Category category) => CategoryManager.Instance.AddCategory(category);

        public void UpdateCategory(Category category) => CategoryManager.Instance.UpdaterCategory(category);
        public bool CategoryExists(short id) => CategoryManager.Instance.CategoryExists(id);

        public int DeleteCategory(short id) => CategoryManager.Instance.DeleteCategory(id);

    }
}
