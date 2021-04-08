using AutoFixture;
using CsvParser.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnitTests.Builders;
using Xunit;

namespace UnitTests.Converters
{
    public class ReflectionBasedConverterTests
    {
       /* Fixture _fixture = new Fixture();
        ReflectionBasedConverter<PersonModel> _converter
            = new ReflectionBasedConverter<PersonModel>();

        char _separator = ',';

        public ReflectionBasedConverterTests()
        {
            string header = DelimeterStringBuilder.Build(_separator, "person_name", "person_surname", "person_age");
            _converter.Initialize(null);
        }

        [Theory]
        [InlineData("John", "Doe", 42)]
        [InlineData("Ann", "Franklin", 24)]
        [InlineData("Helen", "Woo", 61)]
        public async Task ConverterProjectHeaderCorrectlyTestAsync(string name, string surname, int age)
        {
            string row = DelimeterStringBuilder.Build(_separator, name, surname, age.ToString());
            var result = await _converter.ConvertAsync(row);

            Assert.Equal(name, result.Name);
            Assert.Equal(surname, result.Surname);
            Assert.Equal(age, result.Age);
        }*/
    }
}
