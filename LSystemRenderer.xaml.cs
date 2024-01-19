using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace L_System_Renderer
{
    /// <summary>
    /// Interaction logic for LSystemRenderer.xaml
    /// </summary>
    public partial class LSystemRenderer
    {
        public LSystem LSystem;
        public Preset CurrentPreset = null!;
        private string _instructions = null!;
        private int _currentIterations;

        // Prevents rendering when a preset hasn't been loaded
        private bool _presetLoaded; 

        public double BrushThickness = 0.3; // Default Thickness
        public Brush Brush = Brushes.Black; // Default Colour
        
        public LSystemRenderer()
        {
            InitializeComponent();
            LSystem = new LSystem();
        }

        public void Start()
        {
            LSystem.ReadPresets();
        }

        public void LoadPreset(string name)
        {
            _presetLoaded = false;
            // Preset is Cloned to prevent updating preset within the dictionary
            CurrentPreset = LSystem.Presets[name].Clone();
            _currentIterations = CurrentPreset.Iterations;
            _instructions =
                LSystem.GenerateInstructions(CurrentPreset.Axiom, _currentIterations, CurrentPreset.Rules);
            _presetLoaded = true;
            Redraw();
        }

        public void ReloadCurrentPreset()
        {
            _presetLoaded = false;
            _currentIterations = CurrentPreset.Iterations;
            _instructions =
                LSystem.GenerateInstructions(CurrentPreset.Axiom, _currentIterations, CurrentPreset.Rules);
            _presetLoaded = true;
            Redraw();
        }

        public void Redraw()
        {
            if (!_presetLoaded)
            {
                MessageBox.Show("Select a Preset first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            InvalidateVisual();
        }

        public void IncreaseIteration()
        {
            if (!_presetLoaded)
            {
                MessageBox.Show("Select a Preset first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_currentIterations == 10)
            {
                MessageBox.Show("Cannot go above 10 iterations, please use the parameters menu", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _currentIterations++;
            _instructions =
                LSystem.GenerateInstructions(CurrentPreset.Axiom, _currentIterations, CurrentPreset.Rules);
            Redraw();
        }

        public void DecreaseIteration()
        {
            if (!_presetLoaded)
            {
                MessageBox.Show("Select a Preset first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_currentIterations == 0)
            {
                MessageBox.Show("Cannot go below 0 iterations", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _currentIterations--;
            _instructions =
                LSystem.GenerateInstructions(CurrentPreset.Axiom, _currentIterations, CurrentPreset.Rules);
            Redraw();
        }

        public bool PresetLoaded()
        {
            return _presetLoaded;
        }

        public string CurrentIterations()
        {
            return _currentIterations.ToString();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (!_presetLoaded) return;
            var pen = new Pen
            {
                Brush = Brush,
                Thickness = BrushThickness,
                EndLineCap = PenLineCap.Round,
                StartLineCap = PenLineCap.Round
            };
            pen.Freeze();

            // Create a DrawingGroup
            DrawingGroup dGroup = new DrawingGroup();

            // Create empty stack to track drawing states
            var states = new Stack<LSystem.State>();

            LSystem.State state = new LSystem.State
            {
                Position = new Point(0, 0),
                Direction = new Point(0, -1),
                Angle = CurrentPreset.Angle,
                Size = CurrentPreset.Length
            };

            using (DrawingContext dc = dGroup.Open())
            {
                foreach (var c in _instructions)
                {
                    double angle;
                    switch (c)
                    {
                        case 'F': // Forward and Draw
                            var dx = state.Direction.X * state.Size;
                            var dy = state.Direction.Y * state.Size;
                            var newPos = new Point(state.Position.X + dx, state.Position.Y + dy);
                            dc.DrawLine(pen, state.Position, newPos);
                            state.Position = newPos;
                            break;
                        case 'f': // Forward but don't draw
                            var distanceX = state.Direction.X * state.Size;
                            var distanceY = state.Direction.Y * state.Size;
                            state.Position = new Point(state.Position.X + distanceX, state.Position.Y + distanceY);
                            break;
                        case '+': // Rotate right by angle
                            angle = state.Angle * Math.PI / 180.0;
                            angle *= -1;
                            state.Direction = new Point(state.Direction.X * Math.Cos(angle) - state.Direction.Y * Math.Sin(angle),
                                state.Direction.X * Math.Sin(angle) + state.Direction.Y * Math.Cos(angle));
                            break;
                        case '-': // Rotate left by angle
                            angle = state.Angle * Math.PI / 180.0;
                            state.Direction = new Point(state.Direction.X * Math.Cos(angle) - state.Direction.Y * Math.Sin(angle),
                                state.Direction.X * Math.Sin(angle) + state.Direction.Y * Math.Cos(angle));
                            break;
                        case '[': // Save current drawing state
                            states.Push(state.Clone());
                            break;
                        case ']': // Revert back to previous saved drawing state
                            state = states.Pop();
                            break;
                        case '|': // Face opposite direction
                            angle = 180 * Math.PI / 180.0;
                            state.Direction = new Point(state.Direction.X * Math.Cos(angle) - state.Direction.Y * Math.Sin(angle),
                                state.Direction.X * Math.Sin(angle) + state.Direction.Y * Math.Cos(angle));
                            break;
                        case '!': // Inverse current angle, if + turned left then it will turn right and vice versa
                            state.Angle *= -1;
                            break;
                        case ')': // Increase turning angle by angle growth
                            state.Angle *= 1.0 + CurrentPreset.AngleGrowth;
                            break;
                        case '(': // Decrease turning angle by angle growth
                            state.Angle *= 1.0 - CurrentPreset.AngleGrowth;
                            break;
                        case '>': // Increase draw length by length growth
                            state.Size *= 1.0 + CurrentPreset.LengthGrowth;
                            break;
                        case '<': // Decrease draw length by length growth
                            state.Size *= 1.0 - CurrentPreset.LengthGrowth;
                            break;
                    }
                }
            }

            // Display the drawing using an image control.
            Image theImage = new Image();
            DrawingImage dImageSource = new DrawingImage(dGroup);
            theImage.Source = dImageSource;
            theImage.Stretch = Stretch.Uniform;

            this.Content = theImage;

        }
    }
}
