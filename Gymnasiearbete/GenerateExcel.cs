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
            }
            return null;
        }

        private static void WriteDataChunk(Excel excel, int xStart, int yStart, ResultType dataType, List<OpennessResult> opennessResults)
        {
            // write title
            excel.SetCell(xStart, yStart, dataType.ToString());

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
                excel.SetCell(column, yStart, sizeResult.GraphSize);
                column++;
            }

            // Write openness & results
            int row = yStart + 1;
            foreach (var opennessResult in opennessResults)
            {
                // write openness
                excel.SetCell(xStart, row, opennessResult.Openness);

                // Write results
                // For each SizeResult in current OpennesResult
                column = 0;
                foreach (var sizeResult in opennessResult.SizeResults)
                {
                    // write mean search time
                    excel.SetCell(xStart + column + 1, row, GetDataFromDataType(dataType, sizeResult.AverageSearchResult));
                    column++;
                }
                row++;
            }

        }

        public static Excel Generate(TestResult testResult)
        {
            // Create excel with one empty sheet
            var excel = new Excel();
            excel.AddSheet("Data");

            // Create styles
            var thinBorder = excel.CreateStyle(new SimpleCellStyle { BorderBottom = BorderStyle.Thin });
            var mediumBorder = excel.CreateStyle(new SimpleCellStyle { BorderBottom = BorderStyle.Medium });
            var thickBorder = excel.CreateStyle(new SimpleCellStyle { BorderBottom = BorderStyle.Thick });

            int currentRow = 0;
            int currentColumn = 0;

            int dataWidth = 16;
            int dataHeight = 12;

            // For each SearchTypeResult in current GraphOptimizationResult
            foreach (var searchTypeResult in testResult.GraphOptimizationResults[0].SearchTypeResults)
            {
                // write SearchType
                excel.SetCell(0, currentRow, searchTypeResult.SearchType.ToString());

                currentColumn = 1;
                // Write all SearchType data
                foreach (ResultType resultType in Enum.GetValues(typeof(ResultType)))
                {
                    WriteDataChunk(excel, currentColumn, currentRow, resultType, searchTypeResult.OpennessResults);
                    currentColumn += dataWidth;
                }

                currentRow += dataHeight;
            }

            return excel;
        }
    }
}
