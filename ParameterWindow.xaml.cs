using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace L_System_Renderer
{
    /// <summary>
    /// Interaction logic for ParameterWindow.xaml
    /// </summary>
    public partial class ParameterWindow
    {
        private readonly LSystemRenderer? _lSystemRenderer;

        public ParameterWindow(LSystemRenderer? lSystemRenderer)
        {
            _lSystemRenderer = lSystemRenderer;
            InitializeComponent();
        }

        public void SetupWindow()
        {
            Debug.Assert(_lSystemRenderer != null, nameof(_lSystemRenderer) + " != null");
            var preset = _lSystemRenderer.CurrentPreset; // Get the current loaded preset

            // Title is a Label not a TextBox so preset name is assigned via Content not Text
            // Label.Content = <Content>
            // TextBox.Text = <Content>
            Title.Content = preset.Title;
            Axiom.Text = preset.Axiom;
            
            // Create Rules string 
            string rules = "";
            foreach (var rule in preset.Rules)
            {
                if (rule.Value.Length == 1)
                {
                    // Don't include any constants from the rules dictionary in the rules section
                    var isConstant = false;
                    foreach (var constant in preset.Constants)
                    {
                        if (rule.Value[0] == constant) isConstant = true;
                    }
                    if (isConstant) continue;

                }
                rules += $"{rule.Key}={rule.Value}\n";

            }
            rules = rules[..^1]; // Removed the last new line character
            Rules.Text = rules;
            Iterations.Text = _lSystemRenderer.CurrentIterations();
            Angle.Text = preset.Angle.ToString(CultureInfo.InvariantCulture);
            string constants = "";
            for (int i = 0; i < preset.Constants.Count; i++)
            {
                constants += preset.Constants[i].ToString();
                // Only add separator if not last constant
                if (i < preset.Constants.Count - 1) constants += ", ";
            }
            Constants.Text = constants;
            Length.Text = preset.Length.ToString(CultureInfo.InvariantCulture);
            AngleGrowth.Text = preset.AngleGrowth.ToString(CultureInfo.InvariantCulture);
            LengthGrowth.Text = preset.LengthGrowth.ToString(CultureInfo.InvariantCulture);
            MaxIterations.Content = preset.MaxIterations.ToString(CultureInfo.InvariantCulture);
            Show(); // Display the window was all text has been set
        }

        private void ApplyAxiom()
        {
            Debug.Assert(_lSystemRenderer != null, nameof(_lSystemRenderer) + " != null");
            _lSystemRenderer.CurrentPreset.Axiom = Axiom.Text;
        }

        private void ApplyIterations()
        {
            Debug.Assert(_lSystemRenderer != null, nameof(_lSystemRenderer) + " != null");
            var iterations = Convert.ToInt32(Iterations.Text);
            // Max iterations are determined by the preset file.
            // If the user wishes to increase this number they must edit the preset file and reload.
            if (iterations > _lSystemRenderer.CurrentPreset.MaxIterations)
            {
                MessageBox.Show("Cannot go above the presets max iterations. Increase the max iterations within the preset file (DO THIS AT YOUR OWN RISK!)", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Iterations.Text = _lSystemRenderer.CurrentPreset.Iterations.ToString();
            }
            else
            {
                _lSystemRenderer.CurrentPreset.Iterations = iterations;
            }
        }

        private void ApplyAngle()
        {
            Debug.Assert(_lSystemRenderer != null, nameof(_lSystemRenderer) + " != null");
            _lSystemRenderer.CurrentPreset.Angle = Convert.ToDouble(Angle.Text);
        }

        private void ApplyRulesConstants()
        {
            Debug.Assert(_lSystemRenderer != null, nameof(_lSystemRenderer) + " != null");
            var rules = Rules.Text.Split('\n');
            var rulesDict = new Dictionary<char, string>();
            foreach (var rule in rules)
            {
                var split = rule.Split('=', StringSplitOptions.TrimEntries);
                rulesDict.Add(split[0][0], split[1]);
            }

            var constants = Constants.Text.Split(",", StringSplitOptions.TrimEntries);

            var constantsChar = new List<char>();
            foreach (var constant in constants)
            {
                rulesDict.Add(constant[0], constant);
                constantsChar.Add(constant[0]);
            }
            _lSystemRenderer.CurrentPreset.Rules = rulesDict;
            _lSystemRenderer.CurrentPreset.Constants = constantsChar;
            
        }

        private void ApplyLength()
        {
            Debug.Assert(_lSystemRenderer != null, nameof(_lSystemRenderer) + " != null");
            _lSystemRenderer.CurrentPreset.Length = Convert.ToDouble(Length.Text);
            
        }

        private void ApplyAngleGrowth()
        {
            Debug.Assert(_lSystemRenderer != null, nameof(_lSystemRenderer) + " != null");
            _lSystemRenderer.CurrentPreset.AngleGrowth = Convert.ToDouble(AngleGrowth.Text);
        }

        private void ApplyLengthGrowth()
        {
            Debug.Assert(_lSystemRenderer != null, nameof(_lSystemRenderer) + " != null");
            _lSystemRenderer.CurrentPreset.LengthGrowth = Convert.ToDouble(LengthGrowth.Text);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Debug.Assert(_lSystemRenderer != null, nameof(_lSystemRenderer) + " != null");
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
