using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ClosedXML.Excel;
using SFPCalculator;

namespace SFP_Planner.IO
{
    public class ExcelIO
    {
        const int MAXROW = 7, MAXCOL = 5;
        int iColSet = 0, iAltName = -1;
        const string sNumFormat = "0.00";

        string sOutputFile { get; set; }
        string sPlanName { get; set; } = "";
        Process ProcessLine { get; set; }
        XLWorkbook xlWorkbook { get; set; }
        IXLWorksheet xlWorksheet { get; set; }

        public ExcelIO(Process Process, string OutputFile)
        {
            sPlanName += Process.Recipe.ToString().Replace(": ", "").Trim();
            if (sPlanName.Length + 8 >= 30) sPlanName = sPlanName[..22].Trim();
            ProcessLine = Process;
            sOutputFile = OutputFile;
            xlWorkbook = GenWorkbook();
        }

        public Task<bool> Export()
        {
            string sException = "";
            bool bExist;
            do
            {
                iAltName++;
                try
                {
                    if (iAltName <= 0)
                        xlWorksheet = xlWorkbook.Worksheets.Add(sPlanName + " Plan");
                    else
                        xlWorksheet = xlWorkbook.Worksheets.Add($"{sPlanName} Plan ({iAltName})");
                    bExist = false;
                }
                catch (Exception ex) { bExist = true; sException = ex.Message; }
                if (iAltName > 9)
                    throw new Exception("Max Duplication Number Achieved Or Invalid Plan Name : "
                        + sPlanName + $"\r\n{sException}");
            } while (bExist);
            try
            {
                MainEX(ProcessLine);
            }
            catch (Exception e) { Console.WriteLine(e); return Task.FromResult(false); }
            xlWorksheet.Columns().AdjustToContents();

            iAltName = -1;
            //sPlanName = sPlanName.Length + 8 >= 30 ? sPlanName[..22] : sPlanName;
            do
            {
                iAltName++;
                try
                {
                    if (iAltName <= 0)
                        xlWorksheet = xlWorkbook.Worksheets.Add(sPlanName + " Summary");
                    else
                        xlWorksheet = xlWorkbook.Worksheets.Add($"{sPlanName} Summary ({iAltName})");
                    bExist = false;
                }
                catch (Exception ex) { bExist = true; sException = ex.Message; }
                if (iAltName > 10)
                    throw new Exception("Max Duplication Number Achieved Or Invalid Summary Name : "
                        + sPlanName + $"\r\n{sException}");
            } while (bExist);
            try
            {
                SumEx(ProcessLine);
            }
            catch (Exception e) { Console.WriteLine(e); return Task.FromResult(false); }
            xlWorksheet.Columns().AdjustToContents();

            xlWorksheet.Columns().AdjustToContents();
            xlWorkbook.CalculateMode = XLCalculateMode.Auto;
            xlWorkbook.SaveAs(sOutputFile);

            MessageBox.Show("Export Was Sucessfull.", "Excel Export", MessageBoxButton.OK, MessageBoxImage.Information);
            return Task.FromResult(true);
        }

