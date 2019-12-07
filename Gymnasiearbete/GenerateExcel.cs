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
        public static Excel Generate(TestResults testResults)
        {
            int width = 7;

            // Create excel with one empty sheet
            var excel = new Excel();
            excel.AddSheet("Data");

            // Create styles
            var thinStyle = excel.CreateStyle(new SimpleCellStyle { BorderBottom = BorderStyle.Thin });
            var mediumStyle = excel.CreateStyle(new SimpleCellStyle { BorderBottom = BorderStyle.Medium });
            var thickStyle = excel.CreateStyle(new SimpleCellStyle { BorderBottom = BorderStyle.Thick });

            int currentRow = 0;

            foreach (var sizeResults in testResults.OpennesResults[0].SizeResults)
            {
                // set graph size
                excel.SetCell(0, currentRow, sizeResults.GraphSize);

                foreach (var sizeRepetResult in sizeResults.SizeRepetResults)
                {
                    // set size repeat
                    excel.SetCell(1, currentRow, sizeRepetResult.GraphSizeRepet);

                    foreach (var searchTypeResults in sizeRepetResult.SearchTypeResults)
                    {
                        // set search type
                        excel.SetCell(2, currentRow, searchTypeResults.SearchType.ToString());

                        var SearchTimes = new List<double>();
                        var ExplordedNodes = new List<int>();
                        var ExplordedRatios = new List<double>();
                        foreach (var item in searchTypeResults.SearchResults)
                        {
                            SearchTimes.Add(item.SearchTime);
                            ExplordedNodes.Add(item.ExplordedNodes);
                            ExplordedRatios.Add(item.ExploredRatio);
                        }
                        // set search times
                        excel.SetCell(3, currentRow, "Search time");
                        excel.SetRow(4, currentRow, SearchTimes);
                        currentRow++;
                        // set explored nodes
                        excel.SetCell(3, currentRow, "Explored nodes");
                        excel.SetRow(4, currentRow, ExplordedNodes);
                        currentRow++;
                        // set explored ratio
                        excel.SetCell(3, currentRow, "Explored ratio");
                        excel.SetRow(4, currentRow, ExplordedRatios);
                        currentRow++;


                        for (int i = 2; i < width; i++)
                        {
                            excel.SetStyle(i, currentRow - 1, thinStyle);
                        }
                    }
                    for (int i = 1; i < width; i++)
                    {
                        excel.SetStyle(i, currentRow - 1, mediumStyle);
                    }
                }
                for (int i = 0; i < width; i++)
                {
                    excel.SetStyle(i, currentRow - 1, thickStyle);
                }
            }

            // auto sizes column 3
            excel.SelectedSheet.AutoSizeColumn(3);

            return excel;
        }
    }
}
