﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace L_System_Renderer
{
    public class LSystem
    {
        public event Action onPresetsLoaded;

        public void PresetsLoaded()
        {
            onPresetsLoaded?.Invoke();
        }

        public class State
        {
            public double Size;
            public double Angle;
            public Point Position;
            public Point Direction;

            public State Clone() { return (State)this.MemberwiseClone(); }
        }

        public string ReWrite(string start, Dictionary<char, string> rules)
        {
            var outString = "";

            foreach (var c in start)
            {
                var s = rules[c];
                outString += s;
            }

            return outString;
        }

        public List<Preset> Presets = new List<Preset>();

        public void ReadPresets()
        {
            Presets = new List<Preset>();
            Trace.WriteLine("Starting ReadPresets");
            try
            {
                var rootPath = Directory.GetCurrentDirectory();

                var presetPath = Path.Combine(rootPath, "Presets");

                var presetFiles = Directory.GetFiles(presetPath);


                foreach (var presetFile in presetFiles)
                {
                    using var sr = new StreamReader(presetFile);
                    var text = sr.ReadToEnd();

                    var lines = text.Split('\n', StringSplitOptions.TrimEntries);
                    var preset = new Preset
                    {
                        Rules = new Dictionary<char, string>(),
                        Constants = new List<char>()
                    };
                    foreach (var line in lines)
                    {
                        if(line.Length == 0) continue;
                        if (line[0] == '#') continue;
                        var contents = line.Split(':', StringSplitOptions.TrimEntries);
                        if (contents[0] == "Title")
                        {
                            preset.Title = contents[1];
                        }
                    
                        if (contents[0] == "Axiom")
                        {
                            preset.Axiom = contents[1];
                        }
                    
                        if (contents[0] == "Rules")
                        {
                            var rules = contents[1].Split(',');
                            foreach (var rule in rules)
                            {
                                var split = rule.Split('=');
                                preset.Rules.Add(Convert.ToChar(split[0]), split[1]);
                            }
                        }
                    
                        if (contents[0] == "Iterations")
                        {
                            preset.Iterations = Convert.ToInt32(contents[1]);
                        }
                    
                        if (contents[0] == "Angle")
                        {
                            preset.Angle = Convert.ToDouble(contents[1]);
                        }
                    
                        if (contents[0] == "Constants")
                        {
                            var constants = contents[1].Split(",");
                            foreach (var constant in constants)
                            {
                                preset.Constants.Add(Convert.ToChar(constant));
                                preset.Rules.Add(Convert.ToChar(constant), constant);
                            }
                        }
                    }
                    Presets.Add(preset);
                }
            }
            catch (FileNotFoundException ex)
            {
                Trace.WriteLine(ex.Message);
            }
            PresetsLoaded();
        }

        public string GenerateInstructions(string axiom, int iterations, Dictionary<char, string> rules)
        {
            var s = axiom;
            for (int i = 0; i < iterations; i++)
            {
                s = ReWrite(s, rules);
            }

            return s;
        }
    }
}
