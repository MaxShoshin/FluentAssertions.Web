﻿using FluentAssertions.Equivalency;
using FluentAssertions.Execution;
using FluentAssertions.Web.Internal;
using System;
using System.Net.Http;

namespace FluentAssertions.Web
{
    /// <summary>
    /// Contains a number of methods to assert that an <see cref="HttpResponseMessage"/> is in the expected state related to the HTTP content.
    /// </summary>
    public partial class HttpResponseMessageAssertions
    {
        /// <summary>
        /// Asserts that HTTP response content can be an equivalent representation of the expected model.
        /// </summary>
        /// <param name="expectedModel">
        /// The expected model.
        /// </param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <see paramref="because" />.
        /// </param>
        [CustomAssertion]
        public AndConstraint<HttpResponseMessageAssertions> BeAs<TModel>(TModel expectedModel, string because = "", params object[] becauseArgs)
            => BeAs(expectedModel, options => options, because, becauseArgs);

        /// <summary>
        /// Asserts that HTTP response content can be an equivalent representation of the expected model.
        /// </summary>
        /// <param name="expectedModel">
        /// The expected model.
        /// </param>
        /// <param name="options">
        /// A reference to the <see cref="EquivalencyAssertionOptions{TExpectation}"/> configuration object that can be used
        /// to influence the way the object graphs are compared. You can also provide an alternative instance of the
        /// <see cref="EquivalencyAssertionOptions{TExpectation}"/> class. The global defaults are determined by the
        /// <see cref="AssertionOptions"/> class.
        /// </param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <see paramref="because" />.
        /// </param>
        [CustomAssertion]
        public AndConstraint<HttpResponseMessageAssertions> BeAs<TModel>(TModel expectedModel, Func<EquivalencyAssertionOptions<TModel>, EquivalencyAssertionOptions<TModel>> options, string because = "", params object[] becauseArgs)
        {
            Guard.ThrowIfArgumentIsNull(options, nameof(options));

            Execute.Assertion
                .ForCondition(Subject is not null)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected a {context:response} to assert{reason}, but found <null>.");

            if (expectedModel == null)
            {
                throw new ArgumentNullException(nameof(expectedModel), "Cannot verify having a content equivalent to a model against a <null> model.");
            }

            var expectedModelType = expectedModel.GetType();

            var (success, errorMessage) = TryGetSubjectModel(out var subjectModel, expectedModelType);

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(success)
                .FailWith("Expected {context:response} to have a content equivalent to a model of type {0}, but the JSON representation could not be parsed, as the operation failed with the following message: {2}{reason}. {1}",
                    expectedModelType?.ToString() ?? "unknown type", Subject, errorMessage);

            string[] failures;

            using (var scope = new AssertionScope())
            {
                if (options != null)
                {
                    subjectModel.Should().BeEquivalentTo(expectedModel, options);
                }
                else
                {
                    subjectModel.Should().BeEquivalentTo(expectedModel);
                }

                failures = scope.Discard();
            }

            Execute.Assertion
                       .BecauseOf(because, becauseArgs)
                       .ForCondition(failures.Length == 0)
                       .FailWith("Expected {context:response} to have a content equivalent to a model, but it has differences:{0}{reason}. {1}",
                           new AssertionsFailures(failures),
                           Subject);

            return new AndConstraint<HttpResponseMessageAssertions>(this);
        }

        /// <summary>
        /// Asserts that HTTP response has content that matches a wildcard pattern.
        /// </summary>
        /// <param name="expectedWildcardText">
        /// The wildcard pattern with which the subject is matched, where * and ? have special meanings.
        /// </param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <see paramref="because" />.
        /// </param>
        [CustomAssertion]
        public AndConstraint<HttpResponseMessageAssertions> MatchInContent(string expectedWildcardText, string because = "", params object[] becauseArgs)
        {
            Guard.ThrowIfArgumentIsNull(expectedWildcardText, nameof(expectedWildcardText), "Cannot verify a HTTP response content match a <null> wildcard pattern.");

            Execute.Assertion
                .ForCondition(Subject is not null)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected a {context:response} to assert{reason}, but found <null>.");

            var content = GetContent();

            if (string.IsNullOrEmpty(content))
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .FailWith("Expected {context:response} to match the wildcard pattern {0} in its content, but content was <null>{reason}. {1}",
                        expectedWildcardText,
                        Subject);
            }

            string[] failures;

            using (var scope = new AssertionScope())
            {
                content.Should().Match(expectedWildcardText);

                failures = scope.Discard();
            }

            Execute.Assertion
                       .BecauseOf(because, becauseArgs)
                       .ForCondition(failures.Length == 0)
                       .FailWith("Expected {context:response} to match a wildcard pattern in its content, but does not since:{0}{reason}. {1}",
                           new AssertionsFailures(failures),
                           Subject);

            return new AndConstraint<HttpResponseMessageAssertions>(this);
        }
    }
}