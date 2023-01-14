using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Win32;
using SFP_Planner.IO;
using System.IO;
using SFPCalculator;

namespace SFP_Planner.Data
{
    static class FileManager
    {
        public static string OpenedFilePath = "";
        static string DRExt = "sfpp";

        static void ThrowError(string Message) => 
            MessageBox.Show(Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        //Save File
        #region Save File
        public static void SaveAs(string SavePath = "")
        {
            if(string.IsNullOrWhiteSpace(SavePath))
            {
                SaveFileDialog sfd = new()
                {
                    Filter = "SFP Plan File|*.sfpp|Excel Export|*.xlsx|Script File|*.txt",
                    DefaultExt = "sfpp"
                };
                if (!(sfd.ShowDialog() ?? false)) return;
                OpenedFilePath = SavePath = sfd.FileName;
                DRExt = SavePath.Split('.').Last().Trim().ToLower();
            }

            switch (DRExt)
            {
                case "xlsx": SaveExcel(SavePath); break;
                case "sfpp": Savejson(SavePath); break;
                case "txt": Savetxt(SavePath); break;
                default: ThrowError("Invalid File Extention : " + DRExt); break;
            }
        }

        static void SaveExcel(string Path)
        {
            if (PlanData.FullPlan == null) { ThrowError("No Production Is Planed To Be Exported."); return; }
            var EXport = new ExcelIO(PlanData.FullPlan, Path);
            EXport.Export();
        }

        static void Savejson(string Path)
        {
            if (PlanData.FullPlan == null) { ThrowError("No Production Is Planed To Be Saved."); return; }
            string jsonString = JsonSerializer.Serialize(SFPScripts.ProcessToMicroProcess(PlanData.FullPlan).Result, 
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path, jsonString);
        }

        static void Savetxt(string Path)
        {
            if (PlanData.FullPlan == null) { ThrowError("No Production Is Planed To Be Saved."); return; }
            RPScriptIO.Export(SFPScripts.ProcessToRPScript(PlanData.FullPlan).Result, Path);
        }
        #endregion

        //Open File
        #region Open File
        public static void Open(MainWindow Form)
        {
            OpenFileDialog sfd = new()
            {
                Filter = "SFP Plan File|*.sfpp|Script File|*.txt",
                DefaultExt = "sfpp"
            };
            if (!(sfd.ShowDialog() ?? false)) return;
            OpenedFilePath = sfd.FileName;
            DRExt = OpenedFilePath.Split('.').Last().Trim().ToLower();

            switch (DRExt)
            {
                case "sfpp": Openjson(Form); break;
                case "txt": Opentxt(Form); break;
                default: ThrowError("Invalid File Extention : " + DRExt); break;
            }
        }

        static void Openjson(MainWindow Form)
        {
            var Plan = JsonSerializer.Deserialize<MacroProcess>(File.ReadAllText(OpenedFilePath));
            if (Plan == null) { ThrowError("No Production In Planed To Be Loaded."); return; }
            Form.ImportPlan(SFPScripts.MicroProcessToProcess(Plan).Result);
        }

        static void Opentxt(MainWindow Form)
        {
            RPScriptIO.Import(out string[] RPScript, OpenedFilePath);
            Form.ImportPlan(SFPScripts.RPScriptToProcess(RPScript).Result);
        }
        #endregion
    }
}
