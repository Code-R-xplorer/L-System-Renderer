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
using System.Reflection;
using System.Data;

namespace L_System_Renderer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static LSystemRenderer lSystemRenderer;

        public MainWindow()
        {
            InitializeComponent();

            EditMenu.Items.Add(new MenuItem
            {
                Header = "Redraw",
                IsEnabled = true,
                Command = Redraw
            });

            EditMenu.Items.Add(new MenuItem
            {
                Header = "Brush Settings",
                IsEnabled = true,
                Command = OpenBrushSettings
            });



            LSystemMenu.Items.Add(new MenuItem
            {
                Header = "Change Parameters",
                Command = OpenParameters
            });

            LSystemMenu.Items.Add(new MenuItem
            {
                Header = "Next Iteration",
                IsEnabled = true,
                Command = NextIteration,
                InputGestureText = "Ctrl + ]"
            });

            LSystemMenu.Items.Add(new MenuItem
            {
                Header = "Previous Iteration",
                IsEnabled = true,
                Command = PreviousIteration,
                InputGestureText = "Ctrl + ["
            });

            InputBindings.Add(new KeyBinding
            {
                Key = Key.OemCloseBrackets,
                Modifiers = ModifierKeys.Control,
                Command = NextIteration
            });

            InputBindings.Add(new KeyBinding
            {
                Key = Key.OemOpenBrackets,
                Modifiers = ModifierKeys.Control,
                Command = PreviousIteration
            });

            lSystemRenderer = new LSystemRenderer();
            lSystemRenderer.LSystem.onPresetsLoaded += PresetsLoaded;
            // lSystemRenderer.InitializeComponent();

            //Frame mainFrame = (Frame)this.FindName("RendererFrame") ?? throw new InvalidOperationException();
            RendererFrame.Navigate(lSystemRenderer);
            lSystemRenderer.Start();

        }

        private void PresetsLoaded()
        {
            var index = -1;
            foreach (var preset in lSystemRenderer.LSystem.Presets)
            {
                index++;
                // var index = lSystemRenderer.LSystem.Presets.IndexOf(preset);
                if (index < 8)
                {
                    var item = new MenuItem
                    {
                        Header = preset.Value.Title,
                        IsEnabled = true,
                        Command = LoadPreset,
                        CommandParameter = preset.Key,
                        InputGestureText = $"Ctrl + {index + 1}"
                    };
                    PresetsMenu.Items.Add(item);
                    
                }
                else
                {
                    var item = new MenuItem
                    {
                        Header = preset.Value.Title,
                        IsEnabled = true,
                        Command = LoadPreset,
                        CommandParameter = preset.Key
                    };
                    PresetsMenu.Items.Add(item);
                }
                if(index > 7) { continue;}
                Key key = index switch
                {
                    0 => Key.D1,
                    1 => Key.D2,
                    2 => Key.D3,
                    3 => Key.D4,
                    4 => Key.D5,
                    5 => Key.D6,
                    6 => Key.D7,
                    7 => Key.D8
                };
                var presetKeyBind = new KeyBinding
                {
                    Key = key,
                    Modifiers = ModifierKeys.Control,
                    Command = LoadPreset,
                    CommandParameter = preset.Key
                };
                InputBindings.Add(presetKeyBind);
            }
        }

        public ICommand LoadPreset { get; } = new SimpleDelegateCommand((x) =>
        {
            string name = (string)x;
            lSystemRenderer.LoadPreset(name);
        });

        public ICommand Redraw { get; } = new SimpleDelegateCommand((x) =>
        {
            lSystemRenderer.Redraw();
        });

        public ICommand NextIteration { get; } = new SimpleDelegateCommand((x) =>
        {
            lSystemRenderer.IncreaseIteration();
        });

        public ICommand PreviousIteration { get; } = new SimpleDelegateCommand((x) =>
        {
            lSystemRenderer.DecreaseIteration();
        });

        public ICommand OpenParameters { get; } = new SimpleDelegateCommand((x) =>
        {
            if (lSystemRenderer.PresetLoaded())
            {
                var preset = lSystemRenderer.CurrentPreset;
                var parameterWindow = new ParameterWindow(preset.Title, lSystemRenderer);
                parameterWindow.Title.Content = preset.Title;
                parameterWindow.Axiom.Text = preset.Axiom;

                string rules = "";
                foreach (var rule in preset.Rules)
                {
                    Trace.WriteLine(rule.Value);
                    if (rule.Value.Length == 1)
                    {
                        var isConstant = false;
                        foreach (var constant in preset.Constants)
                        {
                            if (rule.Value[0] == constant) isConstant = true;
                        }
                        if(isConstant) continue;
                        
                    }
                    rules += $"{rule.Key}={rule.Value}\n";
                    
                }
                rules = rules.Substring(0, rules.Length - 1);
                parameterWindow.Rules.Text = rules;
                parameterWindow.Iterations.Text = lSystemRenderer.CurrentIterations();
                parameterWindow.Angle.Text = preset.Angle.ToString();
                string constants = "";
                for (int i = 0; i < preset.Constants.Count; i++)
                {
                    constants += preset.Constants[i].ToString();
                    if (i < preset.Constants.Count - 1) constants += ", ";
                }
                parameterWindow.Constants.Text = constants;
                parameterWindow.Length.Text = preset.Length.ToString();
                parameterWindow.AngleGrowth.Text = preset.AngleGrowth.ToString();
                parameterWindow.LengthGrowth.Text = preset.LengthGrowth.ToString();
                parameterWindow.Show();
            }
            else
            {
                MessageBox.Show("Select a Preset first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });

        public ICommand OpenBrushSettings { get; } = new SimpleDelegateCommand((x) =>
        {
            if (lSystemRenderer.PresetLoaded())
            {
                var brushSettings = new BrushSettings(lSystemRenderer);
                brushSettings.Show();
            }
            else
            {
                MessageBox.Show("Select a Preset first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });
    }

    public class SimpleDelegateCommand : ICommand
    {
        Action<object> _executeDelegate;

        public SimpleDelegateCommand(Action<object> executeDelegate)

        {
            _executeDelegate = executeDelegate;

        }

        public void Execute(object parameter)

        {

            _executeDelegate(parameter);

        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

    }
}