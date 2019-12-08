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

                    // For each SizeRepetResults
                    foreach (var sizeRepeatResults in sizeResults.SizeRepeatResults)
                    {
                        // write graph size repeat
                        excel.SetCell(currentColums + 1, currentRow, sizeRepeatResults.GraphSizeRepet);

                        // For each SearchTypeResults
                        foreach (var searchTypeResults in sizeRepeatResults.SearchTypeResults)
                        {
                            // write search type
                            excel.SetCell(currentColums + 2, currentRow, searchTypeResults.SearchType.ToString());

                            // write result types
                            excel.SetCell(currentColums + 3, currentRow + 0, "Search Time");
                            excel.SetCell(currentColums + 3, currentRow + 1, "Explored Nodes");
                            excel.SetCell(currentColums + 3, currentRow + 2, "Explored Ratio");

                            // For each SearchResult
                            int index = 0;
                            foreach (var searchResult in searchTypeResults.SearchResults)
                            {
                                // write results
                                excel.SetCell(currentColums + 4 + index, currentRow + 0, searchResult.SearchTime);
                                excel.SetCell(currentColums + 4 + index, currentRow + 1, searchResult.ExplordedNodes);
                                excel.SetCell(currentColums + 4 + index, currentRow + 2, searchResult.ExploredRatio);

                                index++;
                            }
                            currentRow += 3;

                            // calculate dataWidth
                            if (dataWidth == 0)
                                dataWidth = 4 + index;

                            // thin divider
                            StyleRow(excel, currentColums + 2, dataWidth - 2, currentRow - 1, thinStyle);
                        }
                        // medium divider
                        StyleRow(excel, currentColums + 1, dataWidth - 1, currentRow - 1, mediumStyle);
                    }
                    // thick divider
                    StyleRow(excel, currentColums + 0, dataWidth - 0, currentRow - 1, thickStyle);
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
