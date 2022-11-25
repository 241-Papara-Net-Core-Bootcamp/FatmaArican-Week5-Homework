using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

using Week5.Clients;
using Week5.Repository.Entities;

namespace Week5.Controllers
{
    [Route("article")]
    public class HomeController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly IArticleHttpClient _articleHttpClient;

        public HomeController(IArticleRepository articleRepository, IMemoryCache memoryCache, IConfiguration configuration,
            IArticleHttpClient articleHttpClient)
        {
            _articleRepository = articleRepository;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _articleHttpClient = articleHttpClient;
        }

        [HttpGet("startPostFetchJob")]
        public IActionResult StartPostFetchJob()
        {
            RecurringJob.AddOrUpdate(
                "fetchPostJob",
                () => FetchPostAndMigrate(), _configuration.GetValue<string>("CronExpression"));
            return Ok("");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void FetchPostAndMigrate()
        {
            var posts = _articleHttpClient.FetchPosts();

            foreach (var item in posts)
            {
                _articleRepository.SaveOrUpdate(item.Id, item.UserID, item.Title, item.Body);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetArticle([FromRoute] int id)
        {
            if (_memoryCache.TryGetValue(id.ToString(), out Article value))
                return Ok(value);

            var article = _articleRepository.GetArticleById(id);

            _memoryCache.Set(id.ToString(), article, TimeSpan.FromHours(1));

            return Ok(article);
        }

        [HttpGet("search")]
        public IActionResult Search()
        {
            if (_memoryCache.TryGetValue("posts", out List<Article> values))
                return Ok(values);

            var articles = _articleRepository.FetchArticles();

            _memoryCache.Set("posts", articles, TimeSpan.FromHours(1));

            return Ok(articles);
        }
    }
}