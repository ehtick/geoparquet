# GeoParquet

.NET 6 Reader/Writer library for GeoParquet files.

https://geoparquet.org/

Specification: https://github.com/opengeospatial/geoparquet/blob/main/format-specs/geoparquet.md

Blog: https://bertt.wordpress.com/2022/12/20/geoparquet-geospatial-vector-data-using-apache-parquet/

NuGet: https://www.nuget.org/packages/bertt.geoparquet/

Sample GeoParquet JSON metadata:

```json
{
   "version":"1.0.0-beta.1",
   "primary_column":"geometry",
   "columns":{
      "geometry":{
         "encoding":"WKB",
         "geometry_types":[
            "MultiPolygon"
         ],
         "bbox":[
            3.358378252510583,
            50.750367484598314,
            7.227498450845831,
            53.55501451790761
         ]
      }
   }
}
```

## Architecture

In this package there are functions for handling Geometadata:

1] Reading

For reading GeoParquet files there is a ParquetFileReader extension method GetGeoMetadata() to obtain the Geo metadata

2] Writing 

For writing GeoParquet files there is GeoMetadata.GetGeoMetadata(GeoColumn geoColumn) static function to get the geo metadata dictionary. This 
dictionary can be passed to the ParquetFileWriter constructor.

See sample code below for reading/writing samples.

## Sample code

In these samples NetTopologySuite (https://github.com/NetTopologySuite/NetTopologySuite) is used for handling geometries, but any library that can handle 
WKB geometries can be used.

### Reading

Use extension method 'parquetReader.GetGeoMetadata()':

```
var file = "testfixtures/gemeenten2016_1.0.parquet";
var file1 = new ParquetFileReader(file);

var geoParquet = file1.GetGeoMetadata();
Assert.That(geoParquet.Version == "1.0.0-beta.1");

var rowGroupReader = file1.RowGroup(0);
var gemName = rowGroupReader.Column(33).LogicalReader<String>().First();
Assert.IsTrue(gemName == "Appingedam");

var geometryWkb = rowGroupReader.Column(17).LogicalReader<byte[]>().First();
var wkbReader = new WKBReader();
var multiPolygon = (MultiPolygon)wkbReader.Read(geometryWkb);
Assert.That(multiPolygon.Coordinates.Count() == 165);
var firstCoordinate = multiPolygon.Coordinates.First();
Assert.That(firstCoordinate.CoordinateValue.X == 6.8319922331647964);
Assert.That(firstCoordinate.CoordinateValue.Y == 53.327288101088072);
```

### Writing 

Use GeoMetadata.GetGeoMetadata to construct the ParquetFileWriter, store the geometries as WKB.

Result Parquet file can be visualized in QGIS:

![image](https://user-images.githubusercontent.com/538812/210020220-b89da098-0877-45bd-87f2-8285941bf697.png)

```
var columns = new Column[]
{
    new Column<string>("city"),
    new Column<byte[]>("geometry")
};

var bbox = new double[] {  3.3583782525105832,
            50.750367484598314,
            7.2274984508458306,
            53.555014517907608};

var geoColumn = new GeoColumn();
geoColumn.Bbox = bbox;
geoColumn.Encoding = "WKB";
geoColumn.Geometry_types.Add("Point");
var geometadata = GeoMetadata.GetGeoMetadata(geoColumn);

var parquetFileWriter = new ParquetFileWriter(@"d:\aaa\writing_sample.parquet", columns,Compression.Snappy,geometadata);
var rowGroup = parquetFileWriter.AppendRowGroup();

var nameWriter = rowGroup.NextColumn().LogicalWriter<String>();
nameWriter.WriteBatch(new string[] { "London", "Derby" });
        
var geom0 = new Point(5, 51);
var geom1 = new Point(5.5, 51);

var wkbWriter = new WKBWriter();
var wkbBytes = new byte[][] { wkbWriter.Write(geom0), wkbWriter.Write(geom1) };

var geometryWriter = rowGroup.NextColumn().LogicalWriter<byte[]>();
geometryWriter.WriteBatch(wkbBytes);
parquetFileWriter.Close();
  ```

## Dependencies

- Newtonsoft.JSON 13 https://github.com/JamesNK/Newtonsoft.Json

- ParquetSharp 12 https://github.com/G-Research/ParquetSharp

## Schema generation 

GeoParquet metadata classes are generated from JSON schema using NJsonSchema.CodeGeneration.CSharp (https://github.com/RicoSuter/NJsonSchema), see console project 
'geoparquet.codegen' for details.

Schema used: 

https://geoparquet.org/releases/v1.0.0-beta.1/schema.json

# Roadmap

- Add support for multiple geometry columns;

- add reading/writing Apache Arrow encoding for geometries;

- add support for crs;

- add (spatial) filters.

## History

2023-07-13: version 0.5 - using schema v1.0.0-beta.1, ParquetSharp to 12.0.1

2023-01-01: version 0.4 - using ParquetSharp 10.0.1-beta1 instead of Parquet.Net 4

2022-12-30: version 0.3.1 - make geometry column name optional in SetGeoMetadata

2022-12-30: version 0.3 - add extension method to write geo metadata

2022-12-27: version 0.2 - add extension method to read geo metadata

2022-12-23: version 0.1 - implementing reader

