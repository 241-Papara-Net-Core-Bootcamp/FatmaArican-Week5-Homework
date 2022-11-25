using System.Collections.Generic;

namespace Week5.Clients
{ 
public interface IArticleHttpClient
{
    List<PostModel> FetchPosts();
}
}

