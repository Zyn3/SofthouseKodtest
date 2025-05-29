using System.Xml.Linq;

namespace SofthouseConverter.Services
{
    public class XMLConverterService
    {
        private ILogger<XMLConverterService> _logger;

        public XMLConverterService( ILogger<XMLConverterService> logger )
        {
            _logger = logger;
        }

        public List<Person> ParseInput( IEnumerable<string> lines )
        {
            try
            {
                List<Person> people = new List<Person>();
                Person currentPerson = null;
                Family currentFamily = null;

                foreach ( var rawLine in lines )
                {
                    var line = rawLine.Trim();

                    if ( string.IsNullOrWhiteSpace( line ) ) continue;

                    var parts = line.Split( '|' );

                    switch ( parts[0] )
                    {
                        case "P":
                            currentPerson = new Person { Firstname = parts[1], Lastname = parts[2] };
                            people.Add( currentPerson );
                            currentFamily = null;
                            break;

                        case "T":
                            var phone = new Phone { Mobile = parts[1], Landline = parts.Length > 2 ? parts[2] : "" };
                            if ( currentFamily != null )
                                currentFamily.Phones.Add( phone );
                            else if ( currentPerson != null )
                                currentPerson.Phones.Add( phone );
                            break;

                        case "A":
                            var addr = new Address { Street = parts[1], City = parts[2], Postcode = parts.Length > 3 ? parts[3] : "" };
                            if ( currentFamily != null )
                                currentFamily.Addresses.Add( addr );
                            else if ( currentPerson != null )
                                currentPerson.Addresses.Add( addr );
                            break;

                        case "F":
                            currentFamily = new Family { Name = parts[1], Born = parts[2] };
                            if ( currentPerson != null )
                                currentPerson.Families.Add( currentFamily );
                            break;
                    }
                }

                if ( people.Count > 0 )
                {
                    return people;
                }
                else
                {
                    throw new Exception( "Unable to parse, service was unable to parse any people from the provided dataset." );
                }
            }
            catch ( Exception e )
            {
                _logger.LogError( e, "Exception occured while parsing" );
                throw;
            }
        }

        public XDocument GenerateXml( List<Person> people )
        {
            var root = new XElement( "people" );

            foreach ( var p in people )
            {
                var personElem = new XElement( "person",
                    new XElement( "firstname", p.Firstname ),
                    new XElement( "lastname", p.Lastname )
                );

                foreach ( var a in p.Addresses )
                {
                    personElem.Add( new XElement( "address",
                        new XElement( "street", a.Street ),
                        new XElement( "city", a.City ),
                        string.IsNullOrEmpty( a.Postcode ) ? null : new XElement( "postcode", a.Postcode )
                    ) );
                }

                foreach ( var t in p.Phones )
                {
                    personElem.Add( new XElement( "phone",
                        new XElement( "mobile", t.Mobile ),
                        string.IsNullOrEmpty( t.Landline ) ? null : new XElement( "landline", t.Landline )
                    ) );
                }

                foreach ( var f in p.Families )
                {
                    var famElem = new XElement( "family",
                        new XElement( "name", f.Name ),
                        new XElement( "born", f.Born )
                    );
                    foreach ( var fa in f.Addresses )
                    {
                        famElem.Add( new XElement( "address",
                            new XElement( "street", fa.Street ),
                            new XElement( "city", fa.City ),
                            string.IsNullOrEmpty( fa.Postcode ) ? null : new XElement( "postcode", fa.Postcode )
                        ) );
                    }
                    foreach ( var ft in f.Phones )
                    {
                        famElem.Add( new XElement( "phone",
                            new XElement( "mobile", ft.Mobile ),
                            string.IsNullOrEmpty( ft.Landline ) ? null : new XElement( "landline", ft.Landline )
                        ) );
                    }
                    personElem.Add( famElem );
                }

                root.Add( personElem );
            }
            return new XDocument(
                new XDeclaration( "1.0", "utf-8", "yes" ),
                root
            );
        }
    }
}