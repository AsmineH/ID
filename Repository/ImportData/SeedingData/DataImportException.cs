using CsvHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ImportData.SeedingData
{
    public class DataImportException: Exception
    {
        public int Year { get; set; }
        public ReadingContext Context { get; set; }
        public Exception ThrownException { get; set; }
    }
}
