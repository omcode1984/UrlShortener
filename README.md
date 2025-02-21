# UrlShortner
1) Technology used in the project - Asp.net core web api, Xunit, Moq
2) Scalability - Currently shorten id is generating randomly which is high possibility it can generate duplicate id. for scale the system, we can hasing algorithm and save it in persistent layer. To make sure unique shorten url, we can do lookup in the permisent layer for make it would be unique
3) caching - we can use Redis or any distributed caching to improve the performance.
4) we can configure autoscaling vertically or horizontally
5) we can apply throttling behaviour by applying the rate limit policy.
