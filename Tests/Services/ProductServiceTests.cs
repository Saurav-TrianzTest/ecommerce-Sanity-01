using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Utilities;
using EcommerceWebApi.Services;
using EcommerceWebApi.Repositories;
using JsonFlatFileDataStore;

namespace EcommerceWebApi.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IDocumentCollection<Product>> _mockCollection;

        public ProductServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCollection = new Mock<IDocumentCollection<Product>>();
            _mockUnitOfWork.Setup(u => u.Products).Returns(_mockProductRepository.Object);
        }

        private Product CreateSampleProduct(int id = 1, string title = "Test Product")
        {
            return new Product
            {
                Id = id,
                Title = title,
                Price = 99.99f,
                Rating = 4.5f,
                Brand = "TestBrand",
                Category = "Electronics",
                Thumbnail = new Uri("https://example.com/image.jpg"),
                Quantity = 10
            };
        }

        [Fact]
        public void GetAllProducts_WhenProductsExist_ReturnsProductList()
        {
            // Arrange
            var products = new List<Product>
            {
                CreateSampleProduct(1, "Product 1"),
                CreateSampleProduct(2, "Product 2")
            };
            _mockCollection.Setup(c => c.AsQueryable()).Returns(products.AsQueryable());
            _mockProductRepository.Setup(r => r.GetAll()).Returns(_mockCollection.Object);

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetAllProducts();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetAllProducts_WhenNoProducts_ReturnsEmptyList()
        {
            // Arrange
            var products = new List<Product>();
            _mockCollection.Setup(c => c.AsQueryable()).Returns(products.AsQueryable());
            _mockProductRepository.Setup(r => r.GetAll()).Returns(_mockCollection.Object);

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetAllProducts();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetProductById_WhenProductExists_ReturnsProduct()
        {
            // Arrange
            var product = CreateSampleProduct(1);
            _mockProductRepository.Setup(r => r.GetById(1)).Returns(product);

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetProductById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void GetProductById_WhenProductNotExists_ReturnsNull()
        {
            // Arrange
            _mockProductRepository.Setup(r => r.GetById(999)).Returns((Product?)null);

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = service.GetProductById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetPaginationProducts_WithNoFilters_ReturnsFirstPage()
        {
            // Arrange
            var products = new List<Product>();
            for (int i = 1; i <= 15; i++)
            {
                products.Add(CreateSampleProduct(i, $"Product {i}"));
            }
            _mockCollection.Setup(c => c.AsQueryable()).Returns(products.AsQueryable());
            _mockProductRepository.Setup(r => r.GetAll()).Returns(_mockCollection.Object);

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);
            var paginationFilter = new PaginationFilter(1, 10);
            var queryFilter = new QueryFilter();

            // Act
            var result = service.GetPaginationProducts(paginationFilter, queryFilter, out int count);

            // Assert
            Assert.Equal(10, result.Count);
            Assert.Equal(15, count);
        }

        [Fact]
        public void GetPaginationProducts_WithSearchFilter_ReturnsFilteredProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                CreateSampleProduct(1, "Apple iPhone"),
                CreateSampleProduct(2, "Samsung Galaxy"),
                CreateSampleProduct(3, "Apple MacBook")
            };
            _mockCollection.Setup(c => c.AsQueryable()).Returns(products.AsQueryable());
            _mockProductRepository.Setup(r => r.GetAll()).Returns(_mockCollection.Object);

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);
            var paginationFilter = new PaginationFilter(1, 10);
            var queryFilter = new QueryFilter { SearchBy = "Title", Search = "Apple" };

            // Act
            var result = service.GetPaginationProducts(paginationFilter, queryFilter, out int count);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(2, count);
        }

        [Fact]
        public void GetPaginationProducts_WithSortFilter_ReturnsSortedProducts()
        {
            // Arrange
            var p1 = CreateSampleProduct(1, "Product A");
            p1.Price = 300f;
            var p2 = CreateSampleProduct(2, "Product B");
            p2.Price = 100f;
            var p3 = CreateSampleProduct(3, "Product C");
            p3.Price = 200f;
            var products = new List<Product> { p1, p2, p3 };
            _mockCollection.Setup(c => c.AsQueryable()).Returns(products.AsQueryable());
            _mockProductRepository.Setup(r => r.GetAll()).Returns(_mockCollection.Object);

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);
            var paginationFilter = new PaginationFilter(1, 10);
            var queryFilter = new QueryFilter { SortBy = "Price", IsSortAscending = true };

            // Act
            var result = service.GetPaginationProducts(paginationFilter, queryFilter, out int count);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(100f, result[0].Price);
            Assert.Equal(200f, result[1].Price);
            Assert.Equal(300f, result[2].Price);
        }

        [Fact]
        public async Task InsertProductAsync_WhenSuccessful_ReturnsTrue()
        {
            // Arrange
            var product = CreateSampleProduct();
            _mockProductRepository.Setup(r => r.InsertAsync(product)).ReturnsAsync(true);

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.InsertProductAsync(product);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task InsertProductAsync_WhenFails_ReturnsFalse()
        {
            // Arrange
            var product = CreateSampleProduct();
            _mockProductRepository.Setup(r => r.InsertAsync(product)).ReturnsAsync(false);

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.InsertProductAsync(product);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateProductAsync_WhenSuccessful_ReturnsTrue()
        {
            // Arrange
            var product = CreateSampleProduct();
            _mockProductRepository.Setup(r => r.UpdateAsync(product)).ReturnsAsync(true);

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateProductAsync(product);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateProductAsync_WhenFails_ReturnsFalse()
        {
            // Arrange
            var product = CreateSampleProduct();
            _mockProductRepository.Setup(r => r.UpdateAsync(product)).ReturnsAsync(false);

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateProductAsync(product);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateProductPropertyAsync_WithValidProperty_ReturnsTrue()
        {
            // Arrange
            var product = CreateSampleProduct();
            _mockProductRepository.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(true);

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateProductPropertyAsync(product, "Title", "New Title");

            // Assert
            Assert.True(result);
            Assert.Equal("New Title", product.Title);
        }

        [Fact]
        public async Task UpdateProductPropertyAsync_WithInvalidProperty_ReturnsFalse()
        {
            // Arrange
            var product = CreateSampleProduct();

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateProductPropertyAsync(product, "NonExistentProperty", "value");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateProductPropertyAsync_WithPriceProperty_UpdatesPrice()
        {
            // Arrange
            var product = CreateSampleProduct();
            _mockProductRepository.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(true);

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateProductPropertyAsync(product, "Price", 199.99f);

            // Assert
            Assert.True(result);
            Assert.Equal(199.99f, product.Price);
        }

        [Fact]
        public async Task DeleteProductAsync_WhenSuccessful_ReturnsTrue()
        {
            // Arrange
            _mockProductRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.CommitTransaction());

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.DeleteProductAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProductAsync_WhenFails_ReturnsFalse()
        {
            // Arrange
            _mockProductRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(false);
            _mockUnitOfWork.Setup(u => u.CommitTransaction());

            var service = new ProductServiceTestable(_mockUnitOfWork.Object);

            // Act
            var result = await service.DeleteProductAsync(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Dispose_CallsUnitOfWorkDispose()
        {
            // Arrange
            var service = new ProductServiceTestable(_mockUnitOfWork.Object);

            // Act
            service.Dispose();

            // Assert
            _mockUnitOfWork.Verify(u => u.Dispose(), Times.Once);
        }
    }

    // Testable subclass to inject IUnitOfWork
    internal class ProductServiceTestable : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductServiceTestable(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Product> GetAllProducts()
        {
            return _unitOfWork.Products.GetAll().AsQueryable().ToList();
        }

        public int GetNextProductId()
        {
            var all = _unitOfWork.Products.GetAll().AsQueryable().ToList();
            return all.Count == 0 ? 1 : all.Max(p => p.Id) + 1;
        }

        public List<Product> GetPaginationProducts(PaginationFilter paginationFilter, QueryFilter queryFilter, out int queryProductCount)
        {
            var products = GetAllProducts();
            products = QueryHelper.SearchObjects(products, queryFilter.SearchBy, queryFilter.Search, StringComparison.OrdinalIgnoreCase).ToList();
            products = QueryHelper.SortObjects(products, queryFilter.SortBy, queryFilter.IsSortAscending).ToList();
            queryProductCount = products.Count;
            return products.Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize).Take(paginationFilter.PageSize).ToList();
        }

        public Product? GetProductById(int id) => _unitOfWork.Products.GetById(id);

        public async Task<bool> InsertProductAsync(Product product) => await _unitOfWork.Products.InsertAsync(product);
        public async Task<bool> UpdateProductAsync(Product product) => await _unitOfWork.Products.UpdateAsync(product);

        public async Task<bool> UpdateProductPropertyAsync<T>(Product product, string property, T value)
        {
            var propertyInfo = typeof(Product).GetProperty(
                property,
                System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
            );
            if (propertyInfo == null) return false;
            propertyInfo.SetValue(product, value, null);
            return await UpdateProductAsync(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var result = await _unitOfWork.Products.DeleteAsync(id);
            _unitOfWork.CommitTransaction();
            return result;
        }

        public void Dispose() => _unitOfWork.Dispose();
    }
}
