using System;
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
using System.Windows.Shapes;

namespace L_System_Renderer
{
    /// <summary>
    /// Interaction logic for ParameterWindow.xaml
    /// </summary>
    public partial class ParameterWindow : Window
    {
        private string _presetName;
        private LSystemRenderer _lSystemRenderer;

        public ParameterWindow(string presetName, LSystemRenderer lSystemRenderer)
        {
            _presetName = presetName;
            _lSystemRenderer = lSystemRenderer;
            InitializeComponent();
        }



        private void ApplyAxiom()
        {
            _lSystemRenderer.CurrentPreset.Axiom = Axiom.Text;
        }

        private void ApplyIterations()
        {
            var iterations = Convert.ToInt32(Iterations.Text);
            if (iterations > 10)
            {
                MessageBoxResult messageBoxResult =
                    MessageBox.Show("Are you sure you want to go above 10 iterations?", "Warning", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    _lSystemRenderer.CurrentPreset.Iterations = iterations;
                }
                else
                {
                    Iterations.Text = _lSystemRenderer.CurrentPreset.Iterations.ToString();
                }
            }
            else
            {
                _lSystemRenderer.CurrentPreset.Iterations = iterations;
            }
        }

        private void ApplyAngle()
        {
            _lSystemRenderer.CurrentPreset.Angle = Convert.ToDouble(Angle.Text);
        }

        private void ApplyRulesConstants()
        {
            var rules = Rules.Text.Split('\n');
            var rulesDict = new Dictionary<char, string>();
            foreach (var rule in rules)
            {
                var split = rule.Split('=', StringSplitOptions.TrimEntries);
                rulesDict.Add(split[0][0], split[1]);
                Trace.WriteLine(split[0][0]);
            }

            var constants = Constants.Text.Split(",", StringSplitOptions.TrimEntries);

            var constantsChar = new List<char>();
            foreach (var constant in constants)
            {
                rulesDict.Add(constant[0], constant);
                constantsChar.Add(constant[0]);
                Trace.Write(constant[0]);
            }
            _lSystemRenderer.CurrentPreset.Rules = rulesDict;
            _lSystemRenderer.CurrentPreset.Constants = constantsChar;
            
        }

        private void ApplyLength()
        {
            _lSystemRenderer.CurrentPreset.Length = Convert.ToDouble(Length.Text);
            
        }

        private void ApplyAngleGrowth()
        {
            _lSystemRenderer.CurrentPreset.AngleGrowth = Convert.ToDouble(AngleGrowth.Text);
        }

        private void ApplyLengthGrowth()
        {
            _lSystemRenderer.CurrentPreset.LengthGrowth = Convert.ToDouble(LengthGrowth.Text);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ApplyAxiom();
            ApplyIterations();
            ApplyAngle();
            ApplyRulesConstants();
            ApplyLength();
            ApplyLengthGrowth();
            ApplyAngleGrowth();
            _lSystemRenderer.ReloadCurrentPreset();
        }
    }
}
