using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Gymnasiearbete
{
    class Excel
    {
        public IWorkbook Workbook{ get; }
        public ISheet SelectedSheet { get; private set; }
        // TODO: Default style

        public Excel()
        {
            Workbook = new XSSFWorkbook();
        }

        /// <summary>
        /// Sets a sheet as the selected sheet.
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
        /// Sets a row of cells in the selected sheet.
        /// </summary>
        /// <typeparam name="T">The element type of the cells</typeparam>
        /// <param name="startX">Horizontal start coordinate.</param>
        /// <param name="y">Vertical coordinate.</param>
        /// <param name="values">Cell values from left to right</param>
        public void SetRow<T>(int startX, int y, List<T> values, SimpleCellStyle style = null)
        {
            foreach (var item in values.Select((value, index) => new { Value = value, Index = index }))
            {
                SetCell(startX + item.Index, y, item.Value, style);
            }
        }

        /// <summary>
        /// Sets a column of cells in the selected sheet.
        /// </summary>
        /// <typeparam name="T">The element type of the cells.</typeparam>
        /// <param name="startX">Horizontal coordinate.</param>
        /// <param name="y">Vertical start coordinate.</param>
        /// <param name="values">Cell values from top to bottom</param>
        /// <param name="style">The cells style. If it is set to null, it takes the default style</param>
        public void SetColumn<T>(int x, int startY, List<T> values, SimpleCellStyle style = null)
        {
            foreach (var item in values.Select((value, index) => new { Value = value, Index = index }))
            {
                SetCell(x, startY + item.Index, item.Value, style);
            }
        }

        /// <summary>
        /// Sets a cell value in the selected sheet
        /// </summary>
        /// <typeparam name="T">The element type of the cell.</typeparam>
        /// <param name="x">Horizontal coordinate.</param>
        /// <param name="y">Vertical coordinate.</param>
        /// <param name="value">New cell value.</param>
        /// <param name="style">The cell style. If it is set to null, it takes the default style</param>
        public void SetCell<T>(int x, int y, T value, SimpleCellStyle style = null)
        {
            var cell = CreateCell(x, y);

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

            // set cell style
            if (style != null)
                cell.CellStyle = CreateStyle(style);
        }


        private ICell CreateCell(int x, int y)
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

            return cell;
        }

        private ICell GetCell(int x, int y)
        {
            // TODO: fix quick fix
            ICell cell;
            try
            {
                var row = SelectedSheet.GetRow(y);
                if ((cell = row.GetCell(x)) == null)
                    return null;
            }
            catch
            {
                return null;
            }

            return cell;
        }

        /// <summary>
        /// Sets a sell style in the selected sheet.
        /// </summary>
        /// <param name="x">Horizontal coordinate.</param>
        /// <param name="y">Vertical coordinate.</param>
        /// <param name="style">The cell style</param>
        public void SetStyle(int x, int y, SimpleCellStyle style)
        {
            CreateCell(x, y).CellStyle = CreateStyle(style);
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

        /// <summary>
        /// Creates a new ICellStyle from a SimpleCellStyle object.
        /// </summary>
        /// <param name="style">SimpleCellStyle object</param>
        /// <returns>The created ICellStyle</returns>
        private ICellStyle CreateStyle(SimpleCellStyle style)
        {
            var s = Workbook.CreateCellStyle();

            s.BorderTop = style.BorderTop;
            s.BorderRight = style.BorderRight;
            s.BorderBottom = style.BorderBottom;
            s.BorderLeft = style.BorderLeft;
            s.TopBorderColor = style.TopBorderColor;
            s.RightBorderColor = style.RightBorderColor;
            s.BottomBorderColor = style.BottomBorderColor;
            s.LeftBorderColor = style.LeftBorderColor;

            s.FillForegroundColor = style.FillForegroundColor;
            s.FillBackgroundColor = style.FillBackgroundColor;

            s.FillPattern = style.FillPattern;

            s.Alignment = style.Alignment;
            s.VerticalAlignment = style.VerticalAlignment;

            s.IsHidden = style.IsHidden;
            s.IsLocked = style.IsLocked;
            s.WrapText = style.WrapText;
            s.ShrinkToFit = style.ShrinkToFit;

            s.Rotation = style.Rotation;
            s.Indention = style.Indention;
            s.DataFormat = style.DataFormat;

            s.SetFont(style.Font);

            return s;
        }
    }

    class SimpleCellStyle
    {
        public BorderStyle BorderTop { get; set; }
        public BorderStyle BorderRight { get; set; }
        public BorderStyle BorderBottom { get; set; }
        public BorderStyle BorderLeft { get; set; }
        public short TopBorderColor { get; set; }
        public short RightBorderColor { get; set; }
        public short BottomBorderColor { get; set; }
        public short LeftBorderColor { get; set; }

        public short FillForegroundColor { get; set; }
        public short FillBackgroundColor { get; set; }

        public FillPattern FillPattern { get; set; }

        public HorizontalAlignment Alignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }

        public bool IsHidden { get; set; }
        public bool IsLocked { get; set; }
        public bool WrapText { get; set; }
        public bool ShrinkToFit { get; set; }

        public short Rotation { get; set; }
        public short Indention { get; set; }
        public short DataFormat { get; set; }

        public IFont Font { get; set; }
    }
}
