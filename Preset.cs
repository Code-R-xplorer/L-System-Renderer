using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_System_Renderer
{
    public class Preset
    {
        public string Title;
        public string Axiom;
        public Dictionary<char, string> Rules;
        public int Iterations;
        public double Angle;
        public List<char> Constants;
        public double Length;
        public double AngleGrowth;
        public double LengthGrowth;
        public Preset Clone() { return (Preset)MemberwiseClone(); }
        
    }
}
