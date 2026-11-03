using System;
using System.Collections.Generic;
using Xunit;
using EcommerceWebApi.Filters;

namespace EcommerceWebApi.Filters.Tests
{
    public class AllowAnonymousAttributeTests
    {
        [Fact]
        public void AllowAnonymousAttribute_CanBeInstantiated()
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
            var type = typeof(AllowAnonymousAttribute);

            // Act
            var usageAttr = (AttributeUsageAttribute?)Attribute.GetCustomAttribute(type, typeof(AttributeUsageAttribute));

            // Assert
            Assert.NotNull(usageAttr);
            Assert.Equal(AttributeTargets.Method, usageAttr.ValidOn);
        }
    }

    public class AllowFirstFactorAttributeTests
    {
        [Fact]
        public void AllowFirstFactorAttribute_CanBeInstantiated()
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
            var type = typeof(AllowFirstFactorAttribute);

            // Act
            var usageAttr = (AttributeUsageAttribute?)Attribute.GetCustomAttribute(type, typeof(AttributeUsageAttribute));

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
        public void AuthorizeRoleAttribute_Constructor_WithUserRole()
        {
            // Arrange & Act
            var attr = new AuthorizeRoleAttribute("User");

            // Assert
            Assert.Equal("User", attr.Role);
        }

        [Fact]
        public void AuthorizeRoleAttribute_Constructor_WithEmptyRole()
        {
            // Arrange & Act
            var attr = new AuthorizeRoleAttribute("");

            // Assert
            Assert.Equal("", attr.Role);
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
            var type = typeof(AuthorizeRoleAttribute);

            // Act
            var usageAttr = (AttributeUsageAttribute?)Attribute.GetCustomAttribute(type, typeof(AttributeUsageAttribute));

            // Assert
            Assert.NotNull(usageAttr);
            Assert.True(usageAttr.AllowMultiple);
        }

        [Fact]
        public void AuthorizeRoleAttribute_CanBeAppliedToClassAndMethod()
        {
            // Arrange
            var type = typeof(AuthorizeRoleAttribute);

            // Act
            var usageAttr = (AttributeUsageAttribute?)Attribute.GetCustomAttribute(type, typeof(AttributeUsageAttribute));

            // Assert
            Assert.NotNull(usageAttr);
            Assert.True((usageAttr.ValidOn & AttributeTargets.Class) != 0);
            Assert.True((usageAttr.ValidOn & AttributeTargets.Method) != 0);
        }

        [Fact]
        public void AuthorizeRoleAttribute_DifferentRoles_HaveDifferentValues()
        {
            // Arrange & Act
            var adminAttr = new AuthorizeRoleAttribute("Admin");
            var userAttr = new AuthorizeRoleAttribute("User");

            // Assert
            Assert.NotEqual(adminAttr.Role, userAttr.Role);
        }
    }
}
