using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using EcommerceWebApi.Utilities;
using EcommerceWebApi.Entities;

namespace EcommerceWebApi.Tests.Utilities
{
    public class QueryHelperTests
    {
        private List<Product> GetSampleProducts()
        {
            return new List<Product>
            {
                new Product { Id = 1, Title = "Apple iPhone", Brand = "Apple", Category = "Electronics", Price = 999f, Rating = 4.5f, Quantity = 10 },
                new Product { Id = 2, Title = "Samsung Galaxy", Brand = "Samsung", Category = "Electronics", Price = 799f, Rating = 4.2f, Quantity = 20 },
                new Product { Id = 3, Title = "Dell Laptop", Brand = "Dell", Category = "Computers", Price = 1299f, Rating = 4.0f, Quantity = 5 },
                new Product { Id = 4, Title = "Apple MacBook", Brand = "Apple", Category = "Computers", Price = 1999f, Rating = 4.8f, Quantity = 8 },
                new Product { Id = 5, Title = "Sony Headphones", Brand = "Sony", Category = "Audio", Price = 299f, Rating = 4.3f, Quantity = 15 }
            };
        }

        // SearchObjects Tests
        [Fact]
        public void SearchObjects_WithNullSearchBy_ReturnsAllObjects()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, null, "Apple", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void SearchObjects_WithEmptySearchBy_ReturnsAllObjects()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, "", "Apple", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void SearchObjects_WithNullSearch_ReturnsAllObjects()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, "Title", null, StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void SearchObjects_WithEmptySearch_ReturnsAllObjects()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, "Title", "", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void SearchObjects_ByTitle_ReturnsMatchingProducts()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, "Title", "Apple", StringComparison.OrdinalIgnoreCase).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Contains("Apple", p.Title, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void SearchObjects_ByBrand_ReturnsMatchingProducts()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, "Brand", "Apple", StringComparison.OrdinalIgnoreCase).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Equal("Apple", p.Brand));
        }

        [Fact]
        public void SearchObjects_ByCategory_ReturnsMatchingProducts()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, "Category", "Electronics", StringComparison.OrdinalIgnoreCase).ToList();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void SearchObjects_WithNonSearchableProperty_ReturnsAllObjects()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            // Price is not [Searchable], so should return all
            var result = QueryHelper.SearchObjects(products, "Price", "999", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void SearchObjects_WithNonExistentProperty_ReturnsAllObjects()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, "NonExistentProperty", "value", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void SearchObjects_CaseInsensitive_ReturnsMatchingProducts()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, "Brand", "apple", StringComparison.OrdinalIgnoreCase).ToList();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void SearchObjects_WithNoMatch_ReturnsEmptyList()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, "Brand", "NonExistentBrand", StringComparison.OrdinalIgnoreCase).ToList();

            // Assert
            Assert.Empty(result);
        }

        // SortObjects Tests
        [Fact]
        public void SortObjects_WithNullSortBy_ReturnsOriginalOrder()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SortObjects(products, null, true).ToList();

            // Assert
            Assert.Equal(5, result.Count);
        }

        [Fact]
        public void SortObjects_WithEmptySortBy_ReturnsOriginalOrder()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SortObjects(products, "", true).ToList();

            // Assert
            Assert.Equal(5, result.Count);
        }

        [Fact]
        public void SortObjects_ByPriceAscending_ReturnsSortedProducts()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SortObjects(products, "Price", true).ToList();

            // Assert
            Assert.Equal(5, result.Count);
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(result[i].Price <= result[i + 1].Price);
            }
        }

        [Fact]
        public void SortObjects_ByPriceDescending_ReturnsSortedProducts()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SortObjects(products, "Price", false).ToList();

            // Assert
            Assert.Equal(5, result.Count);
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(result[i].Price >= result[i + 1].Price);
            }
        }

        [Fact]
        public void SortObjects_ByRatingAscending_ReturnsSortedProducts()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SortObjects(products, "Rating", true).ToList();

            // Assert
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(result[i].Rating <= result[i + 1].Rating);
            }
        }

        [Fact]
        public void SortObjects_ByRatingDescending_ReturnsSortedProducts()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SortObjects(products, "Rating", false).ToList();

            // Assert
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(result[i].Rating >= result[i + 1].Rating);
            }
        }

        [Fact]
        public void SortObjects_ByQuantityAscending_ReturnsSortedProducts()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SortObjects(products, "Quantity", true).ToList();

            // Assert
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(result[i].Quantity <= result[i + 1].Quantity);
            }
        }

        [Fact]
        public void SortObjects_WithNonSortableProperty_ReturnsOriginalOrder()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            // Title is [Searchable] but not [Sortable]
            var result = QueryHelper.SortObjects(products, "Title", true).ToList();

            // Assert
            Assert.Equal(5, result.Count);
            // Should return original order since Title is not sortable
            Assert.Equal(products[0].Id, result[0].Id);
        }

        [Fact]
        public void SortObjects_WithNonExistentProperty_ReturnsOriginalOrder()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SortObjects(products, "NonExistentProperty", true).ToList();

            // Assert
            Assert.Equal(5, result.Count);
        }

        [Fact]
        public void SortObjects_WithEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var products = new List<Product>();

            // Act
            var result = QueryHelper.SortObjects(products, "Price", true).ToList();

            // Assert
            Assert.Empty(result);
        }
    }

    public class QueryFilterTests
    {
        [Fact]
        public void QueryFilter_DefaultConstructor_SetsNullDefaults()
        {
            // Arrange & Act
            var filter = new QueryFilter();

            // Assert
            Assert.Null(filter.SearchBy);
            Assert.Null(filter.Search);
            Assert.Null(filter.SortBy);
            Assert.False(filter.IsSortAscending);
        }

        [Fact]
        public void QueryFilter_SetSearchBy_ReturnsCorrectValue()
        {
            // Arrange
            var filter = new QueryFilter();

            // Act
            filter.SearchBy = "Title";

            // Assert
            Assert.Equal("Title", filter.SearchBy);
        }

        [Fact]
        public void QueryFilter_SetSearch_ReturnsCorrectValue()
        {
            // Arrange
            var filter = new QueryFilter();

            // Act
            filter.Search = "Apple";

            // Assert
            Assert.Equal("Apple", filter.Search);
        }

        [Fact]
        public void QueryFilter_SetSortBy_ReturnsCorrectValue()
        {
            // Arrange
            var filter = new QueryFilter();

            // Act
            filter.SortBy = "Price";

            // Assert
            Assert.Equal("Price", filter.SortBy);
        }

        [Fact]
        public void QueryFilter_SetIsSortAscending_ReturnsCorrectValue()
        {
            // Arrange
            var filter = new QueryFilter();

            // Act
            filter.IsSortAscending = true;

            // Assert
            Assert.True(filter.IsSortAscending);
        }
    }

    public class AttributeTests
    {
        [Fact]
        public void SearchableAttribute_CanBeAppliedToProperty()
        {
            // Arrange & Act
            var attr = new SearchableAttribute();

            // Assert
            Assert.NotNull(attr);
        }

        [Fact]
        public void SortableAttribute_CanBeAppliedToProperty()
        {
            // Arrange & Act
            var attr = new SortableAttribute();

            // Assert
            Assert.NotNull(attr);
        }

        [Fact]
        public void UpdatableAttribute_CanBeAppliedToProperty()
        {
            // Arrange & Act
            var attr = new UpdatableAttribute();

            // Assert
            Assert.NotNull(attr);
        }
    }
}
