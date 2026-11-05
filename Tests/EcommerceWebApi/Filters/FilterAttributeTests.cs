using System;
using System.Collections.Generic;
using EcommerceWebApi.Filters;
using Xunit;

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
        public void AllowAnonymousAttribute_HasMethodTargetUsage()
        {
            // Arrange
            var attrType = typeof(AllowAnonymousAttribute);

            // Act
            var usageAttr = (AttributeUsageAttribute?)Attribute.GetCustomAttribute(attrType, typeof(AttributeUsageAttribute));

            // Assert
            Assert.NotNull(usageAttr);
            Assert.True((usageAttr.ValidOn & AttributeTargets.Method) != 0);
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
        public void AllowFirstFactorAttribute_HasMethodTargetUsage()
        {
            // Arrange
            var attrType = typeof(AllowFirstFactorAttribute);

            // Act
            var usageAttr = (AttributeUsageAttribute?)Attribute.GetCustomAttribute(attrType, typeof(AttributeUsageAttribute));

            // Assert
            Assert.NotNull(usageAttr);
            Assert.True((usageAttr.ValidOn & AttributeTargets.Method) != 0);
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

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Manager")]
        public void AuthorizeRoleAttribute_Constructor_SetsVariousRoles(string role)
        {
            // Arrange & Act
            var attr = new AuthorizeRoleAttribute(role);

            // Assert
            Assert.Equal(role, attr.Role);
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
        public void AuthorizeRoleAttribute_AllowsMultiple()
        {
            // Arrange
            var attrType = typeof(AuthorizeRoleAttribute);

            // Act
            var usageAttr = (AttributeUsageAttribute?)Attribute.GetCustomAttribute(attrType, typeof(AttributeUsageAttribute));

            // Assert
            Assert.NotNull(usageAttr);
            Assert.True(usageAttr.AllowMultiple);
        }

        [Fact]
        public void AuthorizeRoleAttribute_CanBeAppliedToClassAndMethod()
        {
            // Arrange
            var attrType = typeof(AuthorizeRoleAttribute);

            // Act
            var usageAttr = (AttributeUsageAttribute?)Attribute.GetCustomAttribute(attrType, typeof(AttributeUsageAttribute));

            // Assert
            Assert.NotNull(usageAttr);
            Assert.True((usageAttr.ValidOn & AttributeTargets.Class) != 0);
            Assert.True((usageAttr.ValidOn & AttributeTargets.Method) != 0);
        }

        [Fact]
        public void AuthorizeRoleAttribute_WithEmptyRole_SetsEmptyString()
        {
            // Arrange & Act
            var attr = new AuthorizeRoleAttribute("");

            // Assert
            Assert.Equal("", attr.Role);
        }
    }
}
