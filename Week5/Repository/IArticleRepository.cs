using System.Collections.Generic;
using Week5.Repository.Entities;

public interface IArticleRepository
{
    Article GetArticleById(int id);
    List<Article> FetchArticles();
    bool SaveOrUpdate(int id, int userId, string title, string body);
}