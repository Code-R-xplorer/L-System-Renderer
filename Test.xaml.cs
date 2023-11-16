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
        private Dictionary<string, string> _rules;
        private float _angle = MathF.PI / 3;
        private float _length = 1;

        private int _iterations = 1;

        private string _final;

        public Test()
        {
            _rules = new Dictionary<string, string>();
            _rules["F"] = "F+F--F+F";
            _rules["+"] = "+";
            _rules["-"] = "-";

            InitializeComponent();

            _final = _axiom;
            for(int i = 0; i < _iterations; i++) 
            {
                _final = lsystem(_final, _rules);
            }

        }

        private string lsystem(string start, Dictionary<string, string> rules)
        {
            var outString = "";

            foreach(var c in start)
            {
                var s = rules[c.ToString()];
                outString += s;
            }

            return outString;
        }

        

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            Pen shapeOutlinePen = new Pen(Brushes.Black, 2);
            shapeOutlinePen.Freeze();

            // Create a DrawingGroup
            DrawingGroup dGroup = new DrawingGroup();

            // Obtain a DrawingContext from 
            // the DrawingGroup.
            using (DrawingContext dc = dGroup.Open())
            {
                
            }

            // Display the drawing using an image control.
            Image theImage = new Image();
            DrawingImage dImageSource = new DrawingImage(dGroup);
            theImage.Source = dImageSource;

            this.Content = theImage;
        }
    }
}
