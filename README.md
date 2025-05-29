# SofthouseConverter

## Features

- Accepts input in a custom, pipe-separated text format (see example below)
- Parses people, phone numbers, addresses, and family relationships
- Outputs well-structured XML matching a specified schema
- Exposes a REST API endpoint for conversion
- Includes OpenAPI/Swagger documentation for easy testing


## Example

### Input (`text/plain`)

```
 P|Victoria|Bernadotte
 T|070-0101010|0459-123456
 A|Haga Slott|Stockholm|101
 F|Estelle|2012
 A|Solliden|Öland|10002
 F|Oscar|2016
 T|0702-020202|02-202020
 P|Joe|Biden
 A|White House|Washington, D.C
```
### Output (`application/xml`)
```
<people>
  <person>
    <firstname>Victoria</firstname>
    <lastname>Bernadotte</lastname>
    <address>
      <street>Haga Slott</street>
      <city>Stockholm</city>
      <postcode>101</postcode>
    </address>
    <phone>
      <mobile>070-0101010</mobile>
      <landline>0459-123456</landline>
    </phone>
    <family>
      <name>Estelle</name>
      <born>2012</born>
      <address>
        <street>Solliden</street>
        <city>Öland</city>
        <postcode>10002</postcode>
      </address>
    </family>
    <family>
      <name>Oscar</name>
      <born>2016</born>
      <phone>
        <mobile>0702-020202</mobile>
        <landline>02-202020</landline>
      </phone>
    </family>
  </person>
  <person>
    <firstname>Joe</firstname>
    <lastname>Biden</lastname>
    <address>
      <street>White House</street>
      <city>Washington, D.C</city>
    </address>
  </person>
</people>
```

## Usage

### Convert Text to XML via API

**Endpoint:**  
`POST /api/converter/XML`

**Headers:**
- `Content-Type: text/plain`
- `Accept: application/xml`

**Body:**  
Paste your line-based data (see example above).

**Example with curl:**
```

curl -X 'POST' \
  'http://localhost:5265/api/converter/XML' \
  -H 'accept: */*' \
  -H 'Content-Type: text/plain' \
  -d ' P|Victoria|Bernadotte
 T|070-0101010|0459-123456
 A|Haga Slott|Stockholm|101
 F|Estelle|2012
 A|Solliden|Öland|10002
 F|Oscar|2016
 T|0702-020202|02-202020
 P|Joe|Biden
 A|White House|Washington, D.C'

```

**Response:**  
Returns the equivalent XML as shown above.


## Development

- The main logic lives in `XMLConverterService.cs`
- Custom input formatters and Swagger filters are in the `Services` folder
- Unit tests are in the `SofthouseConverter.Tests` project