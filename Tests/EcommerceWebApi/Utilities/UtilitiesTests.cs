using System;
using System.Collections.Generic;
using System.Linq;
using EcommerceWebApi.DTOs;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Utilities;
using Xunit;

namespace EcommerceWebApi.Utilities.Tests
{
    public class PaginationFilterTests
    {
        [Fact]
        public void PaginationFilter_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var filter = new PaginationFilter();

            // Assert
            Assert.Equal(1, filter.PageNumber);
            Assert.Equal(10, filter.PageSize);
        }

        [Fact]
        public void PaginationFilter_ParameterizedConstructor_SetsValues()
        {
            // Arrange & Act
            var filter = new PaginationFilter(2, 5);

            // Assert
            Assert.Equal(2, filter.PageNumber);
            Assert.Equal(5, filter.PageSize);
        }

        [Fact]
        public void PaginationFilter_PageNumberLessThanOne_SetsToOne()
        {
            // Arrange & Act
            var filter = new PaginationFilter(0, 5);

            // Assert
            Assert.Equal(1, filter.PageNumber);
        }

        [Fact]
        public void PaginationFilter_NegativePageNumber_SetsToOne()
        {
            // Arrange & Act
            var filter = new PaginationFilter(-5, 5);

            // Assert
            Assert.Equal(1, filter.PageNumber);
        }

        [Fact]
        public void PaginationFilter_PageSizeGreaterThanTen_SetsToTen()
        {
            // Arrange & Act
            var filter = new PaginationFilter(1, 20);

            // Assert
            Assert.Equal(10, filter.PageSize);
        }

        [Fact]
        public void PaginationFilter_PageSizeExactlyTen_KeepsTen()
        {
            // Arrange & Act
            var filter = new PaginationFilter(1, 10);

            // Assert
            Assert.Equal(10, filter.PageSize);
        }

