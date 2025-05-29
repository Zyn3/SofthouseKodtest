namespace SofthouseConverter.Tests
{
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using System.Text;
    using SofthouseConverter;

    public class TextPlainInputFormatterTests
    {
        private readonly TextPlainInputFormatter _formatter;

        public TextPlainInputFormatterTests()
        {
            _formatter = new TextPlainInputFormatter();
        }

        [Fact]
        public async Task ReadRequestBodyAsync_WithValidText_ReturnsContent()
        {
            // Arrange
            const string testData = "P|Test|User";
            var context = CreateFormatterContext( testData );

            // Act
            var result = await _formatter.ReadRequestBodyAsync( context, Encoding.UTF8 );

            // Assert
            Assert.True( result.IsModelSet );
            Assert.Equal( testData, result.Model );
        }

        [Fact]
        public void SupportedMediaTypes_ContainsTextPlain()
        {
            // Assert
            Assert.Contains( "text/plain", _formatter.SupportedMediaTypes );
        }

        [Fact]
        public void SupportedEncodings_ContainsUtf8()
        {
            // Assert
            Assert.Contains( Encoding.UTF8, _formatter.SupportedEncodings );
        }

        [Fact]
        public async Task ReadRequestBodyAsync_WithEmptyStream_ReturnsEmptyString()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = new MemoryStream();

            var context = new InputFormatterContext(
                httpContext,
                "test",
                new ModelStateDictionary(),
                new EmptyModelMetadataProvider().GetMetadataForType( typeof( string ) ),
                ( s, e ) => new StreamReader( s, e ) );

            // Act
            var result = await _formatter.ReadRequestBodyAsync( context, Encoding.UTF8 );

            // Assert
            Assert.Equal( string.Empty, result.Model );
        }

        private InputFormatterContext CreateFormatterContext( string content )
        {
            var httpContext = new DefaultHttpContext();
            var stream = new MemoryStream( Encoding.UTF8.GetBytes( content ) );
            httpContext.Request.Body = stream;
            httpContext.Request.ContentType = "text/plain";

            var modelState = new ModelStateDictionary();
            var metadata = new EmptyModelMetadataProvider()
                .GetMetadataForType( typeof( string ) );

            return new InputFormatterContext(
                httpContext,
                "inputData",
                modelState,
                metadata,
                ( stream, encoding ) => new StreamReader( stream, encoding ) );
        }
    }
}