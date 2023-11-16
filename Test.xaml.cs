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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace L_System_Renderer
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Page
    {
        
        private string _axiom = "X";
        private Dictionary<char, string> _rules = new()
        {
            ['X'] = "F[+X][-X]FX",
            ['F'] = "FF",
            ['+'] = "+",
            ['-'] = "-",
            ['['] = "[",
            [']'] = "]"
        };
        // private float _angle = 60;
        // private float _length = 1;

        private int _iterations = 7;

        private string _final;

        

        public Test()
        {
            InitializeComponent();

            _final = _axiom;
            for(int i = 0; i < _iterations; i++) 
            {
                _final = LSystem(_final, _rules);
            }

        }

        private string LSystem(string start, Dictionary<char, string> rules)
        {
            var outString = "";

            foreach(var c in start)
            {
                var s = rules[c];
                outString += s;
            }

            return outString;
        }

        private class State
        {
            public double Size;
            public double Angle;
            public Point Position;
            public Point Direction;

            public State Clone() { return (State)this.MemberwiseClone(); }
        }

        

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
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

            State state = new State
            {
                Position = new Point(0, 0),
                Direction = new Point(0, -1),
                Angle = 25.7,
                Size = 1
            };

            var states = new Stack<State>();

            // Obtain a DrawingContext from 
            // the DrawingGroup.
            using (DrawingContext dc = dGroup.Open())
            {
                foreach (var c in _final)
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
