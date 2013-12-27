using System.Collections.Generic;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.CitiFileImport;
using Microsoft.VisualBasic.FileIO;

namespace Dimensional.TinyReturns.FileIo
{
    public class CitiReturnsFileReader : ICitiReturnsFileReader
    {
        public CitiMonthlyReturnsDataFileRecord[] ReadFile(
            string filePath)
        {
            var parser = SetupParserForCsv(filePath);

            var records = new List<CitiMonthlyReturnsDataFileRecord>();

            SkipHeaderRow(parser);

            while (!parser.EndOfData)
            {
                var parseFields = parser.ReadFields();

                var r = CreateCheckModel(parseFields);

                records.Add(r);
            }

            return records.ToArray();
        }

        private CitiMonthlyReturnsDataFileRecord CreateCheckModel(
            string[] parseFields)
        {
            var r = new CitiMonthlyReturnsDataFileRecord();

            r.ExternalId = parseFields[2];
            r.EndDate = parseFields[3];
            r.Value = parseFields[5];

            return r;
        }

        private TextFieldParser SetupParserForCsv(
            string filePath)
        {
            var parser = new TextFieldParser(filePath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(new[] { "," });
            return parser;
        }

        private string SkipHeaderRow(TextFieldParser parser)
        {
            return parser.ReadLine();
        }
    }
}