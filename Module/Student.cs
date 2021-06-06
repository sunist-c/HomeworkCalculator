using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkCalculator.Module
{
    public class Student
    {
        public string Name { get; set; }
        public uint ID { get; set; }

        public Student()
        {
            Name = "";
            ID = 0;
        }
    }
}
