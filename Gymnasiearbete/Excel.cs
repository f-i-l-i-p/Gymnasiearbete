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
    class RowData
    {
        public int Row { get; set; }
        public List<int> Cells { get; set; } = new List<int>();
    }

    class SheetData
    {
        public string Name { get; }
        public List<RowData> RowData { get; set; }= new List<RowData>();

        public SheetData(string Name)
        {
            this.Name = Name;
        }
    }

    class Excel
    {
        public IWorkbook Workbook{ get; }
        public ISheet SelectedSheet { get; private set; }
        // TODO: Default style

        private List<SheetData> SheetData { get; }
        private SheetData SelectedSheetData
        {
            get
            {
                var sd = SheetData.Find(sheet => sheet.Name == SelectedSheet.SheetName);
                if (sd == null) throw new Exception($"SheetData can not be found");
                return sd;
            }
        }

        public Excel()
        {
            Workbook = new XSSFWorkbook();
            SheetData = new List<SheetData>();
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
        /// The new sheet will be set as selected if select = true.
        /// </summary>
        /// <param name="name">Sheet name.</param>
        /// <param name="select">Select the new sheet.</param>
        public void AddSheet(string name, bool select = true)
        {
            // Test if a sheet whith the same name already exists
            if (Workbook.GetSheet(name) != null)
                throw new ArgumentException($"A sheet with the name \"{name}\" already exists", "name");

            // create sheet
            var newSheet = string.IsNullOrEmpty(name) ? Workbook.CreateSheet() : Workbook.CreateSheet(name);
            // add sheet data
            SheetData.Add(new SheetData(name));

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
        public void SetRow<T>(int startX, int y, List<T> values, ICellStyle style = null)
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
        public void SetColumn<T>(int x, int startY, List<T> values, ICellStyle style = null)
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
        public void SetCell<T>(int x, int y, T value, ICellStyle style = null)
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

            // TODO: This is too slow
            // set cell style
            if (style != null)
                cell.CellStyle = style;
        }


        private ICell CreateCell(int x, int y)
        {
            ICell cell;

            var rowData = SelectedSheetData.RowData.Find(rd => rd.Row == y); ;

            // Test if row data was found
            if (rowData != null)
            {
                var row = SelectedSheet.GetRow(y);

                // test if cell x is registered in row y
                if (rowData.Cells != null && rowData.Cells.Contains(x))
                    cell = row.GetCell(x);
                else
                {
                    // create new cell
                    cell = row.CreateCell(x);
                    // add to row data
                    rowData.Cells.Add(x);
                }

            }
            else
            {
                cell = SelectedSheet.CreateRow(y).CreateCell(x);

                SelectedSheetData.RowData.Add(new RowData
                {
                    Row = y,
                    Cells = new List<int> { x }
                });
            }

            return cell;
        }


        /// <summary>
        /// Sets a sell style in the selected sheet.
        /// </summary>
        /// <param name="x">Horizontal coordinate.</param>
        /// <param name="y">Vertical coordinate.</param>
        /// <param name="style">The cell style</param>
        public void SetStyle(int x, int y, ICellStyle style)
        {
            CreateCell(x, y).CellStyle = style;
        }

        /// <summary>
        /// Saves the Workbook with .xlsx extension
        /// </summary>
        /// <param name="fileLocation">File location.</param>
        /// <param name="fileName">File name.</param>
        public void Save(string fileLocation, string fileName)
        {
            // TODO: Om excel filen är öppen så blir de inge bra: System.IO.IOException: 'Processen kan inte komma åt filen C:\Users\FiJo0302\Projects\Gymnasiearbete\SavedGraphs\results.xlsx eftersom den används i en annan process.'

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
        public ICellStyle CreateStyle(SimpleCellStyle style)
        {
            var iCellStyle = Workbook.CreateCellStyle();

            iCellStyle.BorderTop = style.BorderTop;
            iCellStyle.BorderRight = style.BorderRight;
            iCellStyle.BorderBottom = style.BorderBottom;
            iCellStyle.BorderLeft = style.BorderLeft;
            iCellStyle.TopBorderColor = style.TopBorderColor;
            iCellStyle.RightBorderColor = style.RightBorderColor;
            iCellStyle.BottomBorderColor = style.BottomBorderColor;
            iCellStyle.LeftBorderColor = style.LeftBorderColor;

            iCellStyle.FillForegroundColor = style.FillForegroundColor;
            iCellStyle.FillBackgroundColor = style.FillBackgroundColor;

            iCellStyle.FillPattern = style.FillPattern;

            iCellStyle.Alignment = style.Alignment;
            iCellStyle.VerticalAlignment = style.VerticalAlignment;

            iCellStyle.IsHidden = style.IsHidden;
            iCellStyle.IsLocked = style.IsLocked;
            iCellStyle.WrapText = style.WrapText;
            iCellStyle.ShrinkToFit = style.ShrinkToFit;

            iCellStyle.Rotation = style.Rotation;
            iCellStyle.Indention = style.Indention;
            iCellStyle.DataFormat = style.DataFormat;

            iCellStyle.SetFont(style.Font);

            return iCellStyle;
        }

        /// <summary>
        /// Adds an extra style to all style values in the base style that have the default value.
        /// </summary>
        /// <param name="baseStyle">Base style.</param>
        /// <param name="extraStyle">Extra style that will be added to all default values in the base style.</param>
        public static void AddStyles(ICellStyle baseStyle, ICellStyle extraStyle)
        {
            if (baseStyle.BorderTop == BorderStyle.None) baseStyle.BorderTop = extraStyle.BorderTop;
            if (baseStyle.BorderRight == BorderStyle.None) baseStyle.BorderRight = extraStyle.BorderRight;
            if (baseStyle.BorderBottom == BorderStyle.None) baseStyle.BorderBottom = extraStyle.BorderBottom;
            if (baseStyle.BorderLeft == BorderStyle.None) baseStyle.BorderLeft = extraStyle.BorderLeft;
            if (baseStyle.TopBorderColor == 0) baseStyle.TopBorderColor = extraStyle.TopBorderColor;
            if (baseStyle.RightBorderColor == 0) baseStyle.RightBorderColor = extraStyle.RightBorderColor;
            if (baseStyle.BottomBorderColor == 0) baseStyle.BottomBorderColor = extraStyle.BottomBorderColor;
            if (baseStyle.LeftBorderColor == 0) baseStyle.LeftBorderColor = extraStyle.LeftBorderColor;

            if (baseStyle.FillForegroundColor == 0) baseStyle.FillForegroundColor = extraStyle.FillForegroundColor;
            if (baseStyle.FillBackgroundColor == 0) baseStyle.FillBackgroundColor = extraStyle.FillBackgroundColor;

            if (baseStyle.FillPattern == FillPattern.NoFill) baseStyle.FillPattern = extraStyle.FillPattern;

            if (baseStyle.Alignment == HorizontalAlignment.General) baseStyle.Alignment = extraStyle.Alignment;
            if (baseStyle.VerticalAlignment == VerticalAlignment.Top) baseStyle.VerticalAlignment = extraStyle.VerticalAlignment;

            if (baseStyle.IsHidden == false) baseStyle.IsHidden = extraStyle.IsHidden;
            if (baseStyle.IsLocked == false) baseStyle.IsLocked = extraStyle.IsLocked;
            if (baseStyle.WrapText == false) baseStyle.WrapText = extraStyle.WrapText;
            if (baseStyle.ShrinkToFit == false) baseStyle.ShrinkToFit = extraStyle.ShrinkToFit;

            if (baseStyle.Rotation == 0) baseStyle.Rotation = extraStyle.Rotation;
            if (baseStyle.Indention == 0) baseStyle.Indention = extraStyle.Indention;
            if (baseStyle.DataFormat == 0) baseStyle.DataFormat = extraStyle.DataFormat;
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
