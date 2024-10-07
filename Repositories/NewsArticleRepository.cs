using BusinessObject;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface INewsArticleRepository
    {
        public IEnumerable<NewsArticle> GetAllNewsArticle();
        public void AddNews(NewsArticle newsArticle, List<int>? idTag);
        public void AddTagToNews(string? newsArticleId, Tag tag);
        public void UpdateNews(NewsArticle newsArticle);
        public int DeleteNews(string id);
        public void RemoveTag(NewsArticle? newsArticle, Tag tag);
        public bool NewsArticleExists(string id);
        public IEnumerable<NewsArticle> GetAllListReportStatistic(DateTime startTime, DateTime endTime);
    }
    public class NewsArticleRepository : INewsArticleRepository
    {
        public IEnumerable<NewsArticle> GetAllNewsArticle() => NewsArticleManager.Instance.GetAllNewsArticle();
        public void AddNews(NewsArticle newsArticle, List<int>? idTag) => NewsArticleManager.Instance.AddNews(newsArticle, idTag);

        public void AddTagToNews(string? newsArticleId, Tag tag) => NewsArticleManager.Instance.AddTagToNews(newsArticleId, tag);

        public void UpdateNews(NewsArticle newsArticle) => NewsArticleManager.Instance.UpdaterNews(newsArticle);

        public int DeleteNews(string id) => NewsArticleManager.Instance.DeleteNews(id);

        public void RemoveTag(NewsArticle? newsArticle, Tag tag) => NewsArticleManager.Instance.RemoveTag(newsArticle, tag);

        public IEnumerable<NewsArticle> GetAllListReportStatistic(DateTime startTime, DateTime endTime) => NewsArticleManager.Instance.LoadNewsArticlesReportStatistics(startTime, endTime);

        public bool NewsArticleExists(string id) => NewsArticleManager.Instance.NewsArticleExists(id);
    }
}
