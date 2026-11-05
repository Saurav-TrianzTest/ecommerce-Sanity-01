using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Utilities;
using Xunit;

namespace EcommerceWebApi.Entities.Tests
{
    public class ProductTests
    {
        [Fact]
        public void Product_DefaultValues_AreCorrect()
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
        public void Product_Title_HasSearchableAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty(nameof(Product.Title));

            // Act
            var searchableAttr = propertyInfo?.GetCustomAttributes(typeof(SearchableAttribute), false);

            // Assert
            Assert.NotNull(searchableAttr);
            Assert.NotEmpty(searchableAttr);
        }

        [Fact]
        public void Product_Brand_HasSearchableAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty(nameof(Product.Brand));

            // Act
            var searchableAttr = propertyInfo?.GetCustomAttributes(typeof(SearchableAttribute), false);

            // Assert
            Assert.NotNull(searchableAttr);
            Assert.NotEmpty(searchableAttr);
        }

        [Fact]
        public void Product_Category_HasSearchableAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty(nameof(Product.Category));

            // Act
            var searchableAttr = propertyInfo?.GetCustomAttributes(typeof(SearchableAttribute), false);

            // Assert
            Assert.NotNull(searchableAttr);
            Assert.NotEmpty(searchableAttr);
        }

        [Fact]
        public void Product_Price_HasSortableAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty(nameof(Product.Price));

            // Act
            var sortableAttr = propertyInfo?.GetCustomAttributes(typeof(SortableAttribute), false);

            // Assert
            Assert.NotNull(sortableAttr);
            Assert.NotEmpty(sortableAttr);
        }

        [Fact]
        public void Product_Rating_HasSortableAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty(nameof(Product.Rating));

            // Act
            var sortableAttr = propertyInfo?.GetCustomAttributes(typeof(SortableAttribute), false);

            // Assert
            Assert.NotNull(sortableAttr);
            Assert.NotEmpty(sortableAttr);
        }

        [Fact]
        public void Product_Quantity_HasSortableAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty(nameof(Product.Quantity));

            // Act
            var sortableAttr = propertyInfo?.GetCustomAttributes(typeof(SortableAttribute), false);

            // Assert
            Assert.NotNull(sortableAttr);
            Assert.NotEmpty(sortableAttr);
        }

        [Fact]
        public void Product_Rating_ValidationRange_IsZeroToFive()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty(nameof(Product.Rating));

            // Act
            var rangeAttr = propertyInfo?.GetCustomAttributes(typeof(RangeAttribute), false)
                                         .Cast<RangeAttribute>()
                                         .FirstOrDefault();

            // Assert
            Assert.NotNull(rangeAttr);
            Assert.Equal(0, Convert.ToInt32(rangeAttr.Minimum));
            Assert.Equal(5, Convert.ToInt32(rangeAttr.Maximum));
        }

        [Fact]
        public void Product_Id_HasRequiredAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty(nameof(Product.Id));

            // Act
            var requiredAttr = propertyInfo?.GetCustomAttributes(typeof(RequiredAttribute), false);

            // Assert
            Assert.NotNull(requiredAttr);
            Assert.NotEmpty(requiredAttr);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void Product_Quantity_CanBeSetToVariousValues(int quantity)
        {
            // Arrange & Act
            var product = new Product { Quantity = quantity };

            // Assert
            Assert.Equal(quantity, product.Quantity);
        }

        [Theory]
        [InlineData(0.0f)]
        [InlineData(2.5f)]
        [InlineData(5.0f)]
        public void Product_Rating_CanBeSetToValidValues(float rating)
        {
            // Arrange & Act
            var product = new Product { Rating = rating };

            // Assert
            Assert.Equal(rating, product.Rating);
        }
    }
}
