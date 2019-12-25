using Gymnasiearbete.Test;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymnasiearbete
{
    static class GenerateExcel
    {
        private static void StyleRow(Excel excel, int xStart, int xLength, int y, ICellStyle style)
        {
            for (int i = xStart; i < xStart + xLength; i++)
            {
                excel.SetStyle(i, y, style);
            }
        }

        private enum ResultType { MeanSearchTime, MedianSearchTime, MeanExploredNodes, MedianExploredNodes, MeanExploredRatio, MedianExploredRatio, MeanGraphOptimizationTime, MedianGraphOptimizationTime }

        /// <summary>
        /// Returns a search result value from a AverageSearchResult.
        /// </summary>
        /// <param name="resultType">Result type to return.</param>
        /// <param name="averageSearchResult">AverageSearchResult to find search result value in.</param>
        /// <returns>The value of the search result with the matching result type.</returns>
        private static object GetDataFromDataType(ResultType resultType, SizeResult sizeResult)
        {
            switch (resultType)
            {
                case ResultType.MeanSearchTime:
                    return sizeResult.AverageSearchResult.MeanSearchResult.SearchTime;
                case ResultType.MedianSearchTime:
                    return sizeResult.AverageSearchResult.MedianSearchResult.SearchTime;
                case ResultType.MeanExploredNodes:
                    return sizeResult.AverageSearchResult.MeanSearchResult.ExplordedNodes;
                case ResultType.MedianExploredNodes:
                    return sizeResult.AverageSearchResult.MedianSearchResult.ExplordedNodes;
                case ResultType.MeanExploredRatio:
                    return sizeResult.AverageSearchResult.MeanSearchResult.ExploredRatio;
                case ResultType.MedianExploredRatio:
                    return sizeResult.AverageSearchResult.MedianSearchResult.ExploredRatio;
                case ResultType.MeanGraphOptimizationTime:
                    return sizeResult.AverageGraphOpimizationTime.MeanOptimizationTime;
                case ResultType.MedianGraphOptimizationTime:
                    return sizeResult.AverageGraphOpimizationTime.MedianOpimizatonTime;
                default:
                    throw new ArgumentException($"{resultType.ToString()} is not a supported ResultType type", "resultType");
            }
        }

        /// <summary>
        /// Writes data to a excel from complexity results.
        /// </summary>
        /// <param name="excel">Excel to write data to.</param>
        /// <param name="xStart">X coordinate to start from.</param>
        /// <param name="yStart">Ý coordinate to start from.</param>
        /// <param name="resultType">Result type to write.</param>
        /// <param name="complexityResults">Complexity results containing all data to be written.</param>
        private static void WriteDataChunk(Excel excel, int xStart, int yStart, ResultType resultType, List<ComplexityResult> complexityResults, ICellStyle titleStyle, ICellStyle topBarStyle, ICellStyle sideBarStyle)
        {
            // write title
            excel.SetCell(xStart, yStart, resultType.ToString(), titleStyle);

            // Return if there are no complexity results
            if (complexityResults == null || complexityResults.Count == 0)
                return;
            // Return if there are no size results
            if (complexityResults[0].SizeResults == null || complexityResults[0].SizeResults.Count == 0)
                return;
            
            // Write sizes
            int column = xStart + 1;
            foreach (var sizeResult in complexityResults[0].SizeResults)
            {
                // write data
                excel.SetCell(column, yStart, sizeResult.GraphSize, topBarStyle);
                column++;
            }

            // Write complexity & results
            int row = yStart + 1;
            foreach (var complexityResult in complexityResults)
            {
                // write complexity
                excel.SetCell(xStart, row, complexityResult.Complexity, sideBarStyle);

                // Write results
                // For each SizeResult in current ComplexityResult
                column = 0;
                foreach (var sizeResult in complexityResult.SizeResults)
                {
                    // write mean search time
                    excel.SetCell(xStart + column + 1, row, GetDataFromDataType(resultType, sizeResult));
                    column++;
                }
                row++;
            }

        }

        /// <summary>
        /// Generates a excel containing data from the test result.
        /// </summary>
        /// <param name="testResult">TestResult to create excel from.</param>
        /// <returns>The excel object.</returns>
        public static Excel Generate(TestResult testResult)
        {
            // Create excel with one empty sheet
            var excel = new Excel();
            excel.AddSheet("Data");

            // Create styles
            var PathfindingTopBarStyle = excel.CreateStyle(new SimpleCellStyle
            {
                BorderBottom = BorderStyle.Thin,
                BorderTop = BorderStyle.Thick,
                FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index,
                FillPattern = FillPattern.SolidForeground,
            });
            var PathfindingSideBarStyle = excel.CreateStyle(new SimpleCellStyle
            {
                BorderRight = BorderStyle.Thin,
                BorderLeft = BorderStyle.Medium,
                FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index,
                FillPattern = FillPattern.SolidForeground,
            });
            var PathfindingTitleStyle = excel.CreateStyle(new SimpleCellStyle
            {
                BorderBottom = BorderStyle.Thin,
                BorderTop = BorderStyle.Thick,
                BorderRight = BorderStyle.Thin,
                BorderLeft = BorderStyle.Medium,
                FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index,
                FillPattern = FillPattern.SolidForeground,
            });
            var OptimizeTopBarStyle = excel.CreateStyle(new SimpleCellStyle
            {
                BorderBottom = BorderStyle.Thin,
                BorderTop = BorderStyle.Thick,
                FillForegroundColor = NPOI.HSSF.Util.HSSFColor.CornflowerBlue.Index,
                FillPattern = FillPattern.SolidForeground,
            });
            var OptimizeSideBarStyle = excel.CreateStyle(new SimpleCellStyle
            {
                BorderRight = BorderStyle.Thin,
                BorderLeft = BorderStyle.Medium,
                FillForegroundColor = NPOI.HSSF.Util.HSSFColor.CornflowerBlue.Index,
                FillPattern = FillPattern.SolidForeground,
            });
            var OptimizeTitleStyle = excel.CreateStyle(new SimpleCellStyle
            {
                BorderBottom = BorderStyle.Thin,
                BorderTop = BorderStyle.Thick,
                BorderRight = BorderStyle.Thin,
                BorderLeft = BorderStyle.Medium,
                FillForegroundColor = NPOI.HSSF.Util.HSSFColor.CornflowerBlue.Index,
                FillPattern = FillPattern.SolidForeground,
            });
            var titleStyle = excel.CreateStyle(new SimpleCellStyle
            {
                BorderTop = BorderStyle.Thick,
            });

            int currentRow = 0;
            int currentColumn = 0;

            // TODO: not constants
            int dataWidth = 16;
            int dataHeight = 12;

            // For each GraphOptimizationResult
            foreach (var graphOptimizationResult in testResult.GraphOptimizationResults)
            {
                // Optimization time
                excel.SetCell(currentColumn, currentRow, $"Graph optimization: {graphOptimizationResult.OptimizationType}", titleStyle);
                currentColumn++;
                WriteDataChunk(excel, currentColumn            , currentRow, ResultType.MeanGraphOptimizationTime  , graphOptimizationResult.SearchTypeResults[0].ComplexityResults, OptimizeTitleStyle, OptimizeTopBarStyle, OptimizeSideBarStyle);
                WriteDataChunk(excel, currentColumn + dataWidth, currentRow, ResultType.MedianGraphOptimizationTime, graphOptimizationResult.SearchTypeResults[0].ComplexityResults, OptimizeTitleStyle, OptimizeTopBarStyle, OptimizeSideBarStyle);
                currentRow += dataHeight;

                currentColumn = 0;

                // For each SearchTypeResult in current GraphOptimizationResult
                foreach (var searchTypeResult in graphOptimizationResult.SearchTypeResults)
                {
                    // write SearchType
                    excel.SetCell(currentColumn, currentRow, $"Path-finding: {searchTypeResult.SearchType.ToString()}, Graph optimization: {graphOptimizationResult.OptimizationType}", titleStyle);
                    currentColumn++;

                    // Write all SearchType data
                    foreach (ResultType resultType in Enum.GetValues(typeof(ResultType)))
                    {
                        if (resultType == ResultType.MeanGraphOptimizationTime || resultType == ResultType.MedianGraphOptimizationTime)
                            continue;

                        WriteDataChunk(excel, currentColumn, currentRow, resultType, searchTypeResult.ComplexityResults, PathfindingTitleStyle, PathfindingTopBarStyle, PathfindingSideBarStyle);
                        currentColumn += dataWidth;
                    }

                    currentRow += dataHeight;
                    currentColumn = 0;
                }

                // add empty row
                currentRow++;
            }

            return excel;
        }
    }
}
