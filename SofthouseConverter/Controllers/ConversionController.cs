using Microsoft.AspNetCore.Mvc;
using SofthouseConverter.Services;
using System.Text;

namespace SofthouseConverter.Controllers
{
    [ApiController]
    [Route( "api/converter" )]
    public class ConversionController : ControllerBase
    {
        private readonly ILogger<ConversionController> _logger;
        private readonly XMLConverterService _converterService;

        public ConversionController( ILogger<ConversionController> logger, XMLConverterService converterService )
        {
            _logger = logger;
            _converterService = converterService;
        }

        [HttpPost]
        [Route( "XML" )]
        [Produces( "application/xml" )]
        [Consumes( "text/plain" )]
        public IActionResult ConvertToXml( [FromBody] string inputData )
        {
            try
            {
                var people = _converterService.ParseInput( inputData.Split( '\n' ) );
                var xmlDoc = _converterService.GenerateXml( people );

                return Content( xmlDoc.ToString(), "application/xml" );
            }
            catch ( Exception ex )
            {
                _logger.LogError( ex, "Exception when converting to XML" );
                return BadRequest( $"Error processing input: {ex.Message}" );
            }
        }
    }
}