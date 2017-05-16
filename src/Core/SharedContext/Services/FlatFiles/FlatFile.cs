using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dimensional.TinyReturns.Core.SharedContext.Services.FlatFiles
{
    public class FlatFile<TType>
    {
        private class ColumnInfo
        {
            public Expression<Func<TType, object>> ColumnValueExpression { get; set; }
            public Action<FlatFileOptions> FlatFileOptions { get; set; }

            public Func<TType, object> CompiledExpression { get; set; }
            public string ExpressionName { get; set; }
        }

        private readonly IFlatFileIo _flatFileIo;
        private readonly List<ColumnInfo> _columnInfos;
        private string _delimiter = ",";

        public FlatFile(
            IFlatFileIo flatFileIo)
        {
            _flatFileIo = flatFileIo;

            _columnInfos = new List<ColumnInfo>();
        }

        public FlatFile<TType> AddColumn(Expression<Func<TType, object>> property)
        {
            return AddColumn(property, null);
        }

        public FlatFile<TType> AddColumn(
            Expression<Func<TType, object>> property,
            Action<FlatFileOptions> setOptionsAction)
        {
            var columnInfo = new ColumnInfo();
            columnInfo.ColumnValueExpression = property;
            columnInfo.ExpressionName = GetPropertyName(property);
            columnInfo.CompiledExpression = property.Compile();

            if (setOptionsAction != null)
            {
                columnInfo.FlatFileOptions = setOptionsAction;
            }

            _columnInfos.Add(columnInfo);

            return this;
        }

        public void WriteFile(string fileName, TType[] values)
        {
            _flatFileIo.OpenFile(fileName);

            var header = CreateHeader();

            _flatFileIo.WriteLine(header);

            foreach (var value in values)
            {
                var line = _columnInfos
                    .Select(vf => GetResult(value, vf))
                    .Aggregate((first, second) => first + _delimiter + second);

                _flatFileIo.WriteLine(line);
            }

            _flatFileIo.CloseFile();
        }

        private string CreateHeader()
        {
            var header = _columnInfos
                .Select(CalculateHeading)
                .Aggregate((first, second) => first + _delimiter + second);

            return header;
        }

        private string CalculateHeading(ColumnInfo columnInfo)
        {
            if (columnInfo.FlatFileOptions == null)
                return columnInfo.ExpressionName;

            var flatFileOptions = new FlatFileOptions();

            //TODO: Is really slow.
            columnInfo.FlatFileOptions(flatFileOptions);

            if (flatFileOptions.GetHeading() == null)
                return columnInfo.ExpressionName;

            return flatFileOptions.GetHeading();
        }

        private string GetResult(TType value, ColumnInfo columnInfo)
        {
            var result = columnInfo.CompiledExpression(value);

            if (result == null)
                return string.Empty;

            return result.ToString();
        }

        string GetPropertyName(Expression<Func<TType, object>> property)
        {
            var info = ReflectionHelper.FindProperty(property);

            return info.Name;
        }

        public FlatFile<TType> SetDelimiter(string delimiter)
        {
            _delimiter = delimiter;

            return this;
        }
    }

    public class FlatFileOptions
    {
        private string _heading;

        public FlatFileOptions Heading(string heading)
        {
            _heading = heading;

            return this;
        }

        public string GetHeading()
        {
            return _heading;
        }
    }

}