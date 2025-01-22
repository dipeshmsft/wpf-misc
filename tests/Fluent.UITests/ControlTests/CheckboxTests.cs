using Fluent.UITests.TestUtilities;
using FluentAssertions.Execution;
using System.Windows.Media;
using Xunit.Abstractions;

namespace Fluent.UITests.ControlTests
{
    public class CheckboxTests : BaseControlTests
    {
        private ITestOutputHelper _outputHelper;
        public CheckboxTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            SetupTestCheckbox();
            TestCheckbox.Should().NotBeNull();           

        }

        #region DefaultTests
        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Checkbox_Initialization_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();

            ResourceDictionary rd = GetTestDataDictionary(colorMode, "");
            VerifyControlProperties(TestCheckbox, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Checkbox_Checked_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestCheckbox.IsChecked = true;
            TestWindow.Show();

            ResourceDictionary rd = GetTestDataDictionary(colorMode, "Checked");
            VerifyControlProperties(TestCheckbox, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Checkbox_Indeterminate_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestCheckbox.IsChecked = null;
            TestCheckbox.IsThreeState = true;
            TestWindow.Show();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "Indeterminate");
            VerifyControlProperties(TestCheckbox, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Checkbox_UncheckedDisabled_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestCheckbox.IsEnabled = false;
            TestWindow.Show();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "UncheckedDisabled");
            VerifyControlProperties(TestCheckbox, rd);
        }

        //[WpfTheory]
        //[MemberData(nameof(ColorModes_TestData))]
        //public void Checkbox_CheckedDisabled_Test(ColorMode colorMode)
        //{
        //    SetColorMode(TestWindow, colorMode);
        //    TestCheckbox.IsEnabled = false;
        //    TestCheckbox.IsChecked = true;
        //    TestWindow.Show();

        //    ResourceDictionary rd = GetTestDataDictionary(colorMode, "CheckedDisabled");
        //    VerifyControlProperties(TestCheckbox, rd);
        //}
        #endregion
        #region CustomizedTests
        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Checkbox_CustomSolidColorBrush_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            SetSolidColorBrushProperties();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "CustomizedBrush");
            VerifyControlProperties(TestCheckbox, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Checkbox_Custom_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            SetCustomizedProperties();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "CustomCheckbox");
            VerifyControlProperties(TestCheckbox, rd);
        }

        #endregion
        #region Override Methods

        public override List<FrameworkElement> GetStyleParts(Control element)
        {
            List<FrameworkElement> templateParts = [element];

            Border? rootBorder = element.Template.FindName("RootBorder", element) as Border;
            rootBorder.Should().NotBeNull();

            templateParts.Add(rootBorder);

            Border? border = element.Template.FindName("ControlBorderIconPresenter", element) as Border;
            border.Should().NotBeNull();

            templateParts.Add(border);

            Border? strokeBorder = element.Template.FindName("StrokeBorder", element) as Border;
            strokeBorder.Should().NotBeNull();

            templateParts.Add(strokeBorder);

            ContentPresenter? contentPresenter = element.Template.FindName("ContentPresenter", element) as ContentPresenter;
            contentPresenter.Should().NotBeNull();

            templateParts.Add(contentPresenter);

            TextBlock? txtBlock = element.Template.FindName("ControlIcon", element) as TextBlock;
            txtBlock.Should().NotBeNull();

            templateParts.Add(txtBlock);
            return templateParts;
        }
        public override void VerifyControlProperties(FrameworkElement element, ResourceDictionary expectedProperties)
        {
            if (element is not CheckBox checkbox) return;

            List<FrameworkElement> parts = GetStyleParts(checkbox);

            CheckBox? part_checkbox = parts[0] as CheckBox;
            Border? part_RootBorder = parts[1] as Border;
            Border? part_ControlIconBorder = parts[2] as Border;
            Border? part_StrokeBorder = parts[3] as Border;
            ContentPresenter? part_ContentPresenter = parts[4] as ContentPresenter;
            TextBlock? part_TextBlock = parts[5] as TextBlock;

            using (new AssertionScope())
            {
                //validate Checkbox properties
                VerifyCheckBoxProperties(part_checkbox, expectedProperties);
                //validate rootborder properties
                VerifyRootBorderProperties(part_RootBorder, expectedProperties);
                //validate content icon border properties
                VerifyContentIconBorderProperties(part_ControlIconBorder, expectedProperties);
                //validate stroke border properties
                VerifyStrokeBorderProperties(part_StrokeBorder, expectedProperties);
                //validate content presenter properties
                VerifyContentPresenterProperties(part_ContentPresenter, expectedProperties);
                //validate Textblock properties
                VerifyTextBlockProperties(part_TextBlock, expectedProperties);
            }
        }
        #endregion
        #region Private Methods
        private void SetupTestCheckbox()
        {
            TestCheckbox = new CheckBox() { Content = "TestCheckBox" };
            AddControlToView(TestWindow, TestCheckbox);            
        }

        private static void VerifyCheckBoxProperties(CheckBox? part_checkbox, ResourceDictionary expectedProperties)
        {
            part_checkbox.Should().NotBeNull();
            BrushComparer.Equal(part_checkbox.Background, (Brush)expectedProperties["CheckBoxBackground"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_checkbox.Background, (Brush)expectedProperties["CheckBoxBackground"]))
            {
                Console.WriteLine("part_checkbox.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_checkbox.Background, (Brush)expectedProperties["CheckBoxBackground"]);
            }
            BrushComparer.Equal(part_checkbox.Foreground, (Brush)expectedProperties["CheckBoxForeground"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_checkbox.Foreground, (Brush)expectedProperties["CheckBoxForeground"]))
            {
                Console.WriteLine("part_checkbox.Foreground does not match expected value");
                BrushComparer.LogBrushDifference(part_checkbox.Foreground, (Brush)expectedProperties["CheckBoxForeground"]);
            }
            BrushComparer.Equal(part_checkbox.BorderBrush, (Brush)expectedProperties["CheckBoxBorderBrush"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_checkbox.BorderBrush, (Brush)expectedProperties["CheckBoxBorderBrush"]))
            {
                Console.WriteLine("part_checkbox.BorderBrush does not match expected value");
                BrushComparer.LogBrushDifference(part_checkbox.BorderBrush, (Brush)expectedProperties["CheckBoxBorderBrush"]);
            }
            part_checkbox.BorderThickness.Should().Be((Thickness)expectedProperties["CheckBox_Thickness"]);
            part_checkbox.Padding.Should().Be(expectedProperties["CheckBox_Padding"]);
            part_checkbox.MinHeight.Should().Be((Double)expectedProperties["CheckBox_Height"]);
            part_checkbox.MinWidth.Should().Be((Double)expectedProperties["CheckBox_MinWidth"]);
            //part_checkbox.FontWeight.Should().Be(expectedProperties["CheckBox_Fontweight"]);
            part_checkbox.Cursor.Should().Be(expectedProperties["CheckBox_Cursor"]);
            part_checkbox.IsTabStop.Should().Be((bool)expectedProperties["Checkbox_IsTabStop"]);
        }
        private static void VerifyRootBorderProperties(Border? part_RootBorder, ResourceDictionary expectedProperties)
        {
            part_RootBorder.Should().NotBeNull();
            BrushComparer.Equal(part_RootBorder.Background, (Brush)expectedProperties["CheckBoxBackground"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_RootBorder.Background, (Brush)expectedProperties["CheckBoxBackground"]))
            {
                Console.WriteLine("part_checkbox.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_RootBorder.Background, (Brush)expectedProperties["CheckBoxBackground"]);
            }
            BrushComparer.Equal(part_RootBorder.BorderBrush, (Brush)expectedProperties["CheckBoxBorderBrush"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_RootBorder.BorderBrush, (Brush)expectedProperties["CheckBoxBorderBrush"]))
            {
                Console.WriteLine("part_checkbox.BorderBrush does not match expected value");
                BrushComparer.LogBrushDifference(part_RootBorder.BorderBrush, (Brush)expectedProperties["CheckBoxBorderBrush"]);
            }
            part_RootBorder.BorderThickness.Should().Be((Thickness)expectedProperties["CheckBox_Thickness"]);
            part_RootBorder.CornerRadius.Should().Be(expectedProperties["CheckBox_CornerRadius"]);
        }
        private static void VerifyContentIconBorderProperties(Border? part_ControlIconBorder, ResourceDictionary expectedProperties)
        {
            part_ControlIconBorder.Should().NotBeNull();
            //value mismatch for checked disabled
            BrushComparer.Equal(part_ControlIconBorder.Background, (Brush)expectedProperties["CheckBoxBackground_Fill"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ControlIconBorder.Background, (Brush)expectedProperties["CheckBoxBackground_Fill"]))
            {
                Console.WriteLine("part_ControlIconBorder.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_ControlIconBorder.Background, (Brush)expectedProperties["CheckBoxBackground_Fill"]);
            }
            part_ControlIconBorder.Width.Should().Be((Double)expectedProperties["CheckBoxSize"]);
            part_ControlIconBorder.Height.Should().Be((Double)expectedProperties["CheckBoxSize"]);
        }
        private static void VerifyStrokeBorderProperties(Border? part_StrokeBorder, ResourceDictionary expectedProperties)
        {
            part_StrokeBorder.Should().NotBeNull();
            BrushComparer.Equal(part_StrokeBorder.BorderBrush, (Brush)expectedProperties["CheckBoxBorderBrush_Stroke"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_StrokeBorder.BorderBrush, (Brush)expectedProperties["CheckBoxBorderBrush_Stroke"]))
            {
                Console.WriteLine("part_StrokeBorder.BorderBrush does not match expected value");
                BrushComparer.LogBrushDifference(part_StrokeBorder.BorderBrush, (Brush)expectedProperties["CheckBoxBorderBrush_Stroke"]);
            }
        }
        private static void VerifyContentPresenterProperties(ContentPresenter? part_ContentPresenter, ResourceDictionary expectedProperties)
        {
            part_ContentPresenter.Should().NotBeNull();
            part_ContentPresenter.RecognizesAccessKey.Should().Be((bool)expectedProperties["ContentPresenter_RecognizesAccessKey"]);
            part_ContentPresenter.HorizontalAlignment.Should().Be((HorizontalAlignment)expectedProperties["ContentPresenter_HorizontalAlignment"]);
            part_ContentPresenter.VerticalAlignment.Should().Be((VerticalAlignment)expectedProperties["ContentPresenter_VerticalAlignment"]);
            part_ContentPresenter.Margin.Should().Be((Thickness)expectedProperties["ContentPresenter_Margin"]);
        }
        private static void VerifyTextBlockProperties(TextBlock? part_TextBlock, ResourceDictionary expectedProperties)
        {
            part_TextBlock.Should().NotBeNull();
            //value mismatch
            //part_TextBlock.FontSize.Should().Be((Double)expectedProperties["CheckBoxIcon_Fontsize"]);
            part_TextBlock.Visibility.Should().Be((Visibility)expectedProperties["CheckboxTextblock_Visibility"]);
            part_TextBlock.Text.Should().Be((String)expectedProperties["CheckBoxTextblock_Text"]);
        }
        private void SetSolidColorBrushProperties()
        {
            TestCheckbox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Aqua"));
            TestCheckbox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Maroon"));
            TestCheckbox.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
        }
        private void SetCustomizedProperties()
        {
            TestCheckbox.BorderThickness = new Thickness(4);
            TestCheckbox.Padding = new Thickness(8, 5, 10, 10);
            TestCheckbox.MinHeight = 40;
            TestCheckbox.MinWidth = 150;
            TestCheckbox.IsTabStop = false;
            TestCheckbox.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            TestCheckbox.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            // TestCheckbox.Cursor = Cursors.Hand;
            TestCheckbox.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
            TestCheckbox.VerticalContentAlignment = System.Windows.VerticalAlignment.Bottom;
        }

        #endregion
        #region Private Properties

        private CheckBox TestCheckbox { get; set; }

        protected override string TestDataDictionaryPath => @"/Fluent.UITests;component/ControlTests/Data/CheckboxTests.xaml";

        #endregion
    }
}