        [Fact]
        public void PaginationFilter_PageSizeLessThanTen_KeepsValue()
        {
            // Arrange & Act
            var filter = new PaginationFilter(1, 5);

            // Assert
            Assert.Equal(5, filter.PageSize);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(10, 10)]
        public void PaginationFilter_ValidPageNumbers_ArePreserved(int input, int expected)
        {
            // Arrange & Act
            var filter = new PaginationFilter(input, 5);

            // Assert
            Assert.Equal(expected, filter.PageNumber);
        }
    }

    public class PaginationHelperTests
    {
        [Fact]
        public void CreatePagedReponse_WithValidData_ReturnsCorrectPagination()
        {
            // Arrange
            var data = new List<string> { "a", "b", "c" };
            var filter = new PaginationFilter(1, 10);
            int totalRecords = 25;

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, totalRecords);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(data, result.Data);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(25, result.TotalRecords);
            Assert.Equal(3, result.TotalPages); // ceil(25/10) = 3
        }

        [Fact]
        public void CreatePagedReponse_TotalPagesCalculation_IsCorrect()
        {
            // Arrange
            var data = new List<int> { 1, 2 };
            var filter = new PaginationFilter(1, 5);
            int totalRecords = 10;

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, totalRecords);

            // Assert
            Assert.Equal(2, result.TotalPages); // ceil(10/5) = 2
        }

        [Fact]
        public void CreatePagedReponse_WithZeroRecords_ReturnsTotalPagesZero()
        {
            // Arrange
            var data = new List<string>();
            var filter = new PaginationFilter(1, 10);
            int totalRecords = 0;

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, totalRecords);

            // Assert
            Assert.Equal(0, result.TotalPages);
            Assert.Equal(0, result.TotalRecords);
        }

        [Fact]
        public void CreatePagedReponse_WithPartialLastPage_RoundsUpTotalPages()
        {
            // Arrange
            var data = new List<string> { "a" };
            var filter = new PaginationFilter(1, 10);
            int totalRecords = 11;

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, totalRecords);

            // Assert
            Assert.Equal(2, result.TotalPages); // ceil(11/10) = 2
        }

        [Fact]
        public void CreatePagedReponse_WithProductList_ReturnsCorrectType()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Title = "P1", Brand = "B1", Price = 10f, Quantity = 5 },
                new Product { Id = 2, Title = "P2", Brand = "B2", Price = 20f, Quantity = 3 }
            };
            var filter = new PaginationFilter(1, 10);

            // Act
            var result = PaginationHelper.CreatePagedReponse(products, filter, 2);

            // Assert
            Assert.Equal(2, result.Data.Count);
            Assert.Equal(1, result.TotalPages);
        }
    }

    public class QueryHelperTests
    {
        private List<Product> GetSampleProducts()
        {
            return new List<Product>
            {
                new Product { Id = 1, Title = "Apple iPhone", Brand = "Apple", Category = "Electronics", Price = 999f, Rating = 4.5f, Quantity = 10 },
                new Product { Id = 2, Title = "Samsung Galaxy", Brand = "Samsung", Category = "Electronics", Price = 799f, Rating = 4.0f, Quantity = 5 },
                new Product { Id = 3, Title = "Nike Shoes", Brand = "Nike", Category = "Footwear", Price = 120f, Rating = 3.5f, Quantity = 20 },
                new Product { Id = 4, Title = "Adidas Sneakers", Brand = "Adidas", Category = "Footwear", Price = 100f, Rating = 4.2f, Quantity = 15 }
            };
        }

        [Fact]
        public void SearchObjects_WithNullSearchBy_ReturnsAllObjects()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, null, "Apple", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void SearchObjects_WithNullSearch_ReturnsAllObjects()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, "Title", null, StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void SearchObjects_WithEmptySearchBy_ReturnsAllObjects()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, "", "Apple", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void SearchObjects_ByTitle_ReturnsMatchingProducts()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, "Title", "Apple", StringComparison.OrdinalIgnoreCase).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Apple iPhone", result[0].Title);
        }

        [Fact]
        public void SearchObjects_ByBrand_ReturnsMatchingProducts()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, "Brand", "Nike", StringComparison.OrdinalIgnoreCase).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Nike", result[0].Brand);
        }

        [Fact]
        public void SearchObjects_ByCategory_ReturnsMultipleMatches()
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
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void SearchObjects_WithNonExistentProperty_ReturnsAllObjects()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, "NonExistentProp", "value", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void SearchObjects_CaseInsensitive_FindsMatch()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SearchObjects(products, "Title", "apple", StringComparison.OrdinalIgnoreCase).ToList();

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public void SortObjects_WithNullSortBy_ReturnsOriginalOrder()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SortObjects(products, null, true).ToList();

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal(1, result[0].Id);
        }

        [Fact]
        public void SortObjects_WithEmptySortBy_ReturnsOriginalOrder()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SortObjects(products, "", true).ToList();

            // Assert
            Assert.Equal(4, result.Count);
        }

        [Fact]
        public void SortObjects_ByPriceAscending_ReturnsSortedProducts()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SortObjects(products, "Price", true).ToList();

            // Assert
            Assert.Equal(100f, result[0].Price);
            Assert.Equal(120f, result[1].Price);
            Assert.Equal(799f, result[2].Price);
            Assert.Equal(999f, result[3].Price);
        }

        [Fact]
        public void SortObjects_ByPriceDescending_ReturnsSortedProducts()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SortObjects(products, "Price", false).ToList();

            // Assert
            Assert.Equal(999f, result[0].Price);
            Assert.Equal(799f, result[1].Price);
        }

        [Fact]
        public void SortObjects_ByRatingAscending_ReturnsSortedProducts()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SortObjects(products, "Rating", true).ToList();

            // Assert
            Assert.Equal(3.5f, result[0].Rating);
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
            Assert.Equal(4, result.Count);
            Assert.Equal(1, result[0].Id); // original order preserved
        }

        [Fact]
        public void SortObjects_WithNonExistentProperty_ReturnsOriginalOrder()
        {
            // Arrange
            var products = GetSampleProducts();

            // Act
            var result = QueryHelper.SortObjects(products, "NonExistentProp", true).ToList();

            // Assert
            Assert.Equal(4, result.Count);
        }
    }

    public class QueryFilterTests
    {
        [Fact]
        public void QueryFilter_DefaultValues_AreNull()
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
        public void QueryFilter_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var filter = new QueryFilter
            {
                SearchBy = "Title",
                Search = "Apple",
                SortBy = "Price",
                IsSortAscending = true
            };

            // Assert
            Assert.Equal("Title", filter.SearchBy);
            Assert.Equal("Apple", filter.Search);
            Assert.Equal("Price", filter.SortBy);
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
