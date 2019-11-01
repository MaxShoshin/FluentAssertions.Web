﻿using FluentAssertions.Web.Internal;
using Newtonsoft.Json.Linq;
using Xunit;

namespace FluentAssertions.Web.Tests.Internal
{
    public class JsonExtensionsTests
    {
        [Fact]
        public void GivenJsonWithAKey_WhenHasKeyIsCalledWithThatKeyName_ThenReturnsTrue()
        {
            // Arrange
            var json = JObject.Parse(@"{
                ""errors"": {
                    ""Author"": [
                        ""The Author field is required.""
                    ]
                }
            }");

            // Act
            var result = json.HasKey("Author");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void GivenJsonWithAKey_WhenHasKeyIsCalledWithADifferentKeyName_ThenReturnsFalse()
        {
            // Arrange
            var json = JObject.Parse(@"{
                ""errors"": {
                    ""Comment"": [
                        ""The Comment field is required.""
                    ]
                }
            }");

            // Act
            var result = json.HasKey("Author");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GivenJsonWithAnEmptyKey_WhenHasKeyIsCalledWithEmptyKey_ThenReturnsTrue()
        {
            // Arrange
            var json = JObject.Parse(@"{
                ""errors"": {
                    """": [
                        ""A non-empty request body is required.""
                    ]
                }
            }");

            // Act
            var result = json.HasKey("");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void GivenJsonWithoutAnEmptyKey_WhenHasKeyIsCalledWithEmptyKey_ThenReturnsFalse()
        {
            // Arrange
            var json = JObject.Parse(@"{
                ""errors"": {
                    ""Comment"": [
                        ""The Comment field is required.""
                    ]
                }
            }");

            // Act
            var result = json.HasKey("");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GivenJsonWithAKey_WhenHasKeyIsCalledWithThatKeyNameButInDifferentCase_ThenReturnsTrue()
        {
            // Arrange
            var json = JObject.Parse(@"{
                ""errors"": {
                    ""Author"": [
                        ""The Author field is required.""
                    ]
                }
            }");

            // Act
            var result = json.HasKey("author");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void GivenJson_WhenKeyExistsAndHasAnArrayOfStringValues_ThenReturnsTheStrings()
        {
            // Arrange
            var json = JObject.Parse(@"{
                ""errors"": {
                    ""Author"": [
                        ""The Author field is required.""
                    ]
                }
            }");

            // Act
            var result = json.GetStringValuesByKey("Author");

            // Assert
            result.Should().BeEquivalentTo(new[] { "The Author field is required." });
        }

        [Fact]
        public void GivenJson_WhenKeyDoesNotExist_ThenReturnsEmpty()
        {
            // Arrange
            var json = JObject.Parse(@"{
                ""errors"": {
                    ""Author"": [
                        ""The Author field is required.""
                    ]
                }
            }");

            // Act
            var result = json.GetStringValuesByKey("Comment");

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GivenJson_WhenKeyHasSingleValue_ThenReturnsEmpty()
        {
            // Arrange
            var json = JObject.Parse(@"{
                ""errors"": {
                    ""Author"": ""The Author field is required.""
                }
            }");

            // Act
            var result = json.GetStringValuesByKey("Author");

            // Assert
            result.Should().BeEquivalentTo(new[] { "The Author field is required." });
        }

        [Fact]
        public void GivenJsonWithAField_WhenGetChildrenKeysIsCalledWithThatField_ThenReturnsDirectKeys()
        {
            // Arrange
            var json = JObject.Parse(@"{
                ""errors"": {
                    ""Author"": [
                        ""The Author field is required.""
                    ],
                    ""Content"": [
                        ""The Content field is required.""
                    ]
                }
            }");

            // Act
            var result = json.GetChildrenKeys("errors");

            // Assert
            result.Should().BeEquivalentTo("Author", "Content");
        }

        [Fact]
        public void GivenJsonWithAnEmptyField_WhenGetChildrenKeysIsCalledByItsParent_ThenReturnsDirectKeys()
        {
            // Arrange
            var json = JObject.Parse(@"{
                ""errors"": {
                    """": [
                        ""The Author field is required.""
                    ]                }
            }");

            // Act
            var result = json.GetChildrenKeys("errors");

            // Assert
            result.Should().BeEquivalentTo("");
        }
    }
}