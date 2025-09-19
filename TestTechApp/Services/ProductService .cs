using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TestTechApp.Models;

namespace TestTechApp.Services;
public interface IProductService
{
    Task<List<Product>> GetMostExpensiveSmartphonesAsync(string token, int topN);
    Task<Product?> UpdateProductPriceAsync(string token, int productId, decimal newPrice);
}

public class ProductService : IProductService
{
    private readonly HttpClient _http;
    private readonly ILogger<ProductService> _logger;

    public ProductService(HttpClient http, ILogger<ProductService> logger)
    {
        _http = http;
        _logger = logger;
    }

    private void AddBearerToken(string token)
    {
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<Product>> GetMostExpensiveSmartphonesAsync(string token, int topN)
    {
        AddBearerToken(token);
        var response = await _http.GetAsync("https://dummyjson.com/auth/products/category/smartphones");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var products = json.GetProperty("products")
                           .EnumerateArray()
                           .Select(p => new Product
                           {
                               Id = p.GetProperty("id").GetInt32(),
                               Title = p.GetProperty("title").GetString() ?? "",
                               Brand = p.GetProperty("brand").GetString() ?? "",
                               Price = p.GetProperty("price").GetDecimal()
                           })
                           .OrderByDescending(p => p.Price)
                           .Take(topN)
                           .ToList();

        _logger.LogInformation("Fetched {Count} most expensive smartphones", products.Count);
        return products;
    }

    public async Task<Product?> UpdateProductPriceAsync(string token, int productId, decimal newPrice)
    {
        AddBearerToken(token);
        _logger.LogInformation("Updating product {Id} to price {Price}", productId, newPrice);
        var response = await _http.PutAsJsonAsync(
            $"https://dummyjson.com/products/{productId}",
            new UpdatePriceRequest { Price = newPrice });

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Product>();
    }
}
