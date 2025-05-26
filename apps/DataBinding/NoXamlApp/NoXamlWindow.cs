using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Media3D;

namespace NoXamlApp
{
    public class NoXamlWindow : Window
    {
        public void InitializeComponent()
        {
            Title = "No XAML Window";
            Width = 400;
            Height = 600;

            StackPanel sp = new() { Orientation = Orientation.Vertical };
            Button button = new() { Content = "Update Source" };
            button.Click += Button_Click;
            TextBox tb = new() { Height = 50 };

            Binding textBoxBinding = new Binding("TextBoxContent")
            {
                Source = _vm,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            tb.SetBinding(TextBox.TextProperty, textBoxBinding);


            sp.Children.Add(tb);
            sp.Children.Add(button);

            //Button btn2 = new() { Content = "Delete TextBox" };
            //btn2.Click += Btn2_Click;
            //sp.Children.Add(btn2);
            //_sp = sp;

            Content = sp;
        }

        //private void Btn2_Click(object sender, RoutedEventArgs e)
        //{
        //    TextBox _tb = _sp.Children[0] as TextBox;
        //    _sp.Children.RemoveAt(0);
        //}

        //private StackPanel _sp;

        private ViewModel _vm = new();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _vm.TextBoxContent = "Source Changed";
        }
    }
}
