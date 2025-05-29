namespace SofthouseConverter.Tests
{
    using Xunit;
    using NSubstitute;
    using SofthouseConverter.Services;
    using Microsoft.Extensions.Logging;
    using System;

    public class XMLConverterServiceTest
    {
        private readonly XMLConverterService _sut;
        private readonly ILogger<XMLConverterService> _logger = Substitute.For<ILogger<XMLConverterService>>();

        public XMLConverterServiceTest()
        {
            _sut = new XMLConverterService( _logger );
        }

        [Fact]
        public void ParseInput_WithPersonRecord_CreatesNewPerson()
        {
            // Arrange
            var input = new[] { "P|Victoria|Bernadotte" };

            // Act
            var result = _sut.ParseInput( input );

            // Assert
            var person = Assert.Single( result );
            Assert.Equal( "Victoria", person.Firstname );
            Assert.Equal( "Bernadotte", person.Lastname );
        }

        [Fact]
        public void ParseInput_WithPhoneRecord_AddsToCurrentPerson()
        {
            // Arrange
            var input = new[]
            {
            "P|Victoria|Bernadotte",
            "T|070-0101010|0459-123456"
        };

            // Act
            var result = _sut.ParseInput( input );

            // Assert
            var person = Assert.Single( result );
            var phone = Assert.Single( person.Phones );
            Assert.Equal( "070-0101010", phone.Mobile );
            Assert.Equal( "0459-123456", phone.Landline );
        }

        [Fact]
        public void ParseInput_WithFamilyRecord_AddsFamilyToCurrentPerson()
        {
            // Arrange
            var input = new[]
            {
            "P|Victoria|Bernadotte",
            "F|Estelle|2012"
        };

            // Act
            var result = _sut.ParseInput( input );

            // Assert
            var person = Assert.Single( result );
            var family = Assert.Single( person.Families );
            Assert.Equal( "Estelle", family.Name );
            Assert.Equal( "2012", family.Born );
        }

        [Fact]
        public void GenerateXml_WithValidPerson_ReturnsCorrectStructure()
        {
            // Arrange
            var people = new List<Person>
        {
            new Person
            {
                Firstname = "Test",
                Lastname = "User",
                Phones = new List<Phone>
                {
                    new Phone { Mobile = "123", Landline = "456" }
                },
                Addresses = new List<Address>
                {
                    new Address { Street = "Main St", City = "Anytown", Postcode = "12345" }
                }
            }
        };

            // Act
            var xmlDoc = _sut.GenerateXml( people );

            // Assert
            var xml = xmlDoc.ToString();
            Assert.Contains( "<people>", xml );
            Assert.Contains( "<firstname>Test</firstname>", xml );
            Assert.Contains( "<landline>456</landline>", xml );
            Assert.Contains( "<postcode>12345</postcode>", xml );
        }

        [Fact]
        public void GenerateXml_WithFamily_IncludesNestedFamilyElements()
        {
            // Arrange
            var people = new List<Person>
        {
            new Person
            {
                Firstname = "Parent",
                Families = new List<Family>
                {
                    new Family
                    {
                        Name = "Child",
                        Born = "2020",
                        Addresses = new List<Address>
                        {
                            new Address { Street = "Play St" }
                        }
                    }
                }
            }
        };

            // Act
            var xmlDoc = _sut.GenerateXml( people );
            var familyElement = xmlDoc.Descendants( "family" ).First();

            // Assert
            Assert.Equal( "Child", familyElement.Element( "name" )?.Value );
            Assert.Equal( "Play St", familyElement.Element( "address" )?.Element( "street" )?.Value );
        }

        [Fact]
        public void ParseInput_WithInvalidRecord_LogsError()
        {
            // Arrange
            var input = new[] { "Invalid|Record" };

            // Act/Assert
            Assert.ThrowsAny<Exception>( () => _sut.ParseInput( input ) );
            _logger.Received().LogError( Arg.Any<Exception>(), "Exception occured while parsing" );
        }

        [Fact]
        public void GenerateXml_WithEmptyInput_ReturnsEmptyPeopleElement()
        {
            // Arrange
            var emptyPeople = new List<Person>();

            // Act
            var xmlDoc = _sut.GenerateXml( emptyPeople );

            // Assert
            Assert.Equal( "people", xmlDoc.Root?.Name.LocalName );
            Assert.Empty( xmlDoc.Root?.Elements() );
        }

        [Fact]
        public void GenerateXml_WithSampleData_ProducesValidXml()
        {
            // Arrange
            var input = new[]
            {
                "P|Victoria|Bernadotte",
                "T|070-0101010|0459-123456",
                "A|Haga Slott|Stockholm|101"
             };

            // Act
            var people = _sut.ParseInput( input );
            var xmlDoc = _sut.GenerateXml( people );

            // Assert
            var addressElement = xmlDoc.Descendants( "address" ).First();
            Assert.Equal( "Haga Slott", addressElement.Element( "street" )?.Value );
            Assert.Equal( "101", addressElement.Element( "postcode" )?.Value );
        }
    }
}