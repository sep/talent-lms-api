using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ConsoleTables;
using CsvHelper;
using MoreLinq;

namespace TalentLMSReporting
{
    public interface IOutput
    {
        void Write<T>(IEnumerable<T> rows);
        void Write(IEnumerable<string> header, IEnumerable<IEnumerable<string>> rows);
    }

    public class ConsoleTableOutput : IOutput
    {
        private readonly Action<ConsoleTableOptions> _tableConfiguration;

        public ConsoleTableOutput(Action<ConsoleTableOptions> tableConfiguration)
        {
            _tableConfiguration = tableConfiguration;
        }

        void IOutput.Write<T>(IEnumerable<T> rows)
        {
            ConsoleTable
               .From(rows)
               .Configure(_tableConfiguration)
               .Write();
        }

        void IOutput.Write(IEnumerable<string> header, IEnumerable<IEnumerable<string>> rows)
        {
            var table = new ConsoleTable(header.ToArray());
            table.Configure(_tableConfiguration);

            foreach (var row in rows)
                table.AddRow(row.ToArray());

            table.Write();
        }
    }

    public class CsvOutput : IOutput
    {
        private readonly TextWriter _stdOut;

        public CsvOutput(TextWriter stdOut)
        {
            _stdOut = stdOut;
        }

        void IOutput.Write<T>(IEnumerable<T> rows)
        {
            using (var csv = new CsvWriter(_stdOut, CultureInfo.CurrentUICulture))
            {
                csv.WriteHeader(typeof(T));
                csv.WriteRecords(rows);
            }
        }

        void IOutput.Write(IEnumerable<string> header, IEnumerable<IEnumerable<string>> rows)
        {
            using (var csv = new CsvWriter(_stdOut, CultureInfo.CurrentUICulture, leaveOpen: true))
            {
                rows
                   .Select(r => r.ToArray())
                   .ForEach(r =>
                    {
                        r.ForEach(csv.WriteField);
                        csv.NextRecord();
                    });
            }
        }
    }
}