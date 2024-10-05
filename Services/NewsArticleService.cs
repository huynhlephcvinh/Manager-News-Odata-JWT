using Azure;
using BusinessObject;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface INewsArticleService
    {
        IEnumerable<NewsArticle>? GetAllNews();
        NewsArticle? GetNewsById(string id);
        void CreateNews(NewsArticle news, SystemAccount account);
        void UpdateNews(NewsArticle news, SystemAccount account);
        int DeleteNews(string id);
        void AddTagToNews(string? newsArticleId, int idTag);
        void RemoveTag(string? newsArticleId, int idTagRemove);
        bool NewsArticleExists(string id);
        IEnumerable<NewsArticle> LoadNewsArticlesReportStatistics(DateTime startDate, DateTime endDate);

    }
    public class NewsArticleService : INewsArticleService
    {
        public INewsArticleRepository _newsArticleRepository;
        public ITagRepository _tagRepository;
        public NewsArticleService(INewsArticleRepository newsArticleRepository, ITagRepository tagRepository)
        {
            _newsArticleRepository = newsArticleRepository;
            _tagRepository = tagRepository;
        }

        public IEnumerable<NewsArticle>? GetAllNews()
        {
            IEnumerable<NewsArticle> news = _newsArticleRepository.GetAllNewsArticle();
            return news;
        }

        public NewsArticle? GetNewsById(string id)
        {
            NewsArticle? news = _newsArticleRepository.GetAllNewsArticle().FirstOrDefault(x=>x.NewsArticleId == id);
            return news;
        }

        public void CreateNews(NewsArticle news, SystemAccount account)
        {
            news.CreatedById = account.AccountId;
            news.UpdatedById = account.AccountId;
            news.CreatedDate = DateTime.UtcNow;
            news.ModifiedDate = DateTime.UtcNow;
            news.NewsStatus = true;
            _newsArticleRepository.AddNews(news);
        }

        public void UpdateNews(NewsArticle news, SystemAccount account)
        {
            news.UpdatedById = account.AccountId;
            news.ModifiedDate = DateTime.UtcNow;
            _newsArticleRepository.UpdateNews(news);
        }

        public int DeleteNews(string id)
        {
            return _newsArticleRepository.DeleteNews(id);
        }

        public void AddTagToNews(string? newsArticleId, int idTag)
        {
            Tag tag = _tagRepository.GetTag(idTag);
            _newsArticleRepository.AddTagToNews(newsArticleId, tag);
        }

        public void RemoveTag(string? newsArticleId, int idTagRemove)
        {
            NewsArticle? newsArticle = _newsArticleRepository.GetAllNewsArticle().FirstOrDefault(x => x.NewsArticleId == newsArticleId);
            Tag tagtoremove = _tagRepository.GetTag(idTagRemove);
            _newsArticleRepository.RemoveTag(newsArticle, tagtoremove);
        }

        public bool NewsArticleExists(string id)
        {
            return _newsArticleRepository.NewsArticleExists(id);
        }

        public IEnumerable<NewsArticle> LoadNewsArticlesReportStatistics(DateTime startDate, DateTime endDate)
        {
            return _newsArticleRepository.GetAllListReportStatistic(startDate, endDate);
        }
    }
}
