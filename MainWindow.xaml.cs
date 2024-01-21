using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace L_System_Renderer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static LSystemRenderer? _lSystemRenderer;

        public MainWindow()
        {
            InitializeComponent();

            // Add items in navigation bar

            // Edit Menu
            EditMenu.Items.Add(new MenuItem
            {
                Header = "Redraw", // Name within the menu item
                IsEnabled = true, // if the user can interact with it
                Command = Redraw // The command to call
            });

            EditMenu.Items.Add(new MenuItem
            {
                Header = "Brush Settings",
                IsEnabled = true,
                Command = OpenBrushSettings
            });

            // L-System Menu

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
                InputGestureText = "Ctrl + ]" // Displays the shortcut keys
            });

            LSystemMenu.Items.Add(new MenuItem
            {
                Header = "Previous Iteration",
                IsEnabled = true,
                Command = PreviousIteration,
                InputGestureText = "Ctrl + ["
            });

            // Implement the shortcut keys for menu item

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

            // Create the L-System Renderer Window
            _lSystemRenderer = new LSystemRenderer();
            // Assign event for adding menu items once the L-System has loaded all presets from files
            _lSystemRenderer.LSystem.OnPresetsLoaded += PresetsLoaded;
            
            // Make the main window display the L-System renderer
            RendererFrame.Navigate(_lSystemRenderer);
            // Load the preset files
            _lSystemRenderer.Start();

        }

        private void PresetsLoaded()
        {
            var index = 0;
            foreach (var preset in _lSystemRenderer!.LSystem.Presets)
            {
                index++;
                // Only Assign shortcuts to the first 8 presets
                if (index <= 8)
                {
                    var item = new MenuItem
                    {
                        Header = preset.Value.Title,
                        IsEnabled = true,
                        Command = LoadPreset,
                        CommandParameter = preset.Key,
                        InputGestureText = $"Ctrl + {index}"
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

                if(index > 8) { continue;}
                // Assign key binds to the first 8 presets
                // Keyboard values for numbers on top row not numpad
                Key key = index switch
                {
                    1 => Key.D1,
                    2 => Key.D2,
                    3 => Key.D3,
                    4 => Key.D4,
                    5 => Key.D5,
                    6 => Key.D6,
                    7 => Key.D7,
                    8 => Key.D8,
                    _ => throw new ArgumentOutOfRangeException()
                };
                // Create the keybinding
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

        // MenuItem and KeyBind commands
        public ICommand LoadPreset { get; } = new SimpleDelegateCommand((x) =>
        {
            Debug.Assert(_lSystemRenderer != null, nameof(_lSystemRenderer) + " != null");
            string name = (string)x;
            _lSystemRenderer.LoadPreset(name);
        });

        public ICommand Redraw { get; } = new SimpleDelegateCommand(_ =>
        {
            Debug.Assert(_lSystemRenderer != null, nameof(_lSystemRenderer) + " != null");
            _lSystemRenderer.Redraw();
        });

        public ICommand NextIteration { get; } = new SimpleDelegateCommand(_ =>
        {
            Debug.Assert(_lSystemRenderer != null, nameof(_lSystemRenderer) + " != null");
            _lSystemRenderer.IncreaseIteration();
        });

        public ICommand PreviousIteration { get; } = new SimpleDelegateCommand(_ =>
        {
            Debug.Assert(_lSystemRenderer != null, nameof(_lSystemRenderer) + " != null");
            _lSystemRenderer.DecreaseIteration();
        });

        public ICommand OpenParameters { get; } = new SimpleDelegateCommand(_ =>
        {
            Debug.Assert(_lSystemRenderer != null, nameof(_lSystemRenderer) + " != null");
            // Only open parameter window when a preset is in use
            if (_lSystemRenderer.PresetLoaded())
            {
                var parameterWindow = new ParameterWindow(_lSystemRenderer);
                parameterWindow.SetupWindow();
            }
            // Display error message if a preset hasn't been selected
            else
            {
                MessageBox.Show("Select a Preset first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });

        public ICommand OpenBrushSettings { get; } = new SimpleDelegateCommand(_ =>
        {
            Debug.Assert(_lSystemRenderer != null, nameof(_lSystemRenderer) + " != null");
            if (_lSystemRenderer.PresetLoaded())
            {
                var brushSettings = new BrushSettings(_lSystemRenderer);
                brushSettings.Show();
            }
            else
            {
                MessageBox.Show("Select a Preset first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });
    }

    // Class to be able to assign functions to MenuItem and KeyBind commands
    public class SimpleDelegateCommand : ICommand
    {
        private readonly Action<object> _executeDelegate;

        public SimpleDelegateCommand(Action<object> executeDelegate)

        {
            _executeDelegate = executeDelegate;

        }

        public void Execute(object? parameter)
        {
            _executeDelegate(parameter!);
        }

        // Always execute commands
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public event EventHandler? CanExecuteChanged;

    }
}