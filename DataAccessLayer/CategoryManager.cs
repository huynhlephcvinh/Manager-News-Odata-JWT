using BusinessObject;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class CategoryManager
    {
        //using singleton pattern
        private static CategoryManager instance = null;
        public static readonly object instanceLock = new object();
        private CategoryManager() { }
        public static CategoryManager Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CategoryManager();
                    }
                    return instance;
                }
            }
        }
        //------------------------------------------

        public Category GetCategory(int id)
        {
            try
            {
                Category category = new Category();
                using (var context = new FunewsManagementFall2024Context())
                {
                    category = context.Categories.FirstOrDefault(x => x.CategoryId == id);
                    if(category == null)
                    {
                        throw new Exception("category is empty");
                    }

                }
                return category;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Category> GetAllCategory()
        {
            IEnumerable<Category> list = new List<Category>();
            using (var context = new FunewsManagementFall2024Context())
            {
                list = context.Categories.Include(x=>x.ParentCategory).ToList();
            }
            return list;
        }

        public void AddCategory(Category category)
        {
            try
            {
                using (var context = new FunewsManagementFall2024Context())
                {
                    context.Categories.Add(category);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdaterCategory(Category category)
        {
            try
            {
                using (var context = new FunewsManagementFall2024Context())
                {
                    Category oldCategory = context.Categories.FirstOrDefault(x => x.CategoryId == category.CategoryId);
                    if (oldCategory != null)
                    {
                        oldCategory.CategoryName = category.CategoryName != null ? category.CategoryName : oldCategory.CategoryName;
                        oldCategory.CategoryDesciption = category.CategoryDesciption != null ? category.CategoryDesciption : oldCategory.CategoryDesciption;
                        oldCategory.ParentCategoryId = category.ParentCategoryId != null ? category.ParentCategoryId : oldCategory.ParentCategoryId;
                        context.SaveChanges();
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool CategoryExists(short id)
        {

            using (var context = new FunewsManagementFall2024Context())
            {
                return (context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
            }


        }

        public int DeleteCategory(short id)
        {
            try
            {
                using (var context = new FunewsManagementFall2024Context())
                {
                    List<NewsArticle> newsArticles = context.NewsArticles.Where(x => x.CategoryId == id).ToList();
                    if (newsArticles.Any())
                    {
                        return 0;
                    }
                    else
                    {
                        Category category = context.Categories.Where(x=>x.IsActive == true).FirstOrDefault(x => x.CategoryId == id);
                        if (category != null)
                        {
                            category.IsActive = false;                          
                            context.SaveChanges();
                            return 1;
                        }
                        else { return 2; }
                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
