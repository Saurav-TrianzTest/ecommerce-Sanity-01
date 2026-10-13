using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Xunit;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Utilities;

namespace EcommerceWebApi.Tests.Entities
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
        public void Product_SetId_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Id = 42;

            // Assert
            Assert.Equal(42, product.Id);
        }

        [Fact]
        public void Product_SetTitle_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Title = "Test Product";

            // Assert
            Assert.Equal("Test Product", product.Title);
        }

        [Fact]
        public void Product_SetPrice_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Price = 99.99f;

            // Assert
            Assert.Equal(99.99f, product.Price);
        }

        [Fact]
        public void Product_SetRating_WithValidValue_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Rating = 4.5f;

            // Assert
            Assert.Equal(4.5f, product.Rating);
        }

        [Fact]
        public void Product_SetBrand_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Brand = "TestBrand";

            // Assert
            Assert.Equal("TestBrand", product.Brand);
        }

        [Fact]
        public void Product_SetCategory_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Category = "Electronics";

            // Assert
            Assert.Equal("Electronics", product.Category);
        }

        [Fact]
        public void Product_SetQuantity_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();

            // Act
            product.Quantity = 100;

            // Assert
            Assert.Equal(100, product.Quantity);
        }

        [Fact]
        public void Product_SetThumbnail_ReturnsCorrectValue()
        {
            // Arrange
            var product = new Product();
            var uri = new Uri("https://example.com/image.jpg");

            // Act
            product.Thumbnail = uri;

            // Assert
            Assert.Equal(uri, product.Thumbnail);
        }

        [Fact]
        public void Product_FullyPopulated_AllPropertiesSet()
        {
            // Arrange & Act
            var product = new Product
            {
                Id = 1,
                Title = "Laptop",
                Price = 999.99f,
                Rating = 4.5f,
                Brand = "Dell",
                Category = "Electronics",
                Thumbnail = new Uri("https://example.com/laptop.jpg"),
                Quantity = 50
            };

            // Assert
            Assert.Equal(1, product.Id);
            Assert.Equal("Laptop", product.Title);
            Assert.Equal(999.99f, product.Price);
            Assert.Equal(4.5f, product.Rating);
            Assert.Equal("Dell", product.Brand);
            Assert.Equal("Electronics", product.Category);
            Assert.NotNull(product.Thumbnail);
            Assert.Equal(50, product.Quantity);
        }

        [Fact]
        public void Product_RatingBoundary_ZeroIsValid()
        {
            // Arrange
            var product = new Product { Rating = 0f };
            var context = new ValidationContext(product) { MemberName = "Rating" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(product.Rating, context, results);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Product_RatingBoundary_FiveIsValid()
        {
            // Arrange
            var product = new Product { Rating = 5f };
            var context = new ValidationContext(product) { MemberName = "Rating" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(product.Rating, context, results);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Product_HasSearchableAttribute_OnTitle()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty("Title");

            // Act
            var hasAttribute = propertyInfo?.GetCustomAttributes(typeof(SearchableAttribute), false).Any();

            // Assert
            Assert.True(hasAttribute);
        }

        [Fact]
        public void Product_HasSearchableAttribute_OnBrand()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty("Brand");

            // Act
            var hasAttribute = propertyInfo?.GetCustomAttributes(typeof(SearchableAttribute), false).Any();

            // Assert
            Assert.True(hasAttribute);
        }

        [Fact]
        public void Product_HasSortableAttribute_OnPrice()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty("Price");

            // Act
            var hasAttribute = propertyInfo?.GetCustomAttributes(typeof(SortableAttribute), false).Any();

            // Assert
            Assert.True(hasAttribute);
        }

        [Fact]
        public void Product_HasSortableAttribute_OnRating()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty("Rating");

            // Act
            var hasAttribute = propertyInfo?.GetCustomAttributes(typeof(SortableAttribute), false).Any();

            // Assert
            Assert.True(hasAttribute);
        }

        [Fact]
        public void Product_HasSortableAttribute_OnQuantity()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty("Quantity");

            // Act
            var hasAttribute = propertyInfo?.GetCustomAttributes(typeof(SortableAttribute), false).Any();

            // Assert
            Assert.True(hasAttribute);
        }
    }
}
