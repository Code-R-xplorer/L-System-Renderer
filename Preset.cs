using System.Collections.Generic;

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
        public int MaxIterations;

        public Preset(string title, string axiom, Dictionary<char, string> rules, int iterations, double angle, List<char> constants, double length, double angleGrowth, double lengthGrowth, int maxIterations)
        {
            Title = title;
            Axiom = axiom;
            Rules = rules;
            Iterations = iterations;
            Angle = angle;
            Constants = constants;
            Length = length;
            AngleGrowth = angleGrowth;
            LengthGrowth = lengthGrowth;
            MaxIterations = maxIterations;
        }

        public Preset Clone() { return (Preset)MemberwiseClone(); }
        
    }
}
