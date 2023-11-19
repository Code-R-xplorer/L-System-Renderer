using System;
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

        public Dictionary<string, Preset> Presets = new();

        public void ReadPresets()
        {
            Presets = new Dictionary<string, Preset>();
            Trace.WriteLine("Starting ReadPresets");
            try
            {
                var rootPath = Directory.GetCurrentDirectory();

                var presetPath = Path.Combine(rootPath, "Presets");

                var presetFiles = Directory.GetFiles(presetPath);

                if (presetFiles.Length == 0)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("No preset files found!\nPlease add the preset files to the Presets folder", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (messageBoxResult == MessageBoxResult.OK)
                    {
                        Application.Current.Shutdown();
                    }
                }


                foreach (var presetFile in presetFiles)
                {
                    Trace.WriteLine(presetFile);
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
                        if (line.Length == 0) continue;
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
                                preset.Rules.Add(split[0][0], split[1]);
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
                                preset.Constants.Add(constant[0]);
                                preset.Rules.Add(constant[0], constant);
                            }
                        }

                        if (contents[0] == "Length")
                        {
                            preset.Length = Convert.ToDouble(contents[1]);
                        }

                        if (contents[0] == "Length Growth")
                        {
                            preset.LengthGrowth = Convert.ToDouble(contents[1]);
                        }

                        if (contents[0] == "Angle Growth")
                        {
                            preset.AngleGrowth = Convert.ToDouble(contents[1]);
                        }
                    }

                    Presets.Add(preset.Title, preset);
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show($"Could not find file\n{ex.Message}", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    Application.Current.Shutdown();
                }
                Trace.WriteLine(ex.Message);
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show($"Could not find directory\n{ex.Message}", "Directory Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    Application.Current.Shutdown();
                }
                Trace.WriteLine(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show($"Could not access directory or file\n{ex.Message}", "Access Violation", MessageBoxButton.OK, MessageBoxImage.Error);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    Application.Current.Shutdown();
                }
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
