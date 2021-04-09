namespace UnitTests.Converters
{
    using AutoFixture;
    using AutoFixture.Kernel;
    using CsvParser;
    using CsvParser.Abstract.Models;
    using CsvParser.Converters;
    using System.Globalization;
    using UnitTests.Builders;
    using Xunit;

    public class ReflectionBasedConverterTests
    {
        Fixture _fixture = new Fixture();


        public ReflectionBasedConverterTests()
        {
            _fixture.Customizations.Add(
                 new TypeRelay(typeof(IColumn),typeof(TestColumn)));
            _fixture.Customizations.Add(
                new TypeRelay(typeof(IRow), typeof(TestRow)));
        }

        [Theory]
        [InlineData("John", "Doe", 42)]
        [InlineData("Ann", "Franklin", 24)]
        [InlineData("Helen", "Woo", 61)]
        public void ConverterProjectHeaderCorrectlyTest(string person_name, string person_surname, int person_age)
        {
            // arrange

            var converter = new ReflectionBasedConverter<PersonModel>(CultureInfo.InvariantCulture);
            var header = CsvTextBuilder.BuildRow(_fixture, nameof(person_name), nameof(person_surname), nameof(person_age));
            converter.Initialize(header);

            var bodyRow = CsvTextBuilder.BuildRow(_fixture, person_name, person_surname, person_age.ToString());

            // act
            var result = converter.Convert(bodyRow);

            // assert
            Assert.Equal(person_name, result.Name);
            Assert.Equal(person_surname, result.Surname);
            Assert.Equal(person_age, result.Age);
        }

        public class PersonModel
            : ICsvModel
        {

            [CsvHeader("person_age")]
            public int Age { get; set; }

            [CsvHeader("person_surname")]
            public string Surname { get; set; }

            [CsvHeader("person_name")]
            public string Name { get; set; }


        }
    }
}
