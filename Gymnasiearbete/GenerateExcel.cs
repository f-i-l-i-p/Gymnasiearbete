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

        private enum ResultType { MeanSearchTime, MedianSearchTime, MeanExploredNodes, MedianExploredNodes, MeanExploredRatio, MedieanExploredRatio }

        /// <summary>
        /// Returns a search result value from a AverageSearchResult.
        /// </summary>
        /// <param name="resultType">Result type to return.</param>
        /// <param name="averageSearchResult">AverageSearchResult to find search result value in.</param>
        /// <returns>The value of the search result with the matching result type.</returns>
        private static object GetDataFromDataType(ResultType resultType, AverageSearchResult averageSearchResult)
        {
            switch (resultType)
            {
                case ResultType.MeanSearchTime:
                    return averageSearchResult.MeanSearchResult.SearchTime;
                case ResultType.MedianSearchTime:
                    return averageSearchResult.MedianSearchResult.SearchTime;
                case ResultType.MeanExploredNodes:
                    return averageSearchResult.MeanSearchResult.ExplordedNodes;
                case ResultType.MedianExploredNodes:
                    return averageSearchResult.MedianSearchResult.ExplordedNodes;
                case ResultType.MeanExploredRatio:
                    return averageSearchResult.MeanSearchResult.ExploredRatio;
                case ResultType.MedieanExploredRatio:
                    return averageSearchResult.MedianSearchResult.ExploredRatio;
                default:
                    throw new ArgumentException($"{resultType.ToString()} is not a supported ResultType type", "resultType");
            }
        }

        /// <summary>
        /// Writes data to a excel from openness results.
        /// </summary>
        /// <param name="excel">Excel to write data to.</param>
        /// <param name="xStart">X coordinate to start from.</param>
        /// <param name="yStart">Ý coordinate to start from.</param>
        /// <param name="resultType">Result type to write.</param>
        /// <param name="opennessResults">Openness results containing all data to be written.</param>
        private static void WriteDataChunk(Excel excel, int xStart, int yStart, ResultType resultType, List<OpennessResult> opennessResults, ICellStyle titleStyle, ICellStyle topBarStyle, ICellStyle sideBarStyle)
        {
            // write title
            excel.SetCell(xStart, yStart, resultType.ToString(), titleStyle);

            // Return if there are no openness results
            if (opennessResults == null || opennessResults.Count == 0)
                return;
            // Return if there are no size results
            if (opennessResults[0].SizeResults == null || opennessResults[0].SizeResults.Count == 0)
                return;
            
            // Write sizes
            int column = xStart + 1;
            foreach (var sizeResult in opennessResults[0].SizeResults)
            {
                // write data
                excel.SetCell(column, yStart, sizeResult.GraphSize, topBarStyle);
                column++;
            }

            // Write openness & results
            int row = yStart + 1;
            foreach (var opennessResult in opennessResults)
            {
                // write openness
                excel.SetCell(xStart, row, opennessResult.Openness, sideBarStyle);

                // Write results
                // For each SizeResult in current OpennesResult
                column = 0;
                foreach (var sizeResult in opennessResult.SizeResults)
                {
                    // write mean search time
                    excel.SetCell(xStart + column + 1, row, GetDataFromDataType(resultType, sizeResult.AverageSearchResult));
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
            var chunkTopBarStyle = excel.CreateStyle(new SimpleCellStyle
            {
                BorderBottom = BorderStyle.Thin,
                BorderTop = BorderStyle.Thick,
                FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index,
                FillPattern = FillPattern.SolidForeground,
            });
            var chunkSideBarStyle = excel.CreateStyle(new SimpleCellStyle
            {
                BorderRight = BorderStyle.Thin,
                BorderLeft = BorderStyle.Medium,
                FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index,
                FillPattern = FillPattern.SolidForeground,
            });
            var chunkTitleStyle = excel.CreateStyle(new SimpleCellStyle
            {
                BorderBottom = BorderStyle.Thin,
                BorderTop = BorderStyle.Thick,
                BorderRight = BorderStyle.Thin,
                BorderLeft = BorderStyle.Medium,
                FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index,
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
                // For each SearchTypeResult in current GraphOptimizationResult
                foreach (var searchTypeResult in graphOptimizationResult.SearchTypeResults)
                {
                    // write SearchType
                    excel.SetCell(currentColumn, currentRow, $"Path-finding: {searchTypeResult.SearchType.ToString()}, Graph optimization: {graphOptimizationResult.OptimizationType}", titleStyle);
                    currentColumn++;

                    // Write all SearchType data
                    foreach (ResultType resultType in Enum.GetValues(typeof(ResultType)))
                    {
                        WriteDataChunk(excel, currentColumn, currentRow, resultType, searchTypeResult.OpennessResults, chunkTitleStyle, chunkTopBarStyle, chunkSideBarStyle);
                        currentColumn += dataWidth;
                    }

                    currentRow += dataHeight;
                    currentColumn = 0;
                }
            }

            return excel;
        }
    }
}
