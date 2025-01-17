﻿using System;
using System.Net.Http;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: Xunit.TestFramework("FluentAssertions.Web.Serializers.NewtonsoftJson.Tests.ConfigureFluentAssertionsWebFixture", "FluentAssertions.Web.Serializers.NewtonsoftJson.Tests")]

namespace FluentAssertions.Web.Serializers.NewtonsoftJson.Tests
{
    public class ConfigureFluentAssertionsWebFixture : XunitTestFramework
    {
        public ConfigureFluentAssertionsWebFixture(IMessageSink messageSink) : base(messageSink)
        {
            NewtonsoftJsonSerializerConfig.Options.Converters.Add(new YesNoBooleanJsonConverter());

            FluentAssertionsWebConfig.Serializer = new NewtonsoftJsonSerializer();
        }
    }

    public class NewtonsoftSerializerTests
    {
        [Fact]
        public void When_asserting_response_with_content_convertible_using_Newtonsoft_Json_Converters_to_be_as_model_it_should_succeed()
        {
            // Arrange
            using var subject = new HttpResponseMessage
            {
                Content = new StringContent(@"{
                                            ""accepted"": ""yes"",
                                            ""required"": ""no"",
                                        }", Encoding.UTF8, "application/json")
            };

            subject.Should().BeAs(new
            {
                accepted = true,
                required = false
            });

            // Act
            Action act = () =>
                subject.Should().BeAs(new
                {
                    accepted = true,
                    required = false
                });

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void When_asserting_response_with_content_with_differences_using_Newtonsoft_Json_Converters_to_be_as_model_should_throw_with_descriptive_message()
        {
            // Arrange
            using var subject = new HttpResponseMessage
            {
                Content = new StringContent(@"{
                                            ""accepted"": ""yes""
                                        }", Encoding.UTF8, "application/json")
            };

            // Act
            Action act = () =>
                 subject.Should().BeAs(new
                 {
                     accepted = false
                 });

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("*accepted to be False, but found True*");
        }

        [Fact]
        public void When_asserting_response_with_content_with_not_convertible_value_using_Newtonsoft_Json_Converters_to_be_as_model_should_throw_with_descriptive_message()
        {
            // Arrange
            using var subject = new HttpResponseMessage
            {
                Content = new StringContent(@"{
                                            ""accepted"": ""da""
                                        }", Encoding.UTF8, "application/json")
            };

            // Act
            Action act = () =>
                 subject.Should().BeAs(new
                 {
                     accepted = false
                 });

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage(@"*but the JSON representation*NewtonsoftJsonSerializer*Error converting value ""da"" to type 'System.Boolean'*");
        }
    }
}
