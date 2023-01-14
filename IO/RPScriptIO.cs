using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SFP_Planner.IO
{
    public class RPScriptIO
    {
        public static Task<bool> Export(string[] RPScript, string OutputFile)
        {
            if (RPScript.Length <= 0)
                throw new ArgumentException("RPScript Can Not Be Empty");
            var sOutputFile = OutputFile;
            var Script = RPScript;
            var Writer = new StreamWriter(sOutputFile);
            try
            {
                Writer.WriteLine(string.Join("\r\n", Script));
            }
            catch (Exception e) { Console.WriteLine(e); return Task.FromResult(false); }
            finally {Writer.Close(); }

            MessageBox.Show("Script Was Exported.", "Script Export", MessageBoxButton.OK, MessageBoxImage.Information);
            return Task.FromResult(true);
        }

        public static Task<bool> Import(out string[] RPScript, string OutputFile)
        {
            RPScript = new string[0];
            var Script = new List<string>();
            var sOutputFile = OutputFile;
            if (!File.Exists(sOutputFile))
            {
                Console.WriteLine("File Does Not Exist.");
                return Task.FromResult(true);
            }
            var Reader = new StreamReader(sOutputFile);
            try
            {
                while (!Reader.EndOfStream)
                    Script.Add(Reader.ReadLine() ?? "");
            }
            catch (Exception e) { Console.WriteLine(e); return Task.FromResult(false); }
            finally { Reader.Close(); }

            RPScript = Script.ToArray();
            return Task.FromResult(true);
        }
    }
}
