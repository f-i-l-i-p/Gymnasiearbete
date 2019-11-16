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
        public static Excel Generate(List<TestResult> testResults)
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

            foreach (var testResult in testResults)
            {
                // set graph size
                excel.SetCell(0, currentRow, testResult.GraphSize);

                foreach (var graphResult in testResult.GraphResults.Select((value, index) => new { Value = value, Index = index }))
                {
                    // set size repeat
                    excel.SetCell(1, currentRow, graphResult.Index);

                    foreach (var searchTypeResult in graphResult.Value.SearchTypesResults)
                    {
                        // set search type
                        excel.SetCell(2, currentRow, searchTypeResult.SearchType.ToString());

                        var SearchTimes = new List<double>();
                        var ExplordedNodes = new List<int>();
                        var ExplordedRatios = new List<double>();
                        foreach (var item in searchTypeResult.SearchResults)
                        {
                            SearchTimes.Add(item.SearchTime);
                            ExplordedNodes.Add(item.ExplordedNoedes);
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
