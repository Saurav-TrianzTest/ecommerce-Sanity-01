using System;
using System.Collections.Generic;
using Xunit;
using EcommerceWebApi.Utilities;
using EcommerceWebApi.DTOs;

namespace EcommerceWebApi.Tests.Utilities
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
        public void PaginationFilter_Constructor_WithValidValues_SetsCorrectly()
        {
            // Arrange & Act
            var filter = new PaginationFilter(2, 5);

            // Assert
            Assert.Equal(2, filter.PageNumber);
            Assert.Equal(5, filter.PageSize);
        }

        [Fact]
        public void PaginationFilter_Constructor_WithPageNumberLessThanOne_SetsToOne()
        {
            // Arrange & Act
            var filter = new PaginationFilter(0, 5);

            // Assert
            Assert.Equal(1, filter.PageNumber);
        }

        [Fact]
        public void PaginationFilter_Constructor_WithNegativePageNumber_SetsToOne()
        {
            // Arrange & Act
            var filter = new PaginationFilter(-5, 5);

            // Assert
            Assert.Equal(1, filter.PageNumber);
        }

        [Fact]
        public void PaginationFilter_Constructor_WithPageSizeGreaterThanTen_SetsToTen()
        {
            // Arrange & Act
            var filter = new PaginationFilter(1, 20);

            // Assert
            Assert.Equal(10, filter.PageSize);
        }

        [Fact]
        public void PaginationFilter_Constructor_WithPageSizeExactlyTen_SetsToTen()
        {
            // Arrange & Act
            var filter = new PaginationFilter(1, 10);

            // Assert
            Assert.Equal(10, filter.PageSize);
        }

        [Fact]
        public void PaginationFilter_Constructor_WithPageSizeLessThanTen_SetsCorrectly()
        {
            // Arrange & Act
            var filter = new PaginationFilter(1, 5);

            // Assert
            Assert.Equal(5, filter.PageSize);
        }

        [Fact]
        public void PaginationFilter_Constructor_WithPageNumberOne_SetsToOne()
        {
            // Arrange & Act
            var filter = new PaginationFilter(1, 5);

            // Assert
            Assert.Equal(1, filter.PageNumber);
        }

        [Fact]
        public void PaginationFilter_SetPageNumber_ReturnsCorrectValue()
        {
            // Arrange
            var filter = new PaginationFilter();

            // Act
            filter.PageNumber = 3;

            // Assert
            Assert.Equal(3, filter.PageNumber);
        }

        [Fact]
        public void PaginationFilter_SetPageSize_ReturnsCorrectValue()
        {
            // Arrange
            var filter = new PaginationFilter();

            // Act
            filter.PageSize = 7;

            // Assert
            Assert.Equal(7, filter.PageSize);
        }
    }

    public class PaginationHelperTests
    {
        [Fact]
        public void CreatePagedResponse_WithValidData_ReturnsCorrectPagination()
        {
            // Arrange
            var data = new List<string> { "item1", "item2", "item3" };
            var filter = new PaginationFilter(1, 3);
            int totalRecords = 10;

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, totalRecords);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(data, result.Data);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(3, result.PageSize);
            Assert.Equal(10, result.TotalRecords);
        }

        [Fact]
        public void CreatePagedResponse_TotalPages_CalculatedCorrectly()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };
            var filter = new PaginationFilter(1, 3);
            int totalRecords = 10;

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, totalRecords);

            // Assert
            // 10 / 3 = 3.33 -> ceil = 4
            Assert.Equal(4, result.TotalPages);
        }

        [Fact]
        public void CreatePagedResponse_TotalPages_ExactDivision()
        {
            // Arrange
            var data = new List<int> { 1, 2 };
            var filter = new PaginationFilter(1, 5);
            int totalRecords = 10;

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, totalRecords);

            // Assert
            // 10 / 5 = 2 exactly
            Assert.Equal(2, result.TotalPages);
        }

        [Fact]
        public void CreatePagedResponse_WithEmptyData_ReturnsEmptyList()
        {
            // Arrange
            var data = new List<string>();
            var filter = new PaginationFilter(1, 10);
            int totalRecords = 0;

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, totalRecords);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Data);
            Assert.Equal(0, result.TotalRecords);
            Assert.Equal(0, result.TotalPages);
        }

        [Fact]
        public void CreatePagedResponse_WithSingleRecord_ReturnsOnePage()
        {
            // Arrange
            var data = new List<string> { "only-item" };
            var filter = new PaginationFilter(1, 10);
            int totalRecords = 1;

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, totalRecords);

            // Assert
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(1, result.TotalRecords);
        }

        [Fact]
        public void CreatePagedResponse_PageNumber_IsPreserved()
        {
            // Arrange
            var data = new List<string> { "item" };
            var filter = new PaginationFilter(3, 5);
            int totalRecords = 20;

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, totalRecords);

            // Assert
            Assert.Equal(3, result.PageNumber);
        }

        [Fact]
        public void CreatePagedResponse_PageSize_IsPreserved()
        {
            // Arrange
            var data = new List<string> { "item" };
            var filter = new PaginationFilter(1, 5);
            int totalRecords = 20;

            // Act
            var result = PaginationHelper.CreatePagedReponse(data, filter, totalRecords);

            // Assert
            Assert.Equal(5, result.PageSize);
        }
    }
}
