using EcommerceWebApi.DTOs;
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
            var filter = new PaginationFilter(3, 5);

            // Assert
            Assert.Equal(3, filter.PageNumber);
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
            var filter = new PaginationFilter(1, 7);

            // Assert
            Assert.Equal(7, filter.PageSize);
        }

        [Fact]
        public void PaginationFilter_PageNumberExactlyOne_KeepsOne()
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
        public void CreatePagedReponse_ReturnsCorrectPageNumber()
        {
            // Arrange
            var data = new List<string> { "a", "b" };
            var filter = new PaginationFilter(2, 5);

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, 20);

            // Assert
            Assert.Equal(2, result.PageNumber);
        }

        [Fact]
        public void CreatePagedReponse_ReturnsCorrectPageSize()
        {
            // Arrange
            var data = new List<string> { "a", "b" };
            var filter = new PaginationFilter(1, 5);

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, 20);

            // Assert
            Assert.Equal(5, result.PageSize);
        }

        [Fact]
        public void CreatePagedReponse_ReturnsCorrectTotalRecords()
        {
            // Arrange
            var data = new List<string> { "a", "b" };
            var filter = new PaginationFilter(1, 5);

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, 20);

            // Assert
            Assert.Equal(20, result.TotalRecords);
        }

        [Fact]
        public void CreatePagedReponse_CalculatesTotalPagesCorrectly()
        {
            // Arrange
            var data = new List<string>();
            var filter = new PaginationFilter(1, 5);

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, 20);

            // Assert
            Assert.Equal(4, result.TotalPages); // 20 / 5 = 4
        }

        [Fact]
        public void CreatePagedReponse_RoundsUpTotalPages()
        {
            // Arrange
            var data = new List<string>();
            var filter = new PaginationFilter(1, 5);

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, 21);

            // Assert
            Assert.Equal(5, result.TotalPages); // ceil(21/5) = 5
        }

        [Fact]
        public void CreatePagedReponse_WithZeroRecords_ReturnsZeroTotalPages()
        {
            // Arrange
            var data = new List<string>();
            var filter = new PaginationFilter(1, 10);

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, 0);

            // Assert
            Assert.Equal(0, result.TotalPages);
        }

        [Fact]
        public void CreatePagedReponse_ReturnsCorrectData()
        {
            // Arrange
            var data = new List<string> { "item1", "item2", "item3" };
            var filter = new PaginationFilter(1, 10);

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, 3);

            // Assert
            Assert.Equal(data, result.Data);
        }

        [Fact]
        public void CreatePagedReponse_WithExactDivision_ReturnsCorrectTotalPages()
        {
            // Arrange
            var data = new List<int>();
            var filter = new PaginationFilter(1, 10);

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, 30);

            // Assert
            Assert.Equal(3, result.TotalPages); // 30 / 10 = 3
        }
    }

    public class QueryFilterTests
    {
        [Fact]
        public void QueryFilter_DefaultConstructor_SetsNullValues()
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
                Search = "test",
                SortBy = "Price",
                IsSortAscending = true
            };

            // Assert
            Assert.Equal("Title", filter.SearchBy);
            Assert.Equal("test", filter.Search);
            Assert.Equal("Price", filter.SortBy);
            Assert.True(filter.IsSortAscending);
        }
    }

    public class QueryHelperTests
    {
        private class TestItem
        {
            [Searchable]
            public string Name { get; set; } = null!;

            [Sortable]
            public int Value { get; set; }

            public string NonSearchable { get; set; } = null!;
        }

        [Fact]
        public void SearchObjects_WithNullSearchBy_ReturnsAllObjects()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Apple", Value = 1 },
                new TestItem { Name = "Banana", Value = 2 }
            };

            // Act
            var result = QueryHelper.SearchObjects(items, null, "Apple", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void SearchObjects_WithNullSearch_ReturnsAllObjects()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Apple", Value = 1 },
                new TestItem { Name = "Banana", Value = 2 }
            };

            // Act
            var result = QueryHelper.SearchObjects(items, "Name", null, StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void SearchObjects_WithEmptySearchBy_ReturnsAllObjects()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Apple", Value = 1 },
                new TestItem { Name = "Banana", Value = 2 }
            };

            // Act
            var result = QueryHelper.SearchObjects(items, "", "Apple", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void SearchObjects_WithValidSearchableProperty_FiltersCorrectly()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Apple", Value = 1 },
                new TestItem { Name = "Banana", Value = 2 },
                new TestItem { Name = "Apricot", Value = 3 }
            };

            // Act
            var result = QueryHelper.SearchObjects(items, "Name", "Ap", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void SearchObjects_WithNonSearchableProperty_ReturnsAllObjects()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Apple", NonSearchable = "test1", Value = 1 },
                new TestItem { Name = "Banana", NonSearchable = "test2", Value = 2 }
            };

            // Act
            var result = QueryHelper.SearchObjects(items, "NonSearchable", "test1", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void SearchObjects_WithNonExistentProperty_ReturnsAllObjects()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Apple", Value = 1 },
                new TestItem { Name = "Banana", Value = 2 }
            };

            // Act
            var result = QueryHelper.SearchObjects(items, "NonExistentProperty", "test", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void SortObjects_WithNullSortBy_ReturnsOriginalOrder()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Banana", Value = 2 },
                new TestItem { Name = "Apple", Value = 1 }
            };

            // Act
            var result = QueryHelper.SortObjects(items, null, true).ToList();

            // Assert
            Assert.Equal("Banana", result[0].Name);
            Assert.Equal("Apple", result[1].Name);
        }

        [Fact]
        public void SortObjects_WithEmptySortBy_ReturnsOriginalOrder()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Banana", Value = 2 },
                new TestItem { Name = "Apple", Value = 1 }
            };

            // Act
            var result = QueryHelper.SortObjects(items, "", true).ToList();

            // Assert
            Assert.Equal("Banana", result[0].Name);
        }

        [Fact]
        public void SortObjects_Ascending_SortsCorrectly()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "C", Value = 3 },
                new TestItem { Name = "A", Value = 1 },
                new TestItem { Name = "B", Value = 2 }
            };

            // Act
            var result = QueryHelper.SortObjects(items, "Value", true).ToList();

            // Assert
            Assert.Equal(1, result[0].Value);
            Assert.Equal(2, result[1].Value);
            Assert.Equal(3, result[2].Value);
        }

        [Fact]
        public void SortObjects_Descending_SortsCorrectly()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "A", Value = 1 },
                new TestItem { Name = "B", Value = 2 },
                new TestItem { Name = "C", Value = 3 }
            };

            // Act
            var result = QueryHelper.SortObjects(items, "Value", false).ToList();

            // Assert
            Assert.Equal(3, result[0].Value);
            Assert.Equal(2, result[1].Value);
            Assert.Equal(1, result[2].Value);
        }

        [Fact]
        public void SortObjects_WithNonSortableProperty_ReturnsOriginalOrder()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Banana", Value = 2 },
                new TestItem { Name = "Apple", Value = 1 }
            };

            // Act
            var result = QueryHelper.SortObjects(items, "Name", true).ToList();

            // Assert
            // Name is Searchable but not Sortable, so original order preserved
            Assert.Equal("Banana", result[0].Name);
        }

        [Fact]
        public void SortObjects_WithNonExistentProperty_ReturnsOriginalOrder()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Banana", Value = 2 },
                new TestItem { Name = "Apple", Value = 1 }
            };

            // Act
            var result = QueryHelper.SortObjects(items, "NonExistent", true).ToList();

            // Assert
            Assert.Equal("Banana", result[0].Name);
        }

        [Fact]
        public void SearchObjects_CaseInsensitive_FindsMatch()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Apple", Value = 1 },
                new TestItem { Name = "Banana", Value = 2 }
            };

            // Act
            var result = QueryHelper.SearchObjects(items, "Name", "apple", StringComparison.OrdinalIgnoreCase);

            // Assert
            Assert.Single(result);
            Assert.Equal("Apple", result.First().Name);
        }

        [Fact]
        public void SearchObjects_CaseSensitive_NoMatch()
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Name = "Apple", Value = 1 },
                new TestItem { Name = "Banana", Value = 2 }
            };

            // Act
            var result = QueryHelper.SearchObjects(items, "Name", "apple", StringComparison.Ordinal);

            // Assert
            Assert.Empty(result);
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
