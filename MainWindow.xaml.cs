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

            lSystemRenderer = new LSystemRenderer();
            lSystemRenderer.LSystem.onPresetsLoaded += PresetsLoaded;
            // lSystemRenderer.InitializeComponent();

            //Frame mainFrame = (Frame)this.FindName("RendererFrame") ?? throw new InvalidOperationException();
            RendererFrame.Navigate(lSystemRenderer);
            lSystemRenderer.Start();

        }

        private void PresetsLoaded()
        {
            Trace.WriteLine("Hello");
            foreach (var preset in lSystemRenderer.LSystem.Presets)
            {
                var index = lSystemRenderer.LSystem.Presets.IndexOf(preset);
                MenuItem item = new MenuItem
                {
                    Header = preset.Title,
                    IsEnabled = true,
                    Command = LoadPreset,
                    CommandParameter = index,
                    InputGestureText = $"Ctrl + {index + 1}"
                };
                PresetsMenu.Items.Add(item);
                Key key = index switch
                {
                    0 => Key.D1,
                    1 => Key.D2,
                    2 => Key.D3,
                    3 => Key.D4,
                    4 => Key.D5,
                    5 => Key.D6,
                    _ => Key.D0
                };
                var presetKeyBind = new KeyBinding
                {
                    Key = key,
                    Modifiers = ModifierKeys.Control,
                    Command = LoadPreset,
                    CommandParameter = lSystemRenderer.LSystem.Presets.IndexOf(preset)
                };
                InputBindings.Add(presetKeyBind);
            }
        }

        public ICommand LoadPreset { get; } = new SimpleDelegateCommand((x) =>
        {
            int index = (int)x;
            Trace.WriteLine(index);
            lSystemRenderer.LoadPreset(index);
        });

        public ICommand Redraw { get; } = new SimpleDelegateCommand((x) =>
        {
            lSystemRenderer.Redraw();
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