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

        public static Excel Generate(TestResults testResults)
        {
            // Create excel with one empty sheet
            var excel = new Excel();
            excel.AddSheet("Data");

            // Create styles
            var thinStyle = excel.CreateStyle(new SimpleCellStyle { BorderBottom = BorderStyle.Thin });
            var mediumStyle = excel.CreateStyle(new SimpleCellStyle { BorderBottom = BorderStyle.Medium });
            var thickStyle = excel.CreateStyle(new SimpleCellStyle { BorderBottom = BorderStyle.Thick });

            int currentRow = 0;
            int currentColums = 0;

            // For each OpennesResults
            foreach (var opennessResults in testResults.OpennesResults)
            {
                int dataWidth = 0;

                // write graph openness
                excel.SetCell(currentColums, currentRow, opennessResults.Openness);
                currentRow++;

                // For each SizeResults
                foreach (var sizeResults in opennessResults.SizeResults)
                {
                    // write graph size
                    excel.SetCell(currentColums + 0, currentRow, sizeResults.GraphSize);

                    // current row for averageSearchTypeResults
                    int currentSubRow = currentRow;

                    int aIndex = 0;
                    foreach (var averageSearchTypeResult in sizeResults.AverageSearchTypeResults)
                    {
                        // write search type
                        excel.SetCell(currentColums + 1, currentSubRow, averageSearchTypeResult.SearchType.ToString());

                        // write mean result types
                        excel.SetCell(currentColums + 2, currentSubRow + 0, "Mean Search Time");
                        excel.SetCell(currentColums + 2, currentSubRow + 1, "Mean Explored Nodes");
                        excel.SetCell(currentColums + 2, currentSubRow + 2, "Mean Explored Ratio");
                        // write mean results
                        excel.SetCell(currentColums + 3, currentSubRow + 0, averageSearchTypeResult.MeanSearchResult.SearchTime);
                        excel.SetCell(currentColums + 3, currentSubRow + 1, averageSearchTypeResult.MeanSearchResult.ExplordedNodes);
                        excel.SetCell(currentColums + 3, currentSubRow + 2, averageSearchTypeResult.MeanSearchResult.ExploredRatio);

                        // write median result types
                        excel.SetCell(currentColums + 2, currentSubRow + 3, "Median Search Time");
                        excel.SetCell(currentColums + 2, currentSubRow + 4, "Median Explored Nodes");
                        excel.SetCell(currentColums + 2, currentSubRow + 5, "Median Explored Ratio");
                        // write median results
                        excel.SetCell(currentColums + 3, currentSubRow + 3, averageSearchTypeResult.MedianSearchResult.SearchTime);
                        excel.SetCell(currentColums + 3, currentSubRow + 4, averageSearchTypeResult.MedianSearchResult.ExplordedNodes);
                        excel.SetCell(currentColums + 3, currentSubRow + 5, averageSearchTypeResult.MedianSearchResult.ExploredRatio);

                        currentSubRow += 6;

                        aIndex++;
                    }


                    // For each SizeRepetResults
                    foreach (var sizeRepeatResults in sizeResults.SizeRepeatResults)
                    {
                        // write graph size repeat
                        excel.SetCell(currentColums + 4, currentRow, sizeRepeatResults.GraphSizeRepet);

                        // For each SearchTypeResults
                        foreach (var searchTypeResults in sizeRepeatResults.SearchTypeResults)
                        {
                            // write search type
                            excel.SetCell(currentColums + 5, currentRow, searchTypeResults.SearchType.ToString());

                            // write result types
                            excel.SetCell(currentColums + 6, currentRow + 0, "Search Time");
                            excel.SetCell(currentColums + 6, currentRow + 1, "Explored Nodes");
                            excel.SetCell(currentColums + 6, currentRow + 2, "Explored Ratio");

                            // For each SearchResult
                            int sIndex = 0;
                            foreach (var searchResult in searchTypeResults.SearchResults)
                            {
                                // write results
                                excel.SetCell(currentColums + 7 + sIndex, currentRow + 0, searchResult.SearchTime);
                                excel.SetCell(currentColums + 7 + sIndex, currentRow + 1, searchResult.ExplordedNodes);
                                excel.SetCell(currentColums + 7 + sIndex, currentRow + 2, searchResult.ExploredRatio);

                                sIndex++;
                            }
                            currentRow += 3;

                            // calculate dataWidth
                            if (dataWidth == 0)
                                dataWidth = 7 + sIndex;

                            // thin divider
                            StyleRow(excel, currentColums + 5, dataWidth - 5, currentRow - 1, thinStyle);
                        }
                        // medium divider
                        StyleRow(excel, currentColums + 4, dataWidth - 4, currentRow - 1, mediumStyle);
                    }
                    // thick divider
                    StyleRow(excel, currentColums + 3, dataWidth - 3, currentRow - 1, thickStyle);
                }
                // thick top divider
                StyleRow(excel, currentColums, dataWidth, 0, thickStyle);


                currentRow = 0;
                currentColums += dataWidth + 1;
            }

            return excel;
        }
    }
}
