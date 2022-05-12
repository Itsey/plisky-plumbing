using System;
using System.Linq;
using Plisky.Diagnostics;
using Plisky.Test;
using Xunit;
using Xunit.Sdk;

namespace Plisky.PliskyLibTests.AttributeTests {
    public class AttributeTests {
        protected Bilge b = new Bilge();
        public Action MethodCallback;

        [Fact]
        [Trait("UnitTest", "UnitTest")]
        public void MethodWithTraitAttribute_ReturnsTrait() {
            var method = typeof(ClassUnderTest).GetMethod("Trait");

            var traits = TraitHelper.GetTraits(method);

            string value = Assert.Single(traits.Select(kvp => $"{kvp.Key} = {kvp.Value}").OrderBy(_ => _, StringComparer.OrdinalIgnoreCase));
            Assert.Equal("name = value", value);
        }

        [Fact]
        [Category("UnitTest")]
        public void MethodWithCategoryAttribute_ReturnsCategoryTrait() {
            var method = typeof(ClassUnderTest).GetMethod("Category");

            var traits = TraitHelper.GetTraits(method);

            string value = Assert.Single(traits.Select(kvp => $"{kvp.Key} = {kvp.Value}").OrderBy(_ => _, StringComparer.OrdinalIgnoreCase));
            Assert.Equal("Category = name", value);
        }

        [Theory]
        [Unit, Integration, Fresh, Isolated]
        [InlineData("Unit")] 
        [InlineData("Integration")]
        [InlineData("Fresh")]
        [InlineData("Isolated")]
        public void MethodWithCustomCategoryAttribute_ReturnsAppropriateCategory(string category) {
            var method = typeof(ClassUnderTest).GetMethod(category);

            var traits = TraitHelper.GetTraits(method);

            string value = Assert.Single(traits.Select(kvp => $"{kvp.Key} = {kvp.Value}").OrderBy(_ => _, StringComparer.OrdinalIgnoreCase));
            Assert.Equal($"Category = {category}", value);
        }

        [Fact]
        [Build(BuildType.Any)]
        public void MethodWithBuildTraitAttribute_ReturnsBuildTrait() {
            var method = typeof(ClassUnderTest).GetMethod("Build");

            var traits = TraitHelper.GetTraits(method);

            string value = Assert.Single(traits.Select(kvp => $"{kvp.Key} = {kvp.Value}").OrderBy(_ => _, StringComparer.OrdinalIgnoreCase));
            Assert.Equal("Build = Any", value);
        }

        [Fact]
        [Bug(123)]
        public void MethodWithBugTraitAttribute_ReturnsBugTrait() {
            var method = typeof(ClassUnderTest).GetMethod("Bug");

            var traits = TraitHelper.GetTraits(method);

            string value = Assert.Single(traits.Select(kvp => $"{kvp.Key} = {kvp.Value}").OrderBy(_ => _, StringComparer.OrdinalIgnoreCase));
            Assert.Equal("Bug = 123", value);
        }
    }
}
