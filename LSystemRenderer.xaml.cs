using System;
using System.Collections.Generic;
using System.Data;
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

namespace L_System_Renderer
{
    /// <summary>
    /// Interaction logic for LSystemRenderer.xaml
    /// </summary>
    public partial class LSystemRenderer : Page
    {
        public LSystem LSystem;
        private Preset _currentPreset;
        private string _instructions;

        private bool _presetLoaded;
        
        public LSystemRenderer()
        {
            InitializeComponent();
            LSystem = new LSystem();
        }

        public void Start()
        {
            LSystem.ReadPresets();
        }

        public void LoadPreset(int index)
        {
            _presetLoaded = false;
            _currentPreset = LSystem.Presets[index];
            _instructions =
                LSystem.GenerateInstructions(_currentPreset.Axiom, _currentPreset.Iterations, _currentPreset.Rules);
            _presetLoaded = true;
            InvalidateVisual();
        }

        public void Redraw()
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (!_presetLoaded) return;
            var pen = new Pen
            {
                Brush = Brushes.Black,
                Thickness = 0.3f,
                // pen.LineJoin = PenLineJoin.Round;
                EndLineCap = PenLineCap.Round,
                StartLineCap = PenLineCap.Round
            };
            pen.Freeze();

            // Create a DrawingGroup
            DrawingGroup dGroup = new DrawingGroup();

            var states = new Stack<LSystem.State>();

            LSystem.State state = new LSystem.State
            {
                Position = new Point(0, 0),
                Direction = new Point(0, -1),
                Angle = _currentPreset.Angle,
                Size = 1
            };

            using (DrawingContext dc = dGroup.Open())
            {
                foreach (var c in _instructions)
                {
                    double angle;
                    switch (c)
                    {
                        case 'F':
                            var dx = state.Direction.X * state.Size;
                            var dy = state.Direction.Y * state.Size;
                            var newPos = new Point(state.Position.X + dx, state.Position.Y + dy);
                            dc.DrawLine(pen, state.Position, newPos);
                            state.Position = newPos;
                            break;
                        case 'f':
                            var distanceX = state.Direction.X * state.Size;
                            var distanceY = state.Direction.Y * state.Size;
                            state.Position = new Point(state.Position.X + distanceX, state.Position.Y + distanceY);
                            break;
                        case '+':
                            angle = state.Angle * Math.PI / 180.0;
                            angle *= -1;
                            state.Direction = new Point(state.Direction.X * Math.Cos(angle) - state.Direction.Y * Math.Sin(angle),
                                state.Direction.X * Math.Sin(angle) + state.Direction.Y * Math.Cos(angle));
                            break;
                        case '-':
                            angle = state.Angle * Math.PI / 180.0;
                            state.Direction = new Point(state.Direction.X * Math.Cos(angle) - state.Direction.Y * Math.Sin(angle),
                                state.Direction.X * Math.Sin(angle) + state.Direction.Y * Math.Cos(angle));
                            break;
                        case '[':
                            states.Push(state.Clone());
                            break;
                        case ']':
                            state = states.Pop();
                            break;
                        case '|':
                            angle = 180 * Math.PI / 180.0;
                            state.Direction = new Point(state.Direction.X * Math.Cos(angle) - state.Direction.Y * Math.Sin(angle),
                                state.Direction.X * Math.Sin(angle) + state.Direction.Y * Math.Cos(angle));
                            break;
                        case '!':
                            state.Angle *= -1;
                            break;
                    }
                }
            }

            // Display the drawing using an image control.
            Image theImage = new Image();
            DrawingImage dImageSource = new DrawingImage(dGroup);
            theImage.Source = dImageSource;

            this.Content = theImage;

        }
    }
}
