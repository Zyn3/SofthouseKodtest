namespace SofthouseConverter.Tests
{
    using Xunit;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using System.Text.Json;

    public class TextPlainFilterTests
    {
        [Fact]
        public void Apply_TextPlainMediaType()
        {
            // Arrange
            var filter = new TextPlainFilter();
            var operation = new OpenApiOperation();
            var context = new OperationFilterContext(
                new ApiDescription(),
                new SchemaGenerator(
                    new SchemaGeneratorOptions(),
                    new JsonSerializerDataContractResolver( new JsonSerializerOptions() ) ),
                new SchemaRepository(),
                typeof( TestController ).GetMethod( "TestMethod" ) );

            // Act
            filter.Apply( operation, context );

            // Assert
            Assert.True( operation.RequestBody.Content.ContainsKey( "text/plain" ) );
        }

        private class TestController
        {
            public void TestMethod()
            { }
        }
    }
}