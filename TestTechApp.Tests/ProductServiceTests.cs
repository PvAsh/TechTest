using TestTechApp.Models;

namespace  TestTechApp.Tests;
public class ProductServiceTests
{
    [Fact]
    public void IncreasePrice_CalculatesCorrectly()
    {
        var phones = new List<Product>
        {
            new Product { Id = 1, Brand = "BrandA", Title = "PhoneA", Price = 100 },
            new Product { Id = 2, Brand = "BrandB", Title = "PhoneB", Price = 200 }
        };

        decimal percent = 10; // increase 10%

        for (int i = 0; i < phones.Count; i++)
        {
            phones[i].Price += phones[i].Price * percent / 100;
        }

        Assert.Equal(110, phones[0].Price);
        Assert.Equal(220, phones[1].Price);
    }
}