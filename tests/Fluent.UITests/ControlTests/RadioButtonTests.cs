using Fluent.UITests.TestUtilities;
using FluentAssertions.Execution;
using System.Windows.Shapes;
using System.Windows.Media;
using Xunit.Abstractions;
using System.Windows.Documents;

namespace Fluent.UITests.ControlTests
{
    public class RadioButtonTests : BaseControlTests
    {
        private ITestOutputHelper _outputHelper;
        public RadioButtonTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            SetupTestRadioButton();
            TestRadioButton1.Should().NotBeNull();
            TestRadioButton2.Should().NotBeNull();
            TestRadioButton3.Should().NotBeNull();
            TestRadioButton4.Should().NotBeNull();
        }
        #region DefaultTests

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void RadioButton_Initialization_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "");
            VerifyControlProperties(TestRadioButton1, rd);
        }
        [WpfFact]
        public void RadioButton_GroupNameSelectionDeselection_Test()
        {
            TestWindow.Show();
            TestRadioButton1.IsChecked=true;
            TestRadioButton2.IsChecked=true;
            Assert.False(TestRadioButton1.IsChecked);
            Assert.True(TestRadioButton2.IsChecked);    
        }
        [WpfFact]
        public void RadioButton_GroupNameSelection_Test()
        {
            TestWindow.Show();
            TestRadioButton1.IsChecked = true;
            Assert.True(TestRadioButton1.IsChecked);
            Assert.False(TestRadioButton2.IsChecked);
            Assert.False(TestRadioButton3.IsChecked);
        }

        [WpfFact]
        public void RadioButton_SelectionWithoutGroupName_Test()
        {
            TestWindow.Show();
            TestRadioButton1.IsChecked = true;
            TestRadioButton4.IsChecked = true;
            Assert.True(TestRadioButton1.IsChecked);
            Assert.True(TestRadioButton4.IsChecked);
            Assert.False(TestRadioButton2.IsChecked);
            Assert.False(TestRadioButton3.IsChecked);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void RadioButton_UncheckedDisabled_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestRadioButton1.IsEnabled = false;
            TestWindow.Show();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "Disabled");
            VerifyControlProperties(TestRadioButton1, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void RadioButton_Checked_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestRadioButton1.IsChecked = true;
            TestWindow.Show();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "Checked");
            VerifyControlProperties(TestRadioButton1, rd);
        }
        #endregion

        #region CustomTests
        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void RadioButton_CustomSolidColorBrush_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            SetSolidColorBrushProperties();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "CustomizedBrush");
            VerifyControlProperties(TestRadioButton1, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void RadioButton_CustomSolidColorBrushChecked_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            TestRadioButton1.IsChecked = true;
            SetSolidColorBrushProperties();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "CheckedCustomizedBrush");
            VerifyControlProperties(TestRadioButton1, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void RadioButton_Custom_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            SetCustomizedProperties();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "CustomRadioButton");
            VerifyControlProperties(TestRadioButton1, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void RadioButton_CustomChecked_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            TestRadioButton1.IsChecked = true;
            SetCustomizedProperties();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "CustomRadioButtonChecked");
            VerifyControlProperties(TestRadioButton1, rd);
        }

        #endregion

        #region Override Methods

        public override List<FrameworkElement> GetStyleParts(Control element)
        {
            List<FrameworkElement> templateParts = [element];

            Border? rootBorder = element.Template.FindName("RootBorder", element) as Border;
            rootBorder.Should().NotBeNull();

            templateParts.Add(rootBorder);

            Ellipse? outerEllipse = element.Template.FindName("OuterEllipse", element) as Ellipse;
            outerEllipse.Should().NotBeNull();

            templateParts.Add(outerEllipse);

            Ellipse? CheckOuterEllipse = element.Template.FindName("CheckOuterEllipse", element) as Ellipse;
            CheckOuterEllipse.Should().NotBeNull();

            templateParts.Add(CheckOuterEllipse);

            Ellipse? checkGlyph = element.Template.FindName("CheckGlyph", element) as Ellipse;
            checkGlyph.Should().NotBeNull();

            templateParts.Add(checkGlyph);

            Border? pressedCheckGlyph = element.Template.FindName("PressedCheckGlyph", element) as Border;
            pressedCheckGlyph.Should().NotBeNull();

            templateParts.Add(pressedCheckGlyph);

            ContentPresenter? contentPresenter = element.Template.FindName("ContentPresenter", element) as ContentPresenter;
            contentPresenter.Should().NotBeNull();

            templateParts.Add(contentPresenter);

            return templateParts;
        }
        public override void VerifyControlProperties(FrameworkElement element, ResourceDictionary expectedProperties)
        {
            if (element is not RadioButton radiobutton) return;

            List<FrameworkElement> parts = GetStyleParts(radiobutton);

            RadioButton part_RadioButton= (RadioButton)parts[0];
            Border? part_RootBorder = parts[1] as Border;
            Ellipse? part_OuterEllipse = parts[2] as Ellipse;
            Ellipse? part_CheckOuterEllipse = parts[3] as Ellipse;
            Ellipse? part_checkGlyph = parts[4] as Ellipse;
            Border? part_pressedCheckGlyph = parts[5] as Border;
            ContentPresenter? part_ContentPresenter = parts[6] as ContentPresenter;

            using (new AssertionScope())
            {
                //validate RadioButton properties
                VerifyRadioButtonProperties(part_RadioButton, expectedProperties);
                //validate rootborder properties
                VerifyRootBorderProperties(part_RootBorder, expectedProperties);
                ////validate outer ellipse properties
                VerifyOuterEllipseProperties(part_OuterEllipse, expectedProperties);
                ////validate check outer ellipse properties
                VerifyCheckOuterEllipseProperties(part_CheckOuterEllipse, expectedProperties);
                ////validate CheckGlyph properties
                VerifyCheckGlyphEllipseProperties(part_checkGlyph, expectedProperties);
                ////validate PressedCheckGlyph properties
                VerifyPressedCheckGlyphProperties(part_pressedCheckGlyph, expectedProperties);
                //validate content presenter properties
                VerifyContentPresenterProperties(part_ContentPresenter, expectedProperties);
            }
        }
        #endregion

        #region Private Methods
        private void SetupTestRadioButton()
        {
            TestRadioButton1 = new RadioButton { Content = "Option 1", GroupName = "Group1" };
            TestRadioButton2 = new RadioButton { Content = "Option 2", GroupName = "Group1" };
            TestRadioButton3 = new RadioButton { Content = "Option 3", GroupName = "Group1" };
            TestRadioButton4 = new RadioButton { Content = "Option 4" };
            AddControlToView(TestWindow, TestRadioButton1);
            AddControlToView(TestWindow, TestRadioButton2);
            AddControlToView(TestWindow, TestRadioButton3);
            AddControlToView(TestWindow, TestRadioButton4);
        }

        private static void VerifyRadioButtonProperties(RadioButton? part_RadioButton, ResourceDictionary expectedProperties)
        {
            part_RadioButton.Should().NotBeNull();
            BrushComparer.Equal(part_RadioButton.Background, (Brush)expectedProperties["RadioButton_Background"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_RadioButton.Background, (Brush)expectedProperties["RadioButton_Background"]))
            {
                Console.WriteLine("part_RadioButton.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_RadioButton.Background, (Brush)expectedProperties["RadioButton_Background"]);
            }
            BrushComparer.Equal(part_RadioButton.Foreground, (Brush)expectedProperties["RadioButton_Foreground"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_RadioButton.Foreground, (Brush)expectedProperties["RadioButton_Foreground"]))
            {
                Console.WriteLine("part_RadioButton.Foreground does not match expected value");
                BrushComparer.LogBrushDifference(part_RadioButton.Foreground, (Brush)expectedProperties["RadioButton_Foreground"]);
            }
            BrushComparer.Equal(part_RadioButton.BorderBrush, (Brush)expectedProperties["RadioButton_BorderBrush"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_RadioButton.BorderBrush, (Brush)expectedProperties["RadioButton_BorderBrush"]))
            {
                Console.WriteLine("part_RadioButton.BorderBrush does not match expected value");
                BrushComparer.LogBrushDifference(part_RadioButton.BorderBrush, (Brush)expectedProperties["RadioButton_BorderBrush"]);
            }
            part_RadioButton.Padding.Should().Be(expectedProperties["RadioButton_Padding"]);
            part_RadioButton.BorderThickness.Should().Be(expectedProperties["RadioButton_BorderThickness"]);
            part_RadioButton.MinWidth.Should().Be((Double)expectedProperties["RadioButton_MinWidth"]);
            part_RadioButton.HorizontalAlignment.Should().Be((HorizontalAlignment?)expectedProperties["RadioButton_HorizontalAlignment"]);
            part_RadioButton.VerticalAlignment.Should().Be((VerticalAlignment?)expectedProperties["RadioButton_VerticalAlignment"]);
            part_RadioButton.HorizontalContentAlignment.Should().Be((HorizontalAlignment?)expectedProperties["RadioButton_HorizontalContentAlignment"]);
            part_RadioButton.VerticalContentAlignment.Should().Be((VerticalAlignment?)expectedProperties["RadioButton_VerticalContentAlignment"]);
            part_RadioButton.FontWeight.Should().Be(expectedProperties["RadioButton_FontWeight"]);
        }
        private static void VerifyRootBorderProperties(Border? part_RootBorder, ResourceDictionary expectedProperties)
        {
            part_RootBorder.Should().NotBeNull();
            BrushComparer.Equal(part_RootBorder.Background, (Brush)expectedProperties["RootBorder_Background"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_RootBorder.Background, (Brush)expectedProperties["RootBorder_Background"]))
            {
                Console.WriteLine("part_RootBorder.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_RootBorder.Background, (Brush)expectedProperties["RootBorder_Background"]);
            }
            BrushComparer.Equal(part_RootBorder.BorderBrush, (Brush)expectedProperties["RootBorder_BorderBrush"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_RootBorder.BorderBrush, (Brush)expectedProperties["RootBorder_BorderBrush"]))
            {
                Console.WriteLine("part_RootBorder.BorderBrush does not match expected value");
                BrushComparer.LogBrushDifference(part_RootBorder.BorderBrush, (Brush)expectedProperties["RootBorder_BorderBrush"]);
            }
            part_RootBorder.CornerRadius.Should().Be(expectedProperties["RootBorder_CornerRadius"]);
            part_RootBorder.BorderThickness.Should().Be(expectedProperties["RootBorder_BorderThickness"]);
        }
        private static void VerifyOuterEllipseProperties(Ellipse? part_OuterEllipse, ResourceDictionary expectedProperties)
        {
            part_OuterEllipse.Should().NotBeNull();
            BrushComparer.Equal(part_OuterEllipse.Fill, (Brush)expectedProperties["OuterEllipse_Fill"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_OuterEllipse.Fill, (Brush)expectedProperties["OuterEllipse_Fill"]))
            {
                Console.WriteLine("part_OuterEllipse.Fill does not match expected value");
                BrushComparer.LogBrushDifference(part_OuterEllipse.Fill, (Brush)expectedProperties["OuterEllipse_Fill"]);
            }
            BrushComparer.Equal(part_OuterEllipse.Stroke, (Brush)expectedProperties["OuterEllipse_Stroke"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_OuterEllipse.Stroke, (Brush)expectedProperties["OuterEllipse_Stroke"]))
            {
                Console.WriteLine("part_OuterEllipse.Stroke does not match expected value");
                BrushComparer.LogBrushDifference(part_OuterEllipse.Stroke, (Brush)expectedProperties["OuterEllipse_Stroke"]);
            }
            part_OuterEllipse.StrokeThickness.Should().Be((Double)expectedProperties["OuterEllipse_StrokeThickness"]);
            part_OuterEllipse.Width.Should().Be((Double)expectedProperties["OuterEllipse_Width"]);
            part_OuterEllipse.Height.Should().Be((Double)expectedProperties["OuterEllipse_Height"]);
        }
        private static void VerifyCheckOuterEllipseProperties(Ellipse? part_CheckOuterEllipse, ResourceDictionary expectedProperties)
        {
            part_CheckOuterEllipse.Should().NotBeNull();
            BrushComparer.Equal(part_CheckOuterEllipse.Fill, (Brush)expectedProperties["CheckOuterEllipse_Fill"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_CheckOuterEllipse.Fill, (Brush)expectedProperties["CheckOuterEllipse_Fill"]))
            {
                Console.WriteLine("part_CheckOuterEllipse.Fill does not match expected value");
                BrushComparer.LogBrushDifference(part_CheckOuterEllipse.Fill, (Brush)expectedProperties["CheckOuterEllipse_Fill"]);
            }
            BrushComparer.Equal(part_CheckOuterEllipse.Stroke, (Brush)expectedProperties["CheckOuterEllipse_Stroke"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_CheckOuterEllipse.Stroke, (Brush)expectedProperties["CheckOuterEllipse_Stroke"]))
            {
                Console.WriteLine("part_CheckOuterEllipse.Stroke does not match expected value");
                BrushComparer.LogBrushDifference(part_CheckOuterEllipse.Stroke, (Brush)expectedProperties["CheckOuterEllipse_Stroke"]);
            }
            part_CheckOuterEllipse.StrokeThickness.Should().Be((Double)expectedProperties["CheckOuterEllipse_StrokeThickness"]);
            part_CheckOuterEllipse.Width.Should().Be((Double)expectedProperties["CheckOuterEllipse_Width"]);
            part_CheckOuterEllipse.Height.Should().Be((Double)expectedProperties["CheckOuterEllipse_Height"]);
            part_CheckOuterEllipse.Opacity.Should().Be((Double)expectedProperties["CheckOuterEllipse_Opacity"]);
        }
        private static void VerifyCheckGlyphEllipseProperties(Ellipse? part_checkGlyph, ResourceDictionary expectedProperties)
        {
            part_checkGlyph.Should().NotBeNull();
            BrushComparer.Equal(part_checkGlyph.Fill, (Brush)expectedProperties["CheckGlyph_Fill"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_checkGlyph.Fill, (Brush)expectedProperties["CheckGlyph_Fill"]))
            {
                Console.WriteLine("part_checkGlyph.Fill does not match expected value");
                BrushComparer.LogBrushDifference(part_checkGlyph.Fill, (Brush)expectedProperties["CheckGlyph_Fill"]);
            }
            //BrushComparer.Equal(part_checkGlyph.Stroke, (Brush)expectedProperties["CheckGlyph_Stroke"]).Should().BeTrue();
            //if (!BrushComparer.Equal(part_checkGlyph.Stroke, (Brush)expectedProperties["CheckGlyph_Stroke"]))
            //{
            //    Console.WriteLine("part_checkGlyph.Stroke does not match expected value");
            //    BrushComparer.LogBrushDifference(part_checkGlyph.Stroke, (Brush)expectedProperties["CheckGlyph_Stroke"]);
            //}
            part_checkGlyph.Width.Should().Be((Double)expectedProperties["CheckGlyph_Width"]);
            part_checkGlyph.Height.Should().Be((Double)expectedProperties["CheckGlyph_Height"]);
            part_checkGlyph.Opacity.Should().Be((Double)expectedProperties["CheckGlyph_Opacity"]);
        }
        private static void VerifyPressedCheckGlyphProperties(Border? part_pressedCheckGlyph, ResourceDictionary expectedProperties)
        {
            part_pressedCheckGlyph.Should().NotBeNull();
            BrushComparer.Equal(part_pressedCheckGlyph.Background, (Brush)expectedProperties["PressedCheckGlyph_Background"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_pressedCheckGlyph.Background, (Brush)expectedProperties["PressedCheckGlyph_Background"]))
            {
                Console.WriteLine("part_pressedCheckGlyph.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_pressedCheckGlyph.Background, (Brush)expectedProperties["PressedCheckGlyph_Background"]);
            }
            //BrushComparer.Equal(part_pressedCheckGlyph.BorderBrush, (Brush)expectedProperties["PressedCheckGlyph_BorderBrush"]).Should().BeTrue();
            //if (!BrushComparer.Equal(part_pressedCheckGlyph.BorderBrush, (Brush)expectedProperties["PressedCheckGlyph_BorderBrush"]))
            //{
            //    Console.WriteLine("part_pressedCheckGlyph.BorderBrush does not match expected value");
            //    BrushComparer.LogBrushDifference(part_pressedCheckGlyph.BorderBrush, (Brush)expectedProperties["PressedCheckGlyph_BorderBrush"]);
            //}
            part_pressedCheckGlyph.CornerRadius.Should().Be(expectedProperties["PressedCheckGlyph_CornerRadius"]);
            part_pressedCheckGlyph.Width.Should().Be((Double)expectedProperties["PressedCheckGlyph_Width"]);
            part_pressedCheckGlyph.Height.Should().Be((Double)expectedProperties["PressedCheckGlyph_Height"]);
            part_pressedCheckGlyph.Opacity.Should().Be((Double)expectedProperties["PressedCheckGlyph_Opacity"]);
        }
        private static void VerifyContentPresenterProperties(ContentPresenter? part_ContentPresenter, ResourceDictionary expectedProperties)
        {
            part_ContentPresenter.Should().NotBeNull();

            BrushComparer.Equal(TextElement.GetForeground(part_ContentPresenter), (Brush)expectedProperties["ContentPresenter_Foreground"]).Should().BeTrue();
            if (!BrushComparer.Equal(TextElement.GetForeground(part_ContentPresenter), (Brush)expectedProperties["ContentPresenter_Foreground"]))
            {
                Console.WriteLine("part_ContentPresenter.Foreground does not match expected value");
                BrushComparer.LogBrushDifference(TextElement.GetForeground(part_ContentPresenter), (Brush)expectedProperties["ContentPresenter_Foreground"]);
            }
            part_ContentPresenter.HorizontalAlignment.Should().Be((HorizontalAlignment)expectedProperties["ContentPresenter_HorizontalAlignment"]);
            part_ContentPresenter.VerticalAlignment.Should().Be((VerticalAlignment)expectedProperties["ContentPresenter_VerticalAlignment"]);
            part_ContentPresenter.Margin.Should().Be((Thickness)expectedProperties["ContentPresenter_Margin"]);
        }
        private void SetSolidColorBrushProperties()
        {
            TestRadioButton1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Aqua"));
            TestRadioButton1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Maroon"));
            TestRadioButton1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
        }
        private void SetCustomizedProperties()
        {
            TestRadioButton1.BorderThickness = new Thickness(4);
            TestRadioButton1.Padding = new Thickness(8, 5, 10, 10);
            TestRadioButton1.MinWidth = 90;
            TestRadioButton1.FontWeight = FontWeights.Bold;
            TestRadioButton1.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            TestRadioButton1.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            TestRadioButton1.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
            TestRadioButton1.VerticalContentAlignment = System.Windows.VerticalAlignment.Bottom;
        }
        #endregion

        #region Private Properties

        private RadioButton TestRadioButton1 { get; set; }
        private RadioButton TestRadioButton2 { get; set; }
        private RadioButton TestRadioButton3 { get; set; }
        private RadioButton TestRadioButton4 { get; set; }

        protected override string TestDataDictionaryPath => @"/Fluent.UITests;component/ControlTests/Data/RadioButtonTests.xaml";

        #endregion
        
    }
}
