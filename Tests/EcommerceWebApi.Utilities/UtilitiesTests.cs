using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using EcommerceWebApi.Utilities;
using EcommerceWebApi.DTOs;

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
        public void PaginationFilter_Constructor_WithValidValues()
        {
            // Arrange & Act
            var filter = new PaginationFilter(2, 5);

            // Assert
            Assert.Equal(2, filter.PageNumber);
            Assert.Equal(5, filter.PageSize);
        }

        [Fact]
        public void PaginationFilter_Constructor_PageNumberLessThanOne_SetsToOne()
        {
            // Arrange & Act
            var filter = new PaginationFilter(0, 5);

            // Assert
            Assert.Equal(1, filter.PageNumber);
        }

        [Fact]
        public void PaginationFilter_Constructor_NegativePageNumber_SetsToOne()
        {
            // Arrange & Act
            var filter = new PaginationFilter(-5, 5);

            // Assert
            Assert.Equal(1, filter.PageNumber);
        }

        [Fact]
        public void PaginationFilter_Constructor_PageSizeGreaterThanTen_SetsToTen()
        {
            // Arrange & Act
            var filter = new PaginationFilter(1, 20);

            // Assert
            Assert.Equal(10, filter.PageSize);
        }

        [Fact]
        public void PaginationFilter_Constructor_PageSizeExactlyTen_KeepsTen()
        {
            // Arrange & Act
            var filter = new PaginationFilter(1, 10);

            // Assert
            Assert.Equal(10, filter.PageSize);
        }

        [Fact]
        public void PaginationFilter_Constructor_PageSizeLessThanTen_KeepsValue()
        {
            // Arrange & Act
            var filter = new PaginationFilter(1, 7);

            // Assert
            Assert.Equal(7, filter.PageSize);
        }

        [Fact]
        public void PaginationFilter_Constructor_PageNumberExactlyOne_KeepsOne()
        {
            // Arrange & Act
            var filter = new PaginationFilter(1, 5);

            // Assert
            Assert.Equal(1, filter.PageNumber);
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
        public void CreatePagedReponse_TotalRecordsExactlyDivisible_CorrectTotalPages()
        {
            // Arrange
            var data = new List<int> { 1, 2 };
            var filter = new PaginationFilter(1, 5);
            int totalRecords = 20;

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, totalRecords);

            // Assert
            Assert.Equal(4, result.TotalPages); // 20/5 = 4
        }

        [Fact]
        public void CreatePagedReponse_TotalRecordsNotDivisible_CeilsTotalPages()
        {
            // Arrange
            var data = new List<int>();
            var filter = new PaginationFilter(1, 3);
            int totalRecords = 10;

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, totalRecords);

            // Assert
            Assert.Equal(4, result.TotalPages); // ceil(10/3) = 4
        }

        [Fact]
        public void CreatePagedReponse_ZeroTotalRecords_ZeroTotalPages()
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
        public void CreatePagedReponse_PageNumberTwo_SetsCorrectly()
        {
            // Arrange
            var data = new List<string> { "x" };
            var filter = new PaginationFilter(2, 5);
            int totalRecords = 12;

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, totalRecords);

            // Assert
            Assert.Equal(2, result.PageNumber);
            Assert.Equal(5, result.PageSize);
            Assert.Equal(3, result.TotalPages); // ceil(12/5) = 3
        }
    }

    public class QueryHelperTests
    {
        private class TestItem
        {
            [Searchable]
            public string Name { get; set; } = "";

            [Sortable]
            public int Value { get; set; }

            public string NonSearchable { get; set; } = "";
        }

        [Fact]
        public void SearchObjects_NullSearchBy_ReturnsAllObjects()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Apple" },
                new TestItem { Name = "Banana" }
            };

            // Act
            var result = QueryHelper.SearchObjects(items, null, "Apple", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void SearchObjects_NullSearch_ReturnsAllObjects()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Apple" },
                new TestItem { Name = "Banana" }
            };

            // Act
            var result = QueryHelper.SearchObjects(items, "Name", null, StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void SearchObjects_EmptySearchBy_ReturnsAllObjects()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Apple" },
                new TestItem { Name = "Banana" }
            };

            // Act
            var result = QueryHelper.SearchObjects(items, "", "Apple", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void SearchObjects_ValidSearchableProperty_FiltersCorrectly()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Apple" },
                new TestItem { Name = "Banana" },
                new TestItem { Name = "Apricot" }
            };

            // Act
            var result = QueryHelper.SearchObjects(items, "Name", "Ap", StringComparison.OrdinalIgnoreCase).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.Name == "Apple");
            Assert.Contains(result, x => x.Name == "Apricot");
        }

        [Fact]
        public void SearchObjects_NonSearchableProperty_ReturnsAllObjects()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { NonSearchable = "Test1" },
                new TestItem { NonSearchable = "Test2" }
            };

            // Act
            var result = QueryHelper.SearchObjects(items, "NonSearchable", "Test1", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void SearchObjects_NonExistentProperty_ReturnsAllObjects()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Apple" }
            };

            // Act
            var result = QueryHelper.SearchObjects(items, "NonExistentProp", "Apple", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public void SearchObjects_CaseSensitive_FiltersCorrectly()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Apple" },
                new TestItem { Name = "apple" }
            };

            // Act
            var result = QueryHelper.SearchObjects(items, "Name", "Apple", StringComparison.Ordinal).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Apple", result[0].Name);
        }

        [Fact]
        public void SortObjects_NullSortBy_ReturnsOriginalOrder()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Value = 3 },
                new TestItem { Value = 1 },
                new TestItem { Value = 2 }
            };

            // Act
            var result = QueryHelper.SortObjects(items, null, true).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(3, result[0].Value);
        }

        [Fact]
        public void SortObjects_EmptySortBy_ReturnsOriginalOrder()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Value = 3 },
                new TestItem { Value = 1 }
            };

            // Act
            var result = QueryHelper.SortObjects(items, "", true).ToList();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void SortObjects_ValidSortableProperty_Ascending()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Value = 3 },
                new TestItem { Value = 1 },
                new TestItem { Value = 2 }
            };

            // Act
            var result = QueryHelper.SortObjects(items, "Value", true).ToList();

            // Assert
            Assert.Equal(1, result[0].Value);
            Assert.Equal(2, result[1].Value);
            Assert.Equal(3, result[2].Value);
        }

        [Fact]
        public void SortObjects_ValidSortableProperty_Descending()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Value = 3 },
                new TestItem { Value = 1 },
                new TestItem { Value = 2 }
            };

            // Act
            var result = QueryHelper.SortObjects(items, "Value", false).ToList();

            // Assert
            Assert.Equal(3, result[0].Value);
            Assert.Equal(2, result[1].Value);
            Assert.Equal(1, result[2].Value);
        }

        [Fact]
        public void SortObjects_NonSortableProperty_ReturnsOriginalOrder()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { NonSearchable = "B" },
                new TestItem { NonSearchable = "A" }
            };

            // Act
            var result = QueryHelper.SortObjects(items, "NonSearchable", true).ToList();

            // Assert
            Assert.Equal("B", result[0].NonSearchable);
        }

        [Fact]
        public void SortObjects_NonExistentProperty_ReturnsOriginalOrder()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Value = 5 },
                new TestItem { Value = 2 }
            };

            // Act
            var result = QueryHelper.SortObjects(items, "NonExistentProp", true).ToList();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void QueryFilter_DefaultConstructor_CreatesInstance()
        {
            // Arrange & Act
            var filter = new QueryFilter();

            // Assert
            Assert.NotNull(filter);
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
                SearchBy = "Name",
                Search = "test",
                SortBy = "Value",
                IsSortAscending = true
            };

            // Assert
            Assert.Equal("Name", filter.SearchBy);
            Assert.Equal("test", filter.Search);
            Assert.Equal("Value", filter.SortBy);
            Assert.True(filter.IsSortAscending);
        }

        [Fact]
        public void SearchableAttribute_CanBeAppliedToProperty()
        {
            // Arrange
            var prop = typeof(TestItem).GetProperty("Name");

            // Act
            var attr = prop?.GetCustomAttributes(typeof(SearchableAttribute), false);

            // Assert
            Assert.NotNull(attr);
            Assert.Single(attr);
        }

        [Fact]
        public void SortableAttribute_CanBeAppliedToProperty()
        {
            // Arrange
            var prop = typeof(TestItem).GetProperty("Value");

            // Act
            var attr = prop?.GetCustomAttributes(typeof(SortableAttribute), false);

            // Assert
            Assert.NotNull(attr);
            Assert.Single(attr);
        }
    }
}
