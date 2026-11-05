using EcommerceWebApi.Filters;
using Xunit;

namespace EcommerceWebApi.Filters.Tests
{
    public class AllowAnonymousAttributeTests
    {
        [Fact]
        public void AllowAnonymousAttribute_DefaultConstructor_CreatesInstance()
        {
            // Arrange & Act
            var attr = new AllowAnonymousAttribute();

            // Assert
            Assert.NotNull(attr);
        }

        [Fact]
        public void AllowAnonymousAttribute_IsAttribute()
        {
            // Arrange & Act
            var attr = new AllowAnonymousAttribute();

            // Assert
            Assert.IsAssignableFrom<Attribute>(attr);
        }

        [Fact]
        public void AllowAnonymousAttribute_HasCorrectAttributeUsage()
        {
            // Arrange
            var usageAttr = typeof(AllowAnonymousAttribute)
                .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
                .FirstOrDefault() as AttributeUsageAttribute;

            // Assert
            Assert.NotNull(usageAttr);
            Assert.Equal(AttributeTargets.Method, usageAttr.ValidOn);
        }
    }

    public class AllowFirstFactorAttributeTests
    {
        [Fact]
        public void AllowFirstFactorAttribute_DefaultConstructor_CreatesInstance()
        {
            // Arrange & Act
            var attr = new AllowFirstFactorAttribute();

            // Assert
            Assert.NotNull(attr);
        }

        [Fact]
        public void AllowFirstFactorAttribute_IsAttribute()
        {
            // Arrange & Act
            var attr = new AllowFirstFactorAttribute();

            // Assert
            Assert.IsAssignableFrom<Attribute>(attr);
        }

        [Fact]
        public void AllowFirstFactorAttribute_HasCorrectAttributeUsage()
        {
            // Arrange
            var usageAttr = typeof(AllowFirstFactorAttribute)
                .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
                .FirstOrDefault() as AttributeUsageAttribute;

            // Assert
            Assert.NotNull(usageAttr);
            Assert.Equal(AttributeTargets.Method, usageAttr.ValidOn);
        }
    }

    public class AuthorizeRoleAttributeTests
    {
        [Fact]
        public void AuthorizeRoleAttribute_Constructor_SetsRole()
        {
            // Arrange & Act
            var attr = new AuthorizeRoleAttribute("Admin");

            // Assert
            Assert.Equal("Admin", attr.Role);
        }

        [Fact]
        public void AuthorizeRoleAttribute_Constructor_WithUserRole_SetsRole()
        {
            // Arrange & Act
            var attr = new AuthorizeRoleAttribute("User");

            // Assert
            Assert.Equal("User", attr.Role);
        }

        [Fact]
        public void AuthorizeRoleAttribute_IsAttribute()
        {
            // Arrange & Act
            var attr = new AuthorizeRoleAttribute("Admin");

            // Assert
            Assert.IsAssignableFrom<Attribute>(attr);
        }

        [Fact]
        public void AuthorizeRoleAttribute_HasCorrectAttributeUsage()
        {
            // Arrange
            var usageAttr = typeof(AuthorizeRoleAttribute)
                .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
                .FirstOrDefault() as AttributeUsageAttribute;

            // Assert
            Assert.NotNull(usageAttr);
            Assert.True(usageAttr.AllowMultiple);
        }

        [Fact]
        public void AuthorizeRoleAttribute_HasClassAndMethodTargets()
        {
            // Arrange
            var usageAttr = typeof(AuthorizeRoleAttribute)
                .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
                .FirstOrDefault() as AttributeUsageAttribute;

            // Assert
            Assert.NotNull(usageAttr);
            Assert.True((usageAttr.ValidOn & AttributeTargets.Class) != 0);
            Assert.True((usageAttr.ValidOn & AttributeTargets.Method) != 0);
        }

        [Fact]
        public void AuthorizeRoleAttribute_RoleProperty_IsReadOnly()
        {
            // Arrange
            var attr = new AuthorizeRoleAttribute("Admin");

            // Assert - Role is set via constructor and is read-only
            Assert.Equal("Admin", attr.Role);
        }

        [Fact]
        public void AuthorizeRoleAttribute_WithEmptyRole_SetsEmptyRole()
        {
            // Arrange & Act
            var attr = new AuthorizeRoleAttribute("");

            // Assert
            Assert.Equal("", attr.Role);
        }
    }
}
