using System;
using System.Collections.Generic;
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
        
        private string _axiom = "F";
        private Dictionary<char, string> _rules = new()
        {
            ['F'] = "F+F--F+F",
            ['+'] = "+",
            ['-'] = "-"
        };
        private float _angle = 60;
        private float _length = 1;

        private int _iterations = 5;

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

        

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var pen = new Pen
            {
                Brush = Brushes.Black,
                Thickness = 0.05f,
                // pen.LineJoin = PenLineJoin.Round;
                EndLineCap = PenLineCap.Round,
                StartLineCap = PenLineCap.Round
            };
            pen.Freeze();

            // Create a DrawingGroup
            DrawingGroup dGroup = new DrawingGroup();

            var currentPosition = new Point(0, 0);
            var direction = new Point(1, 0);

            // Obtain a DrawingContext from 
            // the DrawingGroup.
            using (DrawingContext dc = dGroup.Open())
            {
                foreach (var c in _final)
                {
                    switch (c)
                    {
                        case 'F':
                            var dx = direction.X * _length;
                            var dy = direction.Y * _length;
                            var newPos = new Point( currentPosition.X + dx, currentPosition.Y + dy);
                            dc.DrawLine(pen, currentPosition, newPos);
                            currentPosition = newPos;
                            break;
                        case '+':
                            var angle = _angle * Math.PI / 180.0;
                            var angleNeg = angle *= -1;
                            direction = new Point(direction.X * Math.Cos(angleNeg) - direction.Y * Math.Sin(angleNeg),
                                direction.X * Math.Sin(angleNeg) + direction.Y * Math.Cos(angleNeg));
                            
                            break;
                        case '-':
                            var anglePos = _angle * Math.PI / 180.0;
                            direction = new Point(direction.X * Math.Cos(anglePos) - direction.Y * Math.Sin(anglePos),
                                direction.X * Math.Sin(anglePos) + direction.Y * Math.Cos(anglePos));
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
