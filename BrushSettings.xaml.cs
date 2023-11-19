﻿using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace L_System_Renderer
{
    /// <summary>
    /// Interaction logic for BrushSettings.xaml
    /// </summary>
    public partial class BrushSettings : Window
    {
        private LSystemRenderer _renderer;
        public BrushSettings(LSystemRenderer renderer)
        {
            _renderer = renderer;
            InitializeComponent();
            Thickness.Text = _renderer.BrushThickness.ToString();
            if (_renderer.Brush == Brushes.Black)
            {
                Colours.SelectedIndex = 0;
            }
            if (_renderer.Brush == Brushes.Blue)
            {
                Colours.SelectedIndex = 1;
            }
            if (_renderer.Brush == Brushes.Red)
            {
                Colours.SelectedIndex = 2;
            }
            if (_renderer.Brush == Brushes.Yellow)
            {
                Colours.SelectedIndex = 3;
            }
            if (_renderer.Brush == Brushes.Green)
            {
                Colours.SelectedIndex = 4;
            }
            if (_renderer.Brush == Brushes.Cyan)
            {
                Colours.SelectedIndex = 5;
            }
            
        }

        private void Thickness_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _renderer.BrushThickness = Convert.ToDouble(Thickness.Text);
                _renderer.Redraw();
            }
                
        }

        private void Colours_OnDropDownClosed(object? sender, EventArgs e)
        {
            switch (Colours.Text)
            {
                case "Black":
                    _renderer.Brush = Brushes.Black;
                    _renderer.Redraw();
                    break;
                case "Blue":
                    _renderer.Brush = Brushes.Blue;
                    _renderer.Redraw();
                    break;
                case "Red":
                    _renderer.Brush = Brushes.Red;
                    _renderer.Redraw();
                    break;
                case "Yellow":
                    _renderer.Brush = Brushes.Yellow;
                    _renderer.Redraw();
                    break;
                case "Green":
                    _renderer.Brush = Brushes.Green;
                    _renderer.Redraw();
                    break;
                case "Cyan":
                    _renderer.Brush = Brushes.Cyan;
                    _renderer.Redraw();
                    break;
            }
        }
    }
}