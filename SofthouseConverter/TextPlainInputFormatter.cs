using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;

namespace SofthouseConverter
{
    public class TextPlainInputFormatter : TextInputFormatter
    {
        public TextPlainInputFormatter()
        {
            SupportedMediaTypes.Add( "text/plain" );
            SupportedEncodings.Add( Encoding.UTF8 );
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(
            InputFormatterContext context, Encoding encoding )
        {
            using var reader = new StreamReader( context.HttpContext.Request.Body, encoding );
            var content = await reader.ReadToEndAsync();
            return await InputFormatterResult.SuccessAsync( content );
        }
    }
}