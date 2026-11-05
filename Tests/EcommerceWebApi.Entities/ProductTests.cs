using System.ComponentModel.DataAnnotations;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Utilities;
using Xunit;

namespace EcommerceWebApi.Entities.Tests
{
    public class ProductTests
    {
        [Fact]
        public void Product_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var product = new Product();

            // Assert
            Assert.Equal(0, product.Id);
            Assert.Equal(0f, product.Price);
            Assert.Equal(0f, product.Rating);
            Assert.Equal(0, product.Quantity);
        }

        [Fact]
        public void Product_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Title = "Test Product",
                Price = 99.99f,
                Rating = 4.5f,
                Brand = "TestBrand",
                Category = "Electronics",
                Thumbnail = new Uri("https://example.com/image.jpg"),
                Quantity = 10
            };

            // Assert
            Assert.Equal(1, product.Id);
            Assert.Equal("Test Product", product.Title);
            Assert.Equal(99.99f, product.Price);
            Assert.Equal(4.5f, product.Rating);
            Assert.Equal("TestBrand", product.Brand);
            Assert.Equal("Electronics", product.Category);
            Assert.Equal(new Uri("https://example.com/image.jpg"), product.Thumbnail);
            Assert.Equal(10, product.Quantity);
        }

        [Fact]
        public void Product_RatingRange_ValidValue()
        {
            // Arrange
            var product = new Product { Rating = 3.5f };
            var context = new ValidationContext(product) { MemberName = "Rating" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(product.Rating, context, results);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Product_RatingRange_InvalidValue_AboveMax()
        {
            // Arrange
            var product = new Product { Rating = 6.0f };
            var context = new ValidationContext(product) { MemberName = "Rating" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(product.Rating, context, results);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Product_RatingRange_InvalidValue_BelowMin()
        {
            // Arrange
            var product = new Product { Rating = -1.0f };
            var context = new ValidationContext(product) { MemberName = "Rating" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(product.Rating, context, results);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Product_TitleHasSearchableAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty("Title");

            // Act
            var attribute = propertyInfo?.GetCustomAttributes(typeof(SearchableAttribute), false);

            // Assert
            Assert.NotNull(attribute);
            Assert.NotEmpty(attribute);
        }

        [Fact]
        public void Product_PriceHasSortableAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty("Price");

            // Act
            var attribute = propertyInfo?.GetCustomAttributes(typeof(SortableAttribute), false);

            // Assert
            Assert.NotNull(attribute);
            Assert.NotEmpty(attribute);
        }

        [Fact]
        public void Product_BrandHasSearchableAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty("Brand");

            // Act
            var attribute = propertyInfo?.GetCustomAttributes(typeof(SearchableAttribute), false);

            // Assert
            Assert.NotNull(attribute);
            Assert.NotEmpty(attribute);
        }

        [Fact]
        public void Product_QuantityHasSortableAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty("Quantity");

            // Act
            var attribute = propertyInfo?.GetCustomAttributes(typeof(SortableAttribute), false);

            // Assert
            Assert.NotNull(attribute);
            Assert.NotEmpty(attribute);
        }

        [Fact]
        public void Product_RatingHasSortableAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty("Rating");

            // Act
            var attribute = propertyInfo?.GetCustomAttributes(typeof(SortableAttribute), false);

            // Assert
            Assert.NotNull(attribute);
            Assert.NotEmpty(attribute);
        }
    }
}
