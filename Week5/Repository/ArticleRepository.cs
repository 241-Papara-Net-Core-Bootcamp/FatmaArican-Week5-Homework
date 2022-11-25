using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using Week5.Repository.Entities;

namespace Week5.Repository
{

    public class ArticleRepository : IArticleRepository
    {
        private readonly IConfiguration _configuration;

        public ArticleRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Article GetArticleById(int id)
        {
            using SqlConnection con = new SqlConnection(_configuration.GetValue<string>("ArticleDatabaseConnection"));
            con.Open();

            var foundItem =
                con.QueryFirstOrDefault<Article>(
                    "select Id,UserId,Subject as Title,Data as Content from Article where Id=" + id);

            return foundItem;
        }

        public List<Article> FetchArticles()
        {
            using SqlConnection con = new SqlConnection(_configuration.GetValue<string>("ArticleDatabaseConnection"));
            con.Open();
            var foundItems = con.Query<Article>("select Id,UserId,Subject as Title,Data as Content from Article");
            return foundItems.ToList();
        }


        public bool SaveOrUpdate(int id, int userId, string title, string body)
        {
            var article = GetArticleById(id);
            if (article == null)
                return insertArticle(id, userId, title, body);

            return updateArticle(id, userId, title, body);
        }

        private bool updateArticle(int id, int userId, string title, string body)
        {
            using SqlConnection con = new SqlConnection(_configuration.GetValue<string>("ArticleDatabaseConnection"));
            con.Open();
            var effectedRow = con.Execute("update article set UserId=@userId,Subject=@title,Data=@body where Id=@id",
                new { id = id, userId = userId, title = title, body = body });

            return effectedRow > 0;
        }

        private bool insertArticle(int id, int userId, string title, string body)
        {
            using SqlConnection con = new SqlConnection(_configuration.GetValue<string>("ArticleDatabaseConnection"));
            con.Open();
            var effectedRow = con.Execute("insert into article values (@id,@userId,@title,@body)",
                new { id = id, userId = userId, title = title, body = body });

            return effectedRow > 0;
        }
    }
}