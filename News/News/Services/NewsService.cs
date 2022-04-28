//#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using News.Models;
//Requires nuget package System.Net.Http.Json

namespace News.Services
{
    public class NewsService
    {
        private DateTime _timeStampNow;
        public EventHandler<string> NewsAvailable;
        private readonly HttpClient _httpClient = new ();
        readonly string apiKey = "faaeddb19bf843498b06c6ad5ee94edf";
        private readonly ConcurrentDictionary<(DateTime, NewsCategory), NewsGroup> _cachedNews = new();
        public async Task<NewsGroup> GetNewsAsync(NewsCategory category)
        {
            _timeStampNow = DateTime.Now;

            // Check to see if there are matching items in the dictionary before calling the api.
            // If a matching item is found, return the value as news, instead of calling the api.
            foreach (var item in _cachedNews)
            {
                if (item.Key.Item2 == category)
                {
                    if (item.Key.Item1.AddMinutes(1) > _timeStampNow)
                        return item.Value;

                    if (!_cachedNews.TryRemove(item.Key, out _))
                        throw new Exception("Couldn't remove cached item from cache.");
                }
            }
            
            // If no matching items found or it has been more than a minute, call the api to get news.
            var newNews = await ReadWebApiAsync(category);
            
            // After calling the api, add a timestamp and category to a conc dict.
            if (!_cachedNews.TryAdd((_timeStampNow, category), newNews))
                throw new Exception("Could not add news to cache.");
            
            return newNews;
        }
        
        private async Task<NewsGroup> ReadWebApiAsync(NewsCategory category)
        {

#if UseNewsApiSample
            NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync(category);
#else
            //https://newsapi.org/docs/endpoints/top-headlines
            var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}&apiKey={apiKey}";

            // Your code here to get live data
            var responseMessage = await _httpClient.GetAsync(uri);
            responseMessage.EnsureSuccessStatusCode();
            var nd = await responseMessage.Content.ReadFromJsonAsync<NewsApiData>();
#endif

            var news = new NewsGroup()
            {
                Category = category,
                Articles = new()
            };

            nd?.Articles.ForEach(x =>
                news.Articles.Add(new NewsItem()
                {
                    DateTime = x.PublishedAt,
                    Title = x.Title,
                    Description = x.Description,
                    Url = x.Url,
                    UrlToImage = x.UrlToImage
                }));

            return news;
        }
    }
}
