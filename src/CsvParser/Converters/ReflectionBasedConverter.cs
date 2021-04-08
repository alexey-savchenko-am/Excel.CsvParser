namespace CsvParser.Converters
{
    using CsvParser.Abstract;
    using CsvParser.Abstract.Models;
    using CsvParser.Models;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public sealed class ReflectionBasedConverter<TModel>
        : CsvConverter<TModel>
        where TModel : ICsvModel, new()
    {

        private FilterValue[] _filterValues;

        public ReflectionBasedConverter(CultureInfo cultureInfo)
            : base(cultureInfo)
        {}

        public override bool Initialize(IRow header)
        {
            if (Culture == null)
                throw new CsvParserException("culture info should not be null");
            
            _filterValues =
                ExtractFilterValues(header.Columns, typeof(TModel));

            return IsInitialized =  _filterValues.Length > 0;
        }

        public override TModel Convert(IRow value)
        {
            if (!IsInitialized)
                throw new CsvParserException("you should initialize csv converter before usage");
            return ProcessRow(value.Columns, Culture);
        }

        public override TModel Convert(IEnumerable<IColumn> value)
        {
            if (!IsInitialized)
                throw new CsvParserException("you should initialize csv converter before usage");
            return ProcessRow(value, Culture);
        }

        public override TModel Convert(ISplitRowStrategy rowSplitter, string value, char separator = ',', bool removeQuotes = true)
        {
            if (!IsInitialized)
                throw new CsvParserException("you should initialize csv converter before usage");

            var columns = rowSplitter.SplitRow<CsvColumn>(value, separator, removeQuotes);

            return ProcessRow(columns, Culture);
        }

        #region Private Members
        private TModel ProcessRow(IEnumerable<IColumn> columns, CultureInfo cultureInfo)
        {
            byte i = 0;

            var obj = new TModel();

            foreach (var column in columns)
            {
                var propInfo = _filterValues[i++].PropertyInfo;

                if (string.IsNullOrWhiteSpace(column.Value))
                {
                    if(propInfo.PropertyType.IsValueType)
                        propInfo.SetValue(obj, Activator.CreateInstance(propInfo.PropertyType));
                    else
                        propInfo.SetValue(obj, null);

                    continue;
                }
                    

                propInfo.SetValue(obj, System.Convert.ChangeType(column.Value, propInfo.PropertyType, cultureInfo));
            }

            return obj;
        }


        private FilterValue[] ExtractFilterValues(IEnumerable<IColumn> columns, Type dataModelType)
        {
            PropertyInfo[] props = dataModelType
                .GetProperties();

            var result = new FilterValue[props.Count()];

            int index = 0;

            foreach (PropertyInfo prop in props)
            {

                object[] attrs = prop.GetCustomAttributes(true);

                foreach (var attr in attrs)
                {
                    var dataColumnAttr = attr as CsvHeaderAttribute;

                    if (dataColumnAttr != null)
                    {
                        var filterValue = new FilterValue
                        {
                            Index = columns.First(c => c.Value == dataColumnAttr.ColumnName).Index,
                            PropertyInfo = prop,
                            Type = prop.PropertyType,
                            Label = dataColumnAttr.ColumnName
                        };

                        result[index++] = filterValue;
                    }
                }
            }
            return result.OrderBy(x => x.Index).ToArray();
        }

        #endregion
    }
}
