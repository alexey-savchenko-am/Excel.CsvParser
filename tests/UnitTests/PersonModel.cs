using CsvParser;

namespace UnitTests
{
    public class PersonModel
    {

        [CsvHeader("person_age")]
        public int Age { get; set; }

        [CsvHeader("person_surname")]
        public string Surname { get; set; }

        [CsvHeader("person_name")]
        public string Name { get; set; }

          
    }
}
