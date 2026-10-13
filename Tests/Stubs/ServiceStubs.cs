// Stubs for concrete classes referenced by source files
// These stubs allow the test project to compile without including the full implementations
// that have complex dependencies (JsonFlatFileDataStore, etc.)

using EcommerceWebApi.Entities;
using EcommerceWebApi.Repositories;
using EcommerceWebApi.Services;
using EcommerceWebApi.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace EcommerceWebApi
{
    // Stub for UnitOfWork - referenced by OrderService constructor
    public class UnitOfWork : IUnitOfWork
    {
        public IUserRepository Users => throw new NotImplementedException("Stub");
        public IProductRepository Products => throw new NotImplementedException("Stub");
        public IOrderRepository Orders => throw new NotImplementedException("Stub");
        public void StartTransaction() => throw new NotImplementedException("Stub");
        public void CommitTransaction() => throw new NotImplementedException("Stub");
        public void AbortTransaction() => throw new NotImplementedException("Stub");
        public bool IsTransactionInProgress() => throw new NotImplementedException("Stub");
        public void Dispose() { }
    }
}

namespace EcommerceWebApi.Services
{
    // Stub for UserService - referenced by AuthService, JwtService, AuthorizeFilter constructors
    public class UserService : IUserService
    {
        public UserService(UnitOfWork unitOfWork) { }
        public List<User> GetAllUsers() => throw new NotImplementedException("Stub");
        public List<User> GetPaginationUsers(Utilities.PaginationFilter paginationFilter) => throw new NotImplementedException("Stub");
        public User? GetUserById(string id) => throw new NotImplementedException("Stub");
        public User? GetUserByToken(string token) => throw new NotImplementedException("Stub");
        public User? GetUserByName(string name) => throw new NotImplementedException("Stub");
        public Task<bool> InsertUserAsync(User user) => throw new NotImplementedException("Stub");
        public Task<bool> UpdateUserAsync(User user) => throw new NotImplementedException("Stub");
        public Task<bool> UpdateUserTokenAsync(User user, RefreshToken? refreshToken) => throw new NotImplementedException("Stub");
        public Task<bool> UpdateUserPropertyAsync<T>(User user, string property, T value) => throw new NotImplementedException("Stub");
        public Task<bool> DeleteUserAsync(string id) => throw new NotImplementedException("Stub");
        public void Dispose() { }
    }

    // Stub for ProductService - referenced by OrderService constructor
    public class ProductService : IProductService
    {
        public ProductService(UnitOfWork unitOfWork) { }
        public List<Product> GetAllProducts() => throw new NotImplementedException("Stub");
        public int GetNextProductId() => throw new NotImplementedException("Stub");
        public List<Product> GetPaginationProducts(Utilities.PaginationFilter paginationFilter, Utilities.QueryFilter queryFilter, out int queryProductCount) => throw new NotImplementedException("Stub");
        public Product? GetProductById(int id) => throw new NotImplementedException("Stub");
        public Task<bool> InsertProductAsync(Product product) => throw new NotImplementedException("Stub");
        public Task<bool> UpdateProductAsync(Product product) => throw new NotImplementedException("Stub");
        public Task<bool> UpdateProductPropertyAsync<T>(Product product, string property, T value) => throw new NotImplementedException("Stub");
        public Task<bool> DeleteProductAsync(int id) => throw new NotImplementedException("Stub");
        public void Dispose() { }
    }
}

namespace EcommerceWebApi.Authentication
{
    // Stub for JwtService - referenced by AuthService, AuthorizeFilter constructors
    public class JwtService : IJwtService
    {
        public JwtService(IOptions<AppSettings> applicationSettings, Services.UserService userService) { }
        public Task<bool> GenerateJWT(User user, bool isSecondFactorChecked, Microsoft.AspNetCore.Http.HttpContext context) => throw new NotImplementedException("Stub");
        public void SetJWT(string encrypterToken, Microsoft.AspNetCore.Http.HttpContext context) => throw new NotImplementedException("Stub");
        public Task<bool> SetRefreshToken(RefreshToken refreshToken, User user, Microsoft.AspNetCore.Http.HttpContext context) => throw new NotImplementedException("Stub");
        public Task<bool> RevokeToken(User user, Microsoft.AspNetCore.Http.HttpContext context) => throw new NotImplementedException("Stub");
        public User? GetUserFromJWT(Microsoft.AspNetCore.Http.HttpContext context) => throw new NotImplementedException("Stub");
        public string? GetTotpStatusFromJWT(Microsoft.AspNetCore.Http.HttpContext context) => throw new NotImplementedException("Stub");
    }
}
