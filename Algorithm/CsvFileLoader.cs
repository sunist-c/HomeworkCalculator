using HomeworkCalculator.ViewModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkCalculator.Algorithm
{
    internal static class CsvFileLoader
    {
        internal static List<FormsView> Load(string path)
        {
            List<FormsView> views = new List<FormsView>();

            FileStream fs = new FileStream(path, System.IO.FileMode.Open);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);
            string buffer = "";
            while (!((buffer = sr.ReadLine()) is null))
            {
                string[] bufferArr = buffer.Split(',');
                views.Add(new FormsView() { Name = bufferArr[1], StudentNumber = bufferArr[0] });
            }
            fs.Close();
            sr.Close();
            return views;
        }
    }
}
