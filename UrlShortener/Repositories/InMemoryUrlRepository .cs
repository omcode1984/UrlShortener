﻿using System.Collections.Generic;
using UrlShortener.Models;
using UrlShortener.Repositories.Interfaces;
using URLShortener.Controllers;

namespace UrlShortener.Repositories
{
    public class InMemoryUrlRepository : IUrlRepository
    {
        // In-memory storage for URL mappings
        private readonly Dictionary<string, UrlMapping> _urlMappings;
        private readonly ILogger<InMemoryUrlRepository> _logger;

        public InMemoryUrlRepository(ILogger<InMemoryUrlRepository> logger)
        {
            _logger = logger;
            _urlMappings= new Dictionary<string, UrlMapping>();
        }
        // Adds a URL mapping to the repository
        public void AddUrlMapping(string shortId, UrlMapping urlMapping)
        {
            if (!_urlMappings.ContainsKey(shortId))
            {
                _urlMappings[shortId] = urlMapping;
            }
        }

        // Retrieves a URL mapping by short ID
        public UrlMapping? GetUrlMapping(string shortId)
        {
            _urlMappings.TryGetValue(shortId, out var urlMapping);            
            return urlMapping;
        }
        
    }

}
