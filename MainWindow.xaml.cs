using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace L_System_Renderer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // ReadPreset(1);


        }

        private List<Preset> presets;

        private async void ReadPreset(int presetId)
        {
            try
            {
                var rootPath = Directory.GetCurrentDirectory();

                var presetPath = Path.Combine(rootPath, "Presets", $"Preset{presetId}.txt");

                using var sr = new StreamReader(presetPath);
                var text = await sr.ReadToEndAsync();

                var lines = text.Split('\n', StringSplitOptions.TrimEntries);
                var preset = new Preset();
                foreach (var line in lines)
                {
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
                            preset.Rules.Add(Convert.ToChar(constant), constant);
                        }
                    }
                }
                
                // Trace.WriteLine(text.Split('\n')[0].Split(':')[1].Remove(0, 1));
            }
            catch (FileNotFoundException ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }
    }
}
