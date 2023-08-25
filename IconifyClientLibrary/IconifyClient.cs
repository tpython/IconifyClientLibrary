using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using IconifyClientLibrary.API;

namespace IconifyClientLibrary
{
    public class IconifyClient
    {
        private static readonly HttpClient http = new HttpClient();
        public string URL { get; set; } = @"https://api.iconify.design";

        internal HttpClient GetHttpClient() => http;

        public async Task<Icons> Search(string query, int? limit = null, int? start = null, string prefix = null, string category = null)
        {
            var uriQry = new Dictionary<string, string>()
            {
                { "query", query },
                { "limit", limit?.ToString() },
                { "start", start?.ToString() },
                { "prefix", prefix },
                { "category", category }
            };

            UriBuilder uri = new UriBuilder(URL);
            uri.Path = "search";
            uri.Query = string.Join("&", uriQry.Where(x => x.Value != null).Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var jsonResult = await http.GetFromJsonAsync<SearchResultIcons>(uri.Uri, options);

            var icons = new Icons();
            var collections = new Dictionary<string, Collection>();

            if (jsonResult != null && jsonResult.Icons != null)
            {
                foreach (var pair in jsonResult.Collections)
                {
                    var c = pair.Value;
                    if (c != null) {
                        string id = pair.Key;
                        if (!collections.ContainsKey(id.ToLower()))
                        {
                            var col = new Collection
                            {
                                ID = id,
                                Name = c.name,
                                Total = c.total,
                                Version = c.version,
                                Author = c.author,
                                License = c.license,
                                Samples = c.samples,
                                Height = c.height,
                                DisplayHeight = c.displayHeight,
                                Category = c.category,
                                Tags = c.tags,
                                Palette = c.palette,
                            };
                            collections[id.ToLower()] = col;
                        }
                    }
                }

                foreach (var item in jsonResult.Icons)
                {
                    var itemTokens = item.Split(':');
                    var collectionName = itemTokens[0];
                    var itemName = itemTokens[1];
                    var icon = new Icon(this)
                    {
                        Name = itemName,
                        CollectionID = collectionName,
                        Collection = collections[collectionName.ToLower()]
                    };
                    icons.Add(icon);
                }
            }

            return icons;
        }

        public Icon GetIcon(string collectionName, string name)
        {
            var icon = new Icon(this)
            {
                Name = name,
                CollectionID = collectionName,
            };
            return icon;
        }
    }
}
