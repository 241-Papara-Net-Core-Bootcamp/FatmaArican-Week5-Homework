using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;

namespace Week5.Clients
{
public class ArticleHttpClientImp : IArticleHttpClient
{
    private readonly IConfiguration _configuration;

    public ArticleHttpClientImp(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<PostModel> FetchPosts()
    {
        var basePath = _configuration.GetValue<string>("PostClientUrl");
        var client = new HttpClient();
        var response = client.GetAsync(string.Format($"{basePath}/posts")).Result;

        if (response.IsSuccessStatusCode)
            return response.Content.ReadFromJsonAsync<List<PostModel>>().Result;

        throw new Exception($"error from client : {response.Content.ReadAsStringAsync().Result}");
    }
}
}