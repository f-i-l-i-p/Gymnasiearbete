using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace Gymnasiearbete
{
    class ExcelSheet
    {
        private Workbook Workbook { get; set; }
        public Worksheet Worksheet { get { return Workbook.Worksheets.get_Item(1); } }

        public ExcelSheet()
        {
            Workbook = CreateApplication().Workbooks.Add();
        }
        public ExcelSheet(Workbook workbook)
        {
            Workbook = workbook;
        }

        /// <summary>
        /// Creates a new excel Workbook.
        /// </summary>
        /// <returns>Excell Workbook.</returns>
        public static Application CreateApplication()
        {
            var ExcelApplication = new Application();

            // check that excel is installed
            if (ExcelApplication == null)
                throw new ArgumentException("Excel is not properly installed!");

            return ExcelApplication;
        }

        /// <summary>
        /// Saves the Worksheet to specified file location
        /// </summary>
        /// <param name="fileLocation">File location.</param>
        /// <param name="fileName">File name.</param>
        public void Save(string fileLocation, string fileName)
        {
            Workbook.SaveAs(fileLocation + "\\" + fileName + ".xlsx");
        }

        // TODO: Test if this works
        public static ExcelSheet Load(string path)
        {
            var Workbook = CreateApplication().Workbooks.Open(path, 0, true, 5, "", "", true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

            return new ExcelSheet(Workbook);
        }
    }
}
