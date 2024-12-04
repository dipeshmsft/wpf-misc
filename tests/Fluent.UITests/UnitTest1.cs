using System.Drawing;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;

namespace Fluent.UITests
{
    public class UnitTest1
    {
        [WpfFact]
        public void Test1()
        {
            Window window = new Window();
            window.ThemeMode = ThemeMode.System;

            var stackPanel = new StackPanel();

            var rb = new RadioButton();
            rb.IsEnabled = false;
            stackPanel.Children.Add(rb);
            window.Content = stackPanel;
            window.Show();

            Style style = rb.Style;

            Border? border = rb.Template.FindName("RootBorder", rb) as Border;
            Assert.NotNull(border);
            Brush br = border.Background as Brush;
            if(br is SolidColorBrush scb)
            {
                Assert.Equal(scb.Color, Colors.Transparent);
            }

            var grid = rb.Template.FindName("RootGrid", rb);
            Assert.NotNull(grid);

        }
    }
}