        void MainEX(Process lstProcess, int Block = 0, string? InputCell = "")
        {
            var PrimeryOut = lstProcess.Product.Item;
            var Children = lstProcess.Children;
            var Inputs = lstProcess.Inputs;
            var Outputs = lstProcess.Outputs;
            var InPerMin = Inputs.Select(el => el.PerMin).ToList();
            var OutPerMin = Outputs.Select(el => el.PerMin).ToList();
            var OutDefPerMin = PrimeryOut.ID == 0 ? lstProcess.Building.PowerUsed
                : lstProcess.Recipe.GetOutput().First().Value;
            var InputCells = new List<string?>();

            if (Children.ToList().Count <= 0)
                return;

            int iRow = Block * MAXROW + 1;
            int iCol = iColSet * MAXCOL + 1;

            var A1 = xlWorksheet.Cell(iRow, iCol);
            var A4 = xlWorksheet.Cell(iRow + 3, iCol);
            var C1 = xlWorksheet.Cell(iRow, iCol + 2);
            var D1 = xlWorksheet.Cell(iRow, iCol + 3);

            if (string.IsNullOrWhiteSpace(InputCell))
                xlWorksheet.Cell(iRow, iCol).Value = OutPerMin.First().ToString();
            else
                xlWorksheet.Cell(iRow, iCol).FormulaA1 = "=" + InputCell;
            xlWorksheet.Cell(iRow, iCol + 1).Value = PrimeryOut.Name;
            xlWorksheet.Cell(iRow, iCol + 2).Value = OutDefPerMin;
            xlWorksheet.Cell(iRow, iCol + 3).FormulaA1 = $"={A1}/{C1}";
            xlWorksheet.Cell(iRow, iCol).Style.Fill.BackgroundColor = XLColor.FromHtml("#A9D08E");
            xlWorksheet.Cell(iRow, iCol).Style.NumberFormat.Format = sNumFormat;

            xlWorksheet.Cell(iRow + 1, iCol).Value = "Clock Speed";
            xlWorksheet.Cell(iRow + 2, iCol).Value = 0;
            xlWorksheet.Cell(iRow + 3, iCol).FormulaA1 = $"=(({xlWorksheet.Cell(iRow + 2, iCol)}*50)+100)/100";
            xlWorksheet.Cell(iRow + 4, iCol).Value = lstProcess.Building.Name;
            xlWorksheet.Cell(iRow + 5, iCol).FormulaA1 = $"={A1}/{C1}/{A4}";
            xlWorksheet.Cell(iRow + 2, iCol).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFD966");
            xlWorksheet.Cell(iRow + 5, iCol).Style.Fill.BackgroundColor = XLColor.FromHtml("#C9C9C9");
            xlWorksheet.Cell(iRow + 5, iCol).Style.NumberFormat.Format = sNumFormat;

            xlWorksheet.Cell(iRow + 1, iCol + 1).Value = Outputs.ToList().Count >= 2 ? PrimeryOut.Name : "None";
            xlWorksheet.Cell(iRow + 1, iCol + 2).Value = Outputs.ToList().Count >= 2 ? OutDefPerMin : 0;
            xlWorksheet.Cell(iRow + 1, iCol + 3).FormulaA1 = $"={xlWorksheet.Cell(iRow + 1, iCol + 2)}*{D1}";
            xlWorksheet.Cell(iRow + 1, iCol + 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8CBAD");

            var J = -1;
            foreach (var In in Inputs)
            {
                J++;
                xlWorksheet.Cell(iRow + 2 + J, iCol + 1).Value = In.Item.Name;
                var Def = SFPCalculator.Recipes.Data.GetOutput(In.Item.ID).First();
                xlWorksheet.Cell(iRow + 2 + J, iCol + 2).Value = lstProcess.Recipe.GetInput().Values.ToList()[J];
                xlWorksheet.Cell(iRow + 2 + J, iCol + 3).FormulaA1 = $"={xlWorksheet.Cell(iRow + 2 + J, iCol + 2)}*{D1}";
                xlWorksheet.Cell(iRow + 2 + J, iCol + 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#9BC2E6");
                InputCells.Add(xlWorksheet.Cell(iRow + 2 + J, iCol + 3).ToString());
            }

            xlWorksheet.Range(iRow, iCol, iRow + MAXROW - 2, iCol + MAXCOL - 2).Style.Border.InsideBorder =
                XLBorderStyleValues.Thin;
            xlWorksheet.Range(iRow, iCol, iRow + MAXROW - 2, iCol + MAXCOL - 2).Style.Border.OutsideBorder =
                XLBorderStyleValues.Thin;
            xlWorksheet.Range(iRow, iCol + 2, iRow + MAXROW - 2, iCol + MAXCOL - 2).Style.NumberFormat.Format = sNumFormat;

            J = -1;
            if (lstProcess.Production)
            foreach (var Child in Children)
            {
                J++;
                MainEX(Child, Block + 1, InputCells[J]);
                iColSet++;
                if (Child != Children.First() && Child.Children.ToList().Count <= 0)
                    iColSet--;
            }
            iColSet--;
        }

        void SumEx(Process lstProcess)
        {
            var lstSum = new List<EX>();

            Sum(lstProcess);
            EX();
            xlWorksheet.Columns().AdjustToContents();

            void Sum(Process Process)
            {
                foreach (var OUT in Process.Outputs)
                {
                    if (lstSum.Select(x => x.Name).Contains(OUT.Item.Name))
                        lstSum.First(x => x.Name == OUT.Item.Name).OUT += OUT.PerMin;
                    else
                        lstSum.Add(new EX(OUT.Item.Name, 0, OUT.PerMin,
                            Process.Product != OUT));
                }

                int I = -1;
                foreach (var IN in Process.Inputs)
                {
                    I++;
                    if (lstSum.Select(x => x.Name).Contains(IN.Item.Name))
                        lstSum.First(x => x.Name == IN.Item.Name).IN += IN.PerMin;
                    else
                        lstSum.Add(new EX(IN.Item.Name, IN.PerMin, 0,
                            Process.Children.ToList()[I].Children.ToList().Count <= 0));
                    Sum(Process.Children.ToList()[I]);
                }
            }

            void EX()
            {
                int Index = 1;

                xlWorksheet.Cell(Index, 1).Value = "Resource";
                xlWorksheet.Cell(Index, 2).Value = "Input /Min";
                xlWorksheet.Cell(Index, 3).Value = "Output /Min";
                xlWorksheet.Cell(Index, 4).Value = "Excess /Min";

                foreach (var Item in lstSum)
                {
                    Index++;
                    var B2 = xlWorksheet.Cell(Index, 2);
                    var C2 = xlWorksheet.Cell(Index, 3);

                    xlWorksheet.Cell(Index, 1).Value = Item.Name;
                    xlWorksheet.Cell(Index, 2).Value = Item.IN;
                    xlWorksheet.Cell(Index, 3).Value = Item.OUT;
                    xlWorksheet.Cell(Index, 4).FormulaA1 = $"={C2}-{B2}";
                }

                xlWorksheet.Range(1, 1, 1, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#9BC2E6");
                xlWorksheet.Range(2, 1, Index, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#A9D08E");
                xlWorksheet.Range(1, 1, 1, 4).Style.Font.Bold = true;
                xlWorksheet.Range(1, 1, Index, 1).Style.Font.Bold = true;
                xlWorksheet.Range(1, 1, Index, 4).Style.Border.InsideBorder =
                    XLBorderStyleValues.Thin;
                xlWorksheet.Range(1, 1, Index, 4).Style.Border.OutsideBorder =
                    XLBorderStyleValues.Thin;
                xlWorksheet.Range(2, 2, Index, 4).Style.NumberFormat.Format = sNumFormat;
            }
        }

        XLWorkbook GenWorkbook()
        {
            //Var
            bool bOpen;
            //Code
            if (!File.Exists(sOutputFile))
                return new XLWorkbook();
            do
            {
                try
                {
                    FileStream tTest = File.OpenWrite(sOutputFile);
                    tTest.Close();
                    bOpen = false;
                }
                catch
                {
                    Console.WriteLine("Export File Is Open. Please Close And Try Again.");
                    Console.Write("Retry (Y | N) : ");
                    var sLine = Console.ReadLine();
                    if (sLine == null || sLine.Last().ToString().ToUpper() == "N")
                        throw new Exception("Export Canceled.");
                    bOpen = true;
                }
            } while (bOpen);

            return new XLWorkbook(sOutputFile);
        }
    }
}

internal class EX
{
    public string Name { get; set; }
    public double IN { get; set; }
    public double OUT { get; set; }
    public bool Grounded { get; set; }

    public EX (string ItemName, double Input, double Output, bool End)
    {
        Name = ItemName;
        IN = Input;
        OUT = Output;
        Grounded = End;
    }
}