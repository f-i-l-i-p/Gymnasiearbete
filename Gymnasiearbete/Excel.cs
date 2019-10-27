using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Gymnasiearbete
{
    class Excel
    {
        public IWorkbook Workbook{ get; }
        public ISheet SelectedSheet { get; private set; }

        public Excel()
        {
            Workbook = new XSSFWorkbook();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Sheet name.</param>
        public void SelectSheet(string name)
        {
            SelectedSheet = Workbook.GetSheet(name);

            // TODO: test if this work
            if (SelectedSheet == null)
                throw new ArgumentException($"A sheet with the name {name} does not exist", "name");
        }

        /// <summary>
        /// Add an empty sheet to the workbook.
        /// If no sheet name is given, the name will be its index.
        /// The new sheet will be set as selected if select = true.
        /// </summary>
        /// <param name="name">Sheet name.</param>
        /// <param name="select">Select the new sheet.</param>
        public void AddSheet(string name = null, bool select = true)
        {
            if (Workbook.GetSheet(name) != null)
                throw new ArgumentException($"A sheet with the name \"{name}\" already exists", "name");

            // create sheet
            var newSheet = string.IsNullOrEmpty(name) ? Workbook.CreateSheet() : Workbook.CreateSheet(name);

            if (select)
                SelectedSheet = newSheet;
        }

        /// <summary>
        /// Sets a row of cells.
        /// </summary>
        /// <typeparam name="T">The element type of the cells</typeparam>
        /// <param name="startX">Horizontal start coordinate.</param>
        /// <param name="y">Vertical coordinate.</param>
        /// <param name="values">Cell values from left to right</param>
        public void SetRow<T>(int startX, int y, List<T> values)
        {
            foreach (var item in values.Select((value, index) => new { Value = value, Index = index }))
            {
                SetCell(startX + item.Index, y, item.Value);
            }
        }

        /// <summary>
        /// Sets a column of cells.
        /// </summary>
        /// <typeparam name="T">The element type of the cells.</typeparam>
        /// <param name="startX">Horizontal coordinate.</param>
        /// <param name="y">Vertical start coordinate.</param>
        /// <param name="values">Cell values from top to bottom</param>
        public void SetColumn<T>(int x, int startY, List<T> values)
        {
            foreach (var item in values.Select((value, index) => new { Value = value, Index = index }))
            {
                SetCell(x, startY + item.Index, item.Value);
            }
        }

        /// <summary>
        /// Sets a cell value.
        /// </summary>
        /// <typeparam name="T">The element type of the cell.</typeparam>
        /// <param name="x">Horizontal coordinate.</param>
        /// <param name="y">Vertical coordinate.</param>
        /// <param name="value">New cell value.</param>
        public void SetCell<T>(int x, int y, T value)
        {
            // TODO: fix quick fix
            ICell cell;
            try
            {
                var row = SelectedSheet.GetRow(y);
                if ((cell = row.GetCell(x)) == null)
                    cell = row.CreateCell(x);
            }
            catch
            {
                cell = SelectedSheet.CreateRow(y).CreateCell(x);
            }

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    cell.SetCellValue(Convert.ToBoolean(value));
                    break;
                case TypeCode.String:
                    cell.SetCellValue(Convert.ToString(value));
                    break;
                case TypeCode.Int32:
                case TypeCode.Double:
                    cell.SetCellValue(Convert.ToDouble(value));
                    break;
                default:
                    throw new ArgumentException($"{Type.GetTypeCode(value.GetType()).ToString()} is not a supported value type", "value");
            }
        }

        /// <summary>
        /// Saves the Workbook with .xlsx extension
        /// </summary>
        /// <param name="fileLocation">File location.</param>
        /// <param name="fileName">File name.</param>
        public void Save(string fileLocation, string fileName)
        {
            // create file stream
            using (var fs = File.Create($"{fileLocation}\\{fileName}.xlsx"))
            {
                // write to file
                Workbook.Write(fs);
                // close file
                fs.Close();
            }
        }

        // TODO: Load
        //public static Excel Load(string path)
        //{
        //}
    }
}
