using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using SofthouseConverter;
using SofthouseConverter.Services;

var builder = WebApplication.CreateBuilder( args );

builder.Services
    .AddLogging()
    .AddHttpLogging();

builder.Services
    .AddTransient<XMLConverterService>();

builder.Services.AddControllers( options =>
{
    options.InputFormatters.Insert( 0, new TextPlainInputFormatter() );
    options.RespectBrowserAcceptHeader = true;
    options.ReturnHttpNotAcceptable = true;
} )
    .AddXmlSerializerFormatters()
    .AddXmlDataContractSerializerFormatters();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c =>
{
    c.SwaggerDoc( "v1", new OpenApiInfo { Title = "XML Converter", Version = "v1" } );
    c.ResolveConflictingActions( apiDescriptions => apiDescriptions.First() );

    c.OperationFilter<TextPlainFilter>();
} );

var app = builder.Build();

if ( app.Environment.IsDevelopment() )
{
    app.UseSwagger();
    app.UseSwaggerUI( c =>
    {
        c.SwaggerEndpoint( "/swagger/v1/swagger.json", "XML Converter v1" );
    } );
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();

app.MapControllers();

app.Run();