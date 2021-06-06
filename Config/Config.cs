using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkCalculator.Config
{
    public static class Config
    {
        static private List<string> _allFileType = new List<string>();
        static private List<string> _allowFileType = new List<string>();
        static internal List<string> AllFileType => _allFileType;
        static internal List<string> AllowFileType => _allowFileType;

        static internal string MatchingRules { get; set; }
        static internal string ConfigPath { get; set; }
        static Config()
        {
        }

        internal static void Load(configType type, string configPath)
        {
            ConfigPath = configPath;
            _allowFileType = type.AllowFileType;
            _allFileType = type.AllFileType;
            MatchingRules = type.MatchingRules;
        }
    }
}
