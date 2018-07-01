using System;
using Xunit;
using NancyApplication;
using Nancy;
using Nancy.Testing;

namespace NancyApplication.Tests
{
    public class TopicModuleTests
    {   
        [Fact]
        public void AssertRouteExists()
        {
            // Given
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);
            
            // When
            var result = browser.Get("/", with => {
                with.HttpRequest();
            });
                
            // Then
            Assert.Equal(HttpStatusCode.OK, result.Result.StatusCode);
        }   
    } 
}