using CsvParser;
using CsvParser.Abstract.Models;
using CsvParser.Converters;
using CsvParser.CsvParsers;
using CsvParser.DataProviders;
using CsvParser.DataProviders.Iterators;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        class CsvModel
        {
            [CsvHeader("Year")]
            public int Year { get; set; }

            [CsvHeader("Industry_aggregation_NZSIOC")]
            public string Industry { get; set; }

            [CsvHeader("Industry_code_NZSIOC")]
            public string IndustryCodeNZSIOC { get; set; }

            [CsvHeader("Industry_name_NZSIOC")]
            public string IndustryName { get; set; }

            [CsvHeader("Units")]
            public string Units { get; set; }

            [CsvHeader("Variable_code")]
            public string VariableCode { get; set; }

            [CsvHeader("Variable_name")]
            public string VariableName { get; set; }

            [CsvHeader("Variable_category")]
            public string VariableCategory { get; set; }

            [CsvHeader("Value")]
            public string Value { get; set; }

            [CsvHeader("Industry_code_ANZSIC06")]
            public string IndustryCodeANZSIC06 { get; set; }
        }

        //time_ref,"account","code","country_code","product_type",value,"status"
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


        static async Task Main(string[] args)
        {
            string filePath = "./verylarge.csv";
            char separator = ',';

            var provider = new IteratorBasedDataProvider(new CsvFileIterator(filePath), separator);

            using (var parser = new SequentialParser<Action>(provider, OnRowProcessed))
            {
                var converter = new ReflectionBasedConverter<Action>(CultureInfo.InvariantCulture);
                var isProcessed = await parser.ProcessAsync(converter, true, true);
            }

            var status = provider.GetIterator().IsStarted;

            Console.ReadKey();
        }

        private static void OnRowProcessed(int rowNumber, Action model)
        {
            Console.WriteLine($"Line {rowNumber}:   "+ JsonConvert.SerializeObject(model));
        }
    }
}
