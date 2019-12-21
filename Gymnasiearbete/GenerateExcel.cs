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
            int currentColums = 0;


            // For each SearchTypeResult in current GraphOptimizationResult
            foreach (var searchTypeResult in testResult.GraphOptimizationResults[0].SearchTypeResults)
            {
                // write SerchType
                excel.SetCell(0, currentRow, searchTypeResult.SearchType.ToString());
                // Write sizes
                for (int i = 0; i < searchTypeResult.OpennessResults[0].SizeResults.Count; i++)
                {
                    excel.SetCell(i + 1, currentRow, searchTypeResult.OpennessResults[0].SizeResults[i].GraphSize);
                }

                currentRow++;

                // For each OpennessResult in current SerchTypeResult
                foreach (var opennessResult in searchTypeResult.OpennessResults)
                {
                    // write openness
                    excel.SetCell(0, currentRow, opennessResult.Openness);

                    // Write results
                    // For each SizeResult in current OpennesResult
                    int index = 0;
                    foreach (var sizeResult in opennessResult.SizeResults)
                    {
                        // write mean search time
                        excel.SetCell(index + 1, currentRow, sizeResult.AverageSearchResult.MeanSearchResult.SearchTime);
                        index++;
                    }

                    currentRow++;
                }
            }

            return excel;

            //// For each OpennesResults
            //foreach (var opennessResults in testResults.OpennesResults)
            //{
            //    int dataWidth = 0;

            //    // write graph openness
            //    excel.SetCell(currentColums, currentRow, opennessResults.Openness);
            //    currentRow++;

            //    // For each SizeResults
            //    foreach (var sizeResults in opennessResults.SizeResults)
            //    {
            //        // write graph size
            //        excel.SetCell(currentColums + 0, currentRow, sizeResults.GraphSize);

            //        // current row for averageSearchTypeResults
            //        int currentSubRow = currentRow;

            //        int aIndex = 0;
            //        foreach (var averageSearchTypeResult in sizeResults.AverageSearchTypeResults)
            //        {
            //            // write search type
            //            excel.SetCell(currentColums + 1, currentSubRow, averageSearchTypeResult.SearchType.ToString());

            //            // write mean result types
            //            excel.SetCell(currentColums + 2, currentSubRow + 0, "Mean Search Time");
            //            excel.SetCell(currentColums + 2, currentSubRow + 1, "Mean Explored Nodes");
            //            excel.SetCell(currentColums + 2, currentSubRow + 2, "Mean Explored Ratio");
            //            // write mean results
            //            excel.SetCell(currentColums + 3, currentSubRow + 0, averageSearchTypeResult.MeanSearchResult.SearchTime);
            //            excel.SetCell(currentColums + 3, currentSubRow + 1, averageSearchTypeResult.MeanSearchResult.ExplordedNodes);
            //            excel.SetCell(currentColums + 3, currentSubRow + 2, averageSearchTypeResult.MeanSearchResult.ExploredRatio);

            //            // write median result types
            //            excel.SetCell(currentColums + 2, currentSubRow + 3, "Median Search Time");
            //            excel.SetCell(currentColums + 2, currentSubRow + 4, "Median Explored Nodes");
            //            excel.SetCell(currentColums + 2, currentSubRow + 5, "Median Explored Ratio");
            //            // write median results
            //            excel.SetCell(currentColums + 3, currentSubRow + 3, averageSearchTypeResult.MedianSearchResult.SearchTime);
            //            excel.SetCell(currentColums + 3, currentSubRow + 4, averageSearchTypeResult.MedianSearchResult.ExplordedNodes);
            //            excel.SetCell(currentColums + 3, currentSubRow + 5, averageSearchTypeResult.MedianSearchResult.ExploredRatio);

            //            currentSubRow += 6;

            //            // thin divider
            //            StyleRow(excel, currentColums + 1, 3, currentSubRow - 1, thinBorder);

            //            aIndex++;
            //        }


            //        // For each SizeRepetResults
            //        foreach (var sizeRepeatResults in sizeResults.SizeRepeatResults)
            //        {
            //            // write graph size repeat
            //            excel.SetCell(currentColums + 4, currentRow, sizeRepeatResults.GraphSizeRepet);

            //            // For each SearchTypeResults
            //            foreach (var searchTypeResults in sizeRepeatResults.SearchTypeResults)
            //            {
            //                // write search type
            //                excel.SetCell(currentColums + 5, currentRow, searchTypeResults.SearchType.ToString());

            //                // write result types
            //                excel.SetCell(currentColums + 6, currentRow + 0, "Search Time");
            //                excel.SetCell(currentColums + 6, currentRow + 1, "Explored Nodes");
            //                excel.SetCell(currentColums + 6, currentRow + 2, "Explored Ratio");

            //                // For each SearchResult
            //                int sIndex = 0;
            //                foreach (var searchResult in searchTypeResults.SearchResults)
            //                {
            //                    // write results
            //                    excel.SetCell(currentColums + 7 + sIndex, currentRow + 0, searchResult.SearchTime);
            //                    excel.SetCell(currentColums + 7 + sIndex, currentRow + 1, searchResult.ExplordedNodes);
            //                    excel.SetCell(currentColums + 7 + sIndex, currentRow + 2, searchResult.ExploredRatio);

            //                    sIndex++;
            //                }
            //                currentRow += 3;

            //                // calculate dataWidth
            //                if (dataWidth == 0)
            //                    dataWidth = 7 + sIndex;

            //                // thin divider
            //                StyleRow(excel, currentColums + 5, dataWidth - 5, currentRow - 1, thinBorder);
            //            }
            //            // medium divider
            //            StyleRow(excel, currentColums + 4, dataWidth - 4, currentRow - 1, mediumBorder);
            //        }
            //        // thick divider
            //        StyleRow(excel, currentColums + 1, dataWidth - 3, currentRow - 1, thickBorder);
            //    }
            //    // thick top divider
            //    StyleRow(excel, currentColums, dataWidth, 0, thickBorder);


            //    currentRow = 0;
            //    currentColums += dataWidth + 1;
            //}

            //return excel;
        }
    }
}
