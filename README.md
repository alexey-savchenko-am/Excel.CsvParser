# Excel.CsvParser
A tool for parsing csv files. Provides an easy way for extracting data from a specific file in different modes. It currently supports line by line extracting data mode and in-memory file processing. 

# Usage

To use the tool you should specify model deriving from ICsvModel interface, which contains fields similar to header of file.
Fields of model may have an arbitrary name and be in no particular order.
Each field should have CsvHeaderAttribute containing name as in physical file's header.
For example, assume csv file, which contains header like this: time_ref,"account","code","country_code","product_type",value,"status".
For such file we could build model like this one:
```
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
The first one parse specific csv file in memory and returns data as a collection of specified model.
SequentualParser allows to invoke specified deligate each time csv file's body row processed.
For example, lets configure SequentialParser which will output each row to console as Action object.
Full code of configuring csv parser looks like this one:

```
 string filePath = "./verylarge.csv";
 char separator = ',';

 var provider = new IteratorBasedDataProvider(new CsvFileIterator(filePath), separator);

 using (var parser = new SequentialParser<Action>(provider, OnRowProcessed))
 {
      var converter = new ReflectionBasedConverter<Action>(CultureInfo.InvariantCulture);
      var isProcessed = await parser.ProcessAsync(converter, true);
 }
 
 // callback method invokes each obtaining of csv row
 private static void OnRowProcessed(int rowNumber, Action model)
 {
    Console.WriteLine($"Line {rowNumber}:   " + JsonConvert.SerializeObject(model));
 }
```

