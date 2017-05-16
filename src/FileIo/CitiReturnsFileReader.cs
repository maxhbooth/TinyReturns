using System.Collections.Generic;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.ReturnSeriesImportContext.Services.CitiFileImport;
using Dimensional.TinyReturns.Core.SharedContext.Services;
using Microsoft.VisualBasic.FileIO;

namespace Dimensional.TinyReturns.FileIo
{
    public class CitiReturnsFileReader : ICitiReturnsFileReader
    {
        private readonly ISystemLog _systemLog;

        public CitiReturnsFileReader(
            ISystemLog systemLog)
        {
            _systemLog = systemLog;
        }

        public CitiMonthlyReturnsDataFileRecord[] ReadFile(
            string filePath)
        {
            var parser = SetupParserForCsv(filePath);

            var records = new List<CitiMonthlyReturnsDataFileRecord>();

            SkipHeaderRow(parser);

            while (!parser.EndOfData)
            {
                var parseFields = parser.ReadFields();

                var r = CreateFileRecordModel(parseFields);

                records.Add(r);
            }

            _systemLog.Info(string.Format("Closing file: '{0}'", filePath));

            return records.ToArray();
        }

        private CitiMonthlyReturnsDataFileRecord CreateFileRecordModel(
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
            _systemLog.Info(string.Format("Opening file: '{0}'", filePath));
            
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