using Gymnasiearbete.Test;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;

namespace Gymnasiearbete
{
    static class GenerateExcel
    {
        private enum ResultType { MeanSearchTime, MedianSearchTime, MeanExploredNodes, MedianExploredNodes, MeanExploredRatio, MedianExploredRatio, MeanGraphPruningTime, MedianGraphPruningTime }

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
                case ResultType.MeanGraphPruningTime:
                    return sizeResult.AverageGraphPruningTime.MeanPruningTime;
                case ResultType.MedianGraphPruningTime:
                    return sizeResult.AverageGraphPruningTime.MedianPruningTime;
                default:
                    throw new NotImplementedException($"{resultType.ToString()} is not implemented");
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
            var PruningTopBarStyle = excel.CreateStyle(new SimpleCellStyle
            {
                BorderBottom = BorderStyle.Thin,
                BorderTop = BorderStyle.Thick,
                FillForegroundColor = NPOI.HSSF.Util.HSSFColor.CornflowerBlue.Index,
                FillPattern = FillPattern.SolidForeground,
            });
            var PruningSideBarStyle = excel.CreateStyle(new SimpleCellStyle
            {
                BorderRight = BorderStyle.Thin,
                BorderLeft = BorderStyle.Medium,
                FillForegroundColor = NPOI.HSSF.Util.HSSFColor.CornflowerBlue.Index,
                FillPattern = FillPattern.SolidForeground,
            });
            var PruningTitleStyle = excel.CreateStyle(new SimpleCellStyle
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

            int dataHeight = testResult.GraphPruningResults[0].PathfindingAlgorithmResults[0].ComplexityResults.Count + 1;
            int dataWidth = testResult.GraphPruningResults[0].PathfindingAlgorithmResults[0].ComplexityResults[0].SizeResults.Count + 1;

            // For each GraphPruningResult
            foreach (var graphPruningResult in testResult.GraphPruningResults)
            {
                // Pruning time
                excel.SetCell(currentColumn, currentRow, $"Graph pruning: {graphPruningResult.PruningAlgorithm}", titleStyle);
                currentColumn++;
                WriteDataChunk(excel, currentColumn            , currentRow, ResultType.MeanGraphPruningTime  , graphPruningResult.PathfindingAlgorithmResults[0].ComplexityResults, PruningTitleStyle, PruningTopBarStyle, PruningSideBarStyle);
                WriteDataChunk(excel, currentColumn + dataWidth, currentRow, ResultType.MedianGraphPruningTime, graphPruningResult.PathfindingAlgorithmResults[0].ComplexityResults, PruningTitleStyle, PruningTopBarStyle, PruningSideBarStyle);
                currentRow += dataHeight;

                currentColumn = 0;

                // For each PathfindingAlgorithmResult in current GraphPruningResult
                foreach (var pathfindingAlgorithmResult in graphPruningResult.PathfindingAlgorithmResults)
                {
                    // write PathfindingAlgorithm
                    excel.SetCell(currentColumn, currentRow, $"Path-finding: {pathfindingAlgorithmResult.PathfindingAlgorithm.ToString()}, Graph pruning: {graphPruningResult.PruningAlgorithm}", titleStyle);
                    currentColumn++;

                    // Write all PathfindingAlgorithm data
                    foreach (ResultType resultType in Enum.GetValues(typeof(ResultType)))
                    {
                        if (resultType == ResultType.MeanGraphPruningTime || resultType == ResultType.MedianGraphPruningTime)
                            continue;

                        WriteDataChunk(excel, currentColumn, currentRow, resultType, pathfindingAlgorithmResult.ComplexityResults, PathfindingTitleStyle, PathfindingTopBarStyle, PathfindingSideBarStyle);
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
