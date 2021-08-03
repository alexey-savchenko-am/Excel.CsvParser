# Excel.CsvParser
A tool for parsing csv files. Provides an easy way for extracting data from a specific file in different modes. It currently supports line by line extracting data mode and in-memory file processing. 

[![NuGet version (Excel.CsvParser)](https://img.shields.io/nuget/v/Excel.CsvParser?style=flat-square&color=blue)](https://www.nuget.org/packages/Excel.CsvParser)
[![Downloads](https://img.shields.io/nuget/dt/Excel.CsvParser?style=flat-square&color=blue)]()

# Usage
Assume we want to extract data from very large csv file, having structure like this one:

![csv sample](https://github.com/goOrn/Excel.CsvParser/blob/master/screenshots/file.JPG)

To use the tool you should specify model deriving from ICsvModel interface, which contains fields similar to header of file.
Fields of model may have an arbitrary name and be in no particular order.
Each field should have CsvHeaderAttribute containing name as in physical file's header.
For file with structure we displayed above we could build model:
```csharp
class Action : ICsvModel
{
    [CsvHeader("time_ref")]
    public int DateAsInteger { get; set; }
    
    [CsvHeader("account")]
    public string Account { get; set; }

    [CsvHeader("code")]
    public string Code { get; set; }

    [CsvHeader("country_code")]
    public string Country { get; set; }

    [CsvHeader("product_type")]
    public string ProductType { get; set; }

    [CsvHeader("value")]
    public decimal Price { get; set; }

    [CsvHeader("status")]
    public string Status { get; set; }
 }
```

# Configuring csv parser

After model definition you should configure data parser. 
There are two possible csv parsers at the moment: InMemoryParser and SequentialParser.
The first one parse specific csv file in memory and returns data as a collection of specified models.
SequentualParser allows to invoke specified delegate each time csv file's body row has been processed.
Data parser implements IDisposable interface to close streams after processing, so it should be used within using block.
For example, lets configure SequentialParser which will output each row to console as Action object.
Full code of configuring csv parser looks like this one:

```csharp
 string filePath = "./verylarge.csv";
 char separator = ',';

 var provider = new IteratorBasedDataProvider(new CsvFileIterator(filePath), separator);

 using (var parser = new SequentialParser<Action>(provider, OnRowProcessed))
 {
      var converter = new ReflectionBasedConverter<Action>(CultureInfo.InvariantCulture);
      var isProcessed = await parser.ProcessAsync(converter, cleanUpResources:true);
 }
 
 // callback method invokes each obtaining of csv row
 private void OnRowProcessed(int rowNumber, Action model)
 {
    Console.WriteLine($"Line {rowNumber}:  " + JsonConvert.SerializeObject(model));
 }
```

Csv row by row processing in action:

![csv processing](https://github.com/goOrn/Excel.CsvParser/blob/master/screenshots/screen.gif)

# Csv iterator

Iterator allows to enumerate specific file or stream row by row.
There are two kind of iterators: CsvFileIterator and CsvStreamIterator.
CsvFileIterator needs filePath in constructor.
CsvStreamIterator accepts stream.
You could also specify Encoding:

```csharp
var iterator = new CsvFileIterator(filePath, Encoding.UTF8);
```

# Csv row splitter

May be passed to provider specify csv string parsing strategy.
You could specify your own way for processing csv row implementing interface ISplitRowStrategy:

```csharp
/// <summary>
/// Splits specific row of symbols as set of columns.
/// </summary>
public interface ISplitRowStrategy
{
   IEnumerable<T> SplitRow<T>(string row, char separator, bool removeQuotes)
     where T: IColumn, new();
}
```

Or use DefaultRowSplitter or QuotesSensitiveRowSplitter (which uses by default).


# Csv provider

IteratorBasedDataProvider uses CsvFileIterator under the hood. 
It allows to enumerate rows of specified file line by line. 
Parser uses it as source for obtaining data.
The second parameter of IteratorBasedDataProvider is separator, a symbol which uses for separation columns within file:

```csharp
var separator = ',';
var iterator = new CsvFileIterator(filePath, Encoding.UTF8);
var provider = new IteratorBasedDataProvider(iterator, separator);
```

You could also specify row parsing strategy:

```csharp
var provider = new IteratorBasedDataProvider(iterator, new QuotesSensitiveRowSplitter(), separator);
```

# Csv converter

Function of csv converter is converting a raw file string into, in our case, an Action object.
ReflectionBasedConverter is based on reflection to do this task.
It requires CultureInfo object to specify culture of the csv file data.
Parser uses converter while executing its method ProcessAsync, which starts executing parsing:

```csharp
var converter = new ReflectionBasedConverter<Action>(CultureInfo.InvariantCulture);
var isProcessed = await parser.ProcessAsync(converter, cleanUpResources:true);
```

