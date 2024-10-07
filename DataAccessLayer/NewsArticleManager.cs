using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class NewsArticleManager
    {
        //using singleton pattern
        private static NewsArticleManager instance = null;
        public static readonly object instanceLock = new object();
        private NewsArticleManager() { }
        public static NewsArticleManager Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new NewsArticleManager();
                    }
                    return instance;
                }
            }
        }
        //------------------------------------------

        public IEnumerable<NewsArticle> GetAllNewsArticle()
        {
            IEnumerable<NewsArticle> list = new List<NewsArticle>();
            using (var context = new FunewsManagementFall2024Context())
            {
                list = context.NewsArticles.Where(x => x.NewsStatus == true)
                    .Include(na => na.Tags)
                    .Include(ca=>ca.Category)
                    .Include(ac => ac.CreatedBy)
                    .OrderByDescending(n => n.CreatedDate)
                    .ToList();
            }
            return list;
        }

        public void AddNews(NewsArticle news, List<int>? idTag)
        {
            try
            {
                using (var context = new FunewsManagementFall2024Context())
                {
                    byte[] randomBytes = new byte[5]; // 5 bytes sẽ tạo ra 8 ký tự sau khi mã hóa Base64
                    using (var rng = new RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(randomBytes);
                    }

                    // Mã hóa Base64 và chỉ lấy 7 ký tự đầu tiên
                    string base64String = Convert.ToBase64String(randomBytes)
                                                .Replace('+', '-') // Thay đổi ký tự để Base64 URL-safe
                                                .Replace('/', '_') // Thay đổi ký tự để Base64 URL-safe
                                                .Substring(0, 7);
                    news.NewsArticleId = base64String.ToString();

                    // Add the new NewsArticle
                    context.NewsArticles.Add(news);
                    context.SaveChanges(); // Save the NewsArticle first to generate the ID

                    // Add tags if any were provided
                    if (idTag != null && idTag.Count > 0)
                    {
                        // Load all tags for the given tag IDs
                        var tagsToAdd = context.Tags.Where(t => idTag.Contains(t.TagId)).ToList();

                        // Add each tag to the NewsArticle's Tags collection
                        foreach (var tag in tagsToAdd)
                        {
                            news.Tags.Add(tag); // Assuming `Tags` is the navigation property
                        }

                        context.SaveChanges(); // Save changes to update the many-to-many relationship
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding news: {ex.Message}");
            }
        }


        public void AddTagToNews(string? newsArticleId, Tag tag)
        {
            try
            {
                using (var context = new FunewsManagementFall2024Context())
                {
                    var newsArticle = context.NewsArticles.Include(na => na.Tags).FirstOrDefault(na => na.NewsArticleId == newsArticleId);
                    if (newsArticle != null)
                    {
                        var existingTag = context.Tags.FirstOrDefault(x => x.TagId == tag.TagId);
                        if (existingTag != null)
                        {
                            newsArticle.Tags.Add(existingTag);
                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void RemoveTag(NewsArticle? newsArticle, Tag tagtoremove)
        {
            try
            {
                using (var context = new FunewsManagementFall2024Context())
                {
                    var oldnewsArticle = context.NewsArticles.Include(na => na.Tags).FirstOrDefault(na => na.NewsArticleId == newsArticle.NewsArticleId);
                    if (oldnewsArticle != null)
                    {
                        var existingTag = context.Tags.FirstOrDefault(x => x.TagId == tagtoremove.TagId);
                        if (existingTag != null && oldnewsArticle.Tags.Contains(existingTag))
                        {
                            oldnewsArticle.Tags.Remove(existingTag);
                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void UpdaterNews(NewsArticle news)
        {
            try
            {
                using (var context = new FunewsManagementFall2024Context())
                {
                    NewsArticle? oldNews = context.NewsArticles.FirstOrDefault(x => x.NewsArticleId == news.NewsArticleId);
                    if (oldNews != null)
                    {
                        oldNews.NewsTitle = news.NewsTitle != null ? news.NewsTitle : oldNews.NewsTitle;
                        oldNews.NewsSource = news.NewsSource != null ? news.NewsSource : oldNews.NewsSource;
                        oldNews.Headline = news.Headline != null ? news.Headline : oldNews.Headline;
                        oldNews.NewsContent = news.NewsContent != null ? news.NewsContent : oldNews.NewsContent;
                        oldNews.CategoryId = news.CategoryId;
                        oldNews.UpdatedById = news.UpdatedById;
                        oldNews.ModifiedDate = news.ModifiedDate;
                        context.SaveChanges();
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool NewsArticleExists(string id)
        {
            try
            {
                using (var context = new FunewsManagementFall2024Context())
                {
                    return (context.NewsArticles?.Any(e => e.NewsArticleId == id)).GetValueOrDefault();
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public int DeleteNews(string id)
        {
            try
            {
                using (var context = new FunewsManagementFall2024Context())
                {
                    NewsArticle? news = context.NewsArticles.FirstOrDefault(x => x.NewsArticleId == id);
                    if (news != null)
                    {
                        news.NewsStatus = false;
                        context.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<NewsArticle> LoadNewsArticlesReportStatistics(DateTime startDate, DateTime endDate)
        {
            try
            {
                IEnumerable<NewsArticle> listReport = new List<NewsArticle>();
                using (var context = new FunewsManagementFall2024Context())
                {
                    listReport = context.NewsArticles.Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .OrderByDescending(n => n.CreatedDate)
                .ToList();
                }
                return listReport;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
