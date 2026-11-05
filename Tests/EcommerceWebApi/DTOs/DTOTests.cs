using System;
using System.Collections.Generic;
using EcommerceWebApi.DTOs;
using Xunit;

namespace EcommerceWebApi.DTOs.Tests
{
    public class PaginationDTOTests
    {
        [Fact]
        public void PaginationDTO_Constructor_SetsPageNumberAndPageSize()
        {
            // Arrange & Act
            var dto = new PaginationDTO<List<string>>(new List<string>(), 2, 5);

            // Assert
            Assert.Equal(2, dto.PageNumber);
            Assert.Equal(5, dto.PageSize);
        }

        [Fact]
        public void PaginationDTO_Constructor_SetsData()
        {
            // Arrange
            var data = new List<string> { "item1", "item2" };

            // Act
            var dto = new PaginationDTO<List<string>>(data, 1, 10);

            // Assert
            Assert.Equal(data, dto.Data);
        }

        [Fact]
        public void PaginationDTO_TotalPages_CanBeSet()
        {
            // Arrange
            var dto = new PaginationDTO<List<string>>(new List<string>(), 1, 10);

            // Act
            dto.TotalPages = 5;

            // Assert
            Assert.Equal(5, dto.TotalPages);
        }

        [Fact]
        public void PaginationDTO_TotalRecords_CanBeSet()
        {
            // Arrange
            var dto = new PaginationDTO<List<string>>(new List<string>(), 1, 10);

            // Act
            dto.TotalRecords = 50;

            // Assert
            Assert.Equal(50, dto.TotalRecords);
        }

        [Fact]
        public void PaginationDTO_WithIntData_WorksCorrectly()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };

            // Act
            var dto = new PaginationDTO<List<int>>(data, 1, 10);

            // Assert
            Assert.Equal(3, dto.Data.Count);
        }

        [Fact]
        public void PaginationDTO_DefaultTotalPages_IsZero()
        {
            // Arrange & Act
            var dto = new PaginationDTO<string>("test", 1, 10);

            // Assert
            Assert.Equal(0, dto.TotalPages);
        }

        [Fact]
        public void PaginationDTO_DefaultTotalRecords_IsZero()
        {
            // Arrange & Act
            var dto = new PaginationDTO<string>("test", 1, 10);

            // Assert
            Assert.Equal(0, dto.TotalRecords);
        }
    }

    public class LoginDTOTests
    {
        [Fact]
        public void LoginDTO_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new LoginDTO
            {
                Username = "testuser",
                Password = "testpass"
            };

            // Assert
            Assert.Equal("testuser", dto.Username);
            Assert.Equal("testpass", dto.Password);
        }
    }

    public class RegisterDTOTests
    {
        [Fact]
        public void RegisterDTO_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new RegisterDTO
            {
                Username = "newuser",
                Password = "password123",
                ConfirmPassword = "password123"
            };

            // Assert
            Assert.Equal("newuser", dto.Username);
            Assert.Equal("password123", dto.Password);
            Assert.Equal("password123", dto.ConfirmPassword);
        }
    }

    public class TotpDTOTests
    {
        [Fact]
        public void TotpDTO_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new TotpDTO
            {
                QrCode = "otpauth://totp/test",
                Totp = "123456"
            };

            // Assert
            Assert.Equal("otpauth://totp/test", dto.QrCode);
            Assert.Equal("123456", dto.Totp);
        }
    }

    public class OrderDTOTests
    {
        [Fact]
        public void OrderDTO_SetProductList_ReturnsCorrectValues()
        {
            // Arrange
            var productList = new Dictionary<int, int> { { 1, 2 }, { 3, 4 } };

            // Act
            var dto = new OrderDTO { ProductList = productList };

            // Assert
            Assert.Equal(productList, dto.ProductList);
            Assert.Equal(2, dto.ProductList.Count);
        }
    }

    public class ProductDTOTests
    {
        [Fact]
        public void ProductDTO_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new ProductDTO
            {
                Title = "Test Product",
                Price = 29.99f,
                Brand = "TestBrand",
                Category = "Electronics",
                Thumbnail = new Uri("https://example.com/img.jpg"),
                Quantity = 5
            };

            // Assert
            Assert.Equal("Test Product", dto.Title);
            Assert.Equal(29.99f, dto.Price);
            Assert.Equal("TestBrand", dto.Brand);
            Assert.Equal("Electronics", dto.Category);
            Assert.Equal(new Uri("https://example.com/img.jpg"), dto.Thumbnail);
            Assert.Equal(5, dto.Quantity);
        }
    }

    public class UserDTOTests
    {
        [Fact]
        public void UserDTO_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new UserDTO
            {
                Id = "user-id",
                Username = "testuser",
                Role = "Admin",
                IsTwoFactorAuthActivated = true
            };

            // Assert
            Assert.Equal("user-id", dto.Id);
            Assert.Equal("testuser", dto.Username);
            Assert.Equal("Admin", dto.Role);
            Assert.True(dto.IsTwoFactorAuthActivated);
        }

        [Fact]
        public void UserDTO_IsTwoFactorAuthActivated_DefaultIsFalse()
        {
            // Arrange & Act
            var dto = new UserDTO();

            // Assert
            Assert.False(dto.IsTwoFactorAuthActivated);
        }
    }
}
