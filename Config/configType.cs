using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkCalculator.Config
{
    public class configType
    {
        private List<string> _allFileType = new List<string>();
        private List<string> _allowFileType = new List<string>();
        public List<string> AllFileType => _allFileType;
        public List<string> AllowFileType => _allowFileType;

        public string MatchingRules { get; set; }
        public string ConfigPath { get; set; }

        public string DefaultFileName { get; set; }

        public bool Brush { get; set; }
        public configType()
        {
            
        }
    }
}
