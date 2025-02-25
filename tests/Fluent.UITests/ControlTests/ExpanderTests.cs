using Fluent.UITests.TestUtilities;
using FluentAssertions.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using Xunit.Abstractions;

namespace Fluent.UITests.ControlTests
{
    public class ExpanderTests : BaseControlTests
    {
        private ITestOutputHelper _outputHelper;

        public ExpanderTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            SetUpExpander();
            TestExpander.Should().NotBeNull();
        }

        #region Default Tests
        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Expander_Initialization_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            // TestExpander.IsExpanded = true;
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "");
            VerifyControlProperties(TestExpander, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Expander_DownExpansion_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            TestExpander.IsExpanded = true;
            TestExpander.ExpandDirection = ExpandDirection.Down;
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "ExpandDown");
            VerifyControlProperties(TestExpander, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Expander_UpExpansion_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            TestExpander.IsExpanded = true;
            TestExpander.ExpandDirection = ExpandDirection.Up;
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "ExpandUp");
            VerifyControlProperties(TestExpander, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Expander_LeftExpansion_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            TestExpander.IsExpanded = true;
            TestExpander.ExpandDirection = ExpandDirection.Left;
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "ExpandLeft");
            VerifyControlProperties(TestExpander, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Expander_RightExpansion_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            TestExpander.IsExpanded = true;
            TestExpander.ExpandDirection = ExpandDirection.Right;
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "ExpandRight");
            VerifyControlProperties(TestExpander, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Expander_Disabled_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            TestExpander.IsEnabled = false;
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "Disabled");
            VerifyControlProperties(TestExpander, rd);
        }

        #endregion

        #region Custom Test
        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Expander_CustomSolidColorBrush_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            SetSolidColorBrushProperties();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "CustomBrush");
            VerifyControlProperties(TestExpander, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Expander_Custom_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            SetCustomizedProperties();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "Custom");
            VerifyControlProperties(TestExpander, rd);
        }
        #endregion
        #region Override Methods

        public override List<FrameworkElement> GetStyleParts(Control element)
        {
            List<FrameworkElement> templateParts = [element];

            Border? toggleButtonBorder = element.Template.FindName("ToggleButtonBorder", element) as Border;
            toggleButtonBorder.Should().NotBeNull();

            templateParts.Add(toggleButtonBorder);

            ToggleButton? toggleButton = element.Template.FindName("HeaderSite", element) as ToggleButton;
            toggleButton.Should().NotBeNull();

            templateParts.Add(toggleButton);

            if (TestExpander.ExpandDirection == ExpandDirection.Down)
            {
                Grid? toggleButtonDownGrid = GetChildOfType<Grid>(toggleButton);
                toggleButtonDownGrid.Should().NotBeNull();

                templateParts.Add(toggleButtonDownGrid);

                ContentPresenter? toggleButtonDownContentPresenter = GetChildOfType<ContentPresenter>(toggleButtonDownGrid);
                toggleButtonDownContentPresenter.Should().NotBeNull();

                templateParts.Add(toggleButtonDownContentPresenter);

                Grid? ChevronGrid = GetChildOfType<Grid>(toggleButtonDownGrid);
                ChevronGrid.Should().NotBeNull();

                templateParts.Add(ChevronGrid);

                TextBlock? ControlChevronIcon = GetChildOfType<TextBlock>(ChevronGrid);
                ControlChevronIcon.Should().NotBeNull();

                templateParts.Add(ControlChevronIcon);
            }
            Border? contentPresenterBorder = element.Template.FindName("ContentPresenterBorder", element) as Border;
            contentPresenterBorder.Should().NotBeNull();

            templateParts.Add(contentPresenterBorder);

            ContentPresenter? contentPresenter = element.Template.FindName("ContentPresenter", element) as ContentPresenter;
            contentPresenter.Should().NotBeNull();

            templateParts.Add(contentPresenter);
            return templateParts;
        }
        public override void VerifyControlProperties(FrameworkElement element, ResourceDictionary expectedProperties)
        {
            if (element is not Expander expander) return;

            List<FrameworkElement> parts = GetStyleParts(expander);

            Expander? part_expander = parts[0] as Expander;
            Border? part_ToggleButtonBorder = parts[1] as Border;
            ToggleButton? part_ToggleButton = parts[2] as ToggleButton;
            Grid? part_ToggleButtonGrid = null;
            ContentPresenter? part_ToggleButtonContentPresenter = null;
            Grid? part_ChevronGrid = null;
            TextBlock? part_ControlChevronIcon = null;
            Border? part_contentPresenterBorder = null;
            ContentPresenter? part_ContentPresenter = null;

            if (TestExpander.ExpandDirection == ExpandDirection.Down)
            {
                part_ToggleButtonGrid = parts[3] as Grid;
                part_ToggleButtonContentPresenter = parts[4] as ContentPresenter;
                part_ChevronGrid = parts[5] as Grid;
                part_ControlChevronIcon = parts[6] as TextBlock;
                part_contentPresenterBorder = parts[7] as Border;
                part_ContentPresenter = parts[8] as ContentPresenter;
            }
            else
            {
                part_contentPresenterBorder = parts[3] as Border;
                part_ContentPresenter = parts[4] as ContentPresenter;
            }
            using (new AssertionScope())
            {
                //Validate expander properties
                VerifyExpanderProperties(part_expander, expectedProperties);
                //validate toggle button border properties
                VerifyToggleButtonBorderProperties(part_ToggleButtonBorder, expectedProperties);
                //validate toggle button  properties
                VerifyToggleButtonProperties(part_ToggleButton, expectedProperties);
                if (TestExpander.ExpandDirection == ExpandDirection.Down)
                {
                    //validate toggle button grid  properties
                    VerifyToggleButtonGridProperties(part_ToggleButtonGrid, expectedProperties);
                    //validate ToggleButton ContentPresenter properties
                    VerifyToggleButtonContentPresenterProperties(part_ToggleButtonContentPresenter, expectedProperties);
                    //validate toggle button ChevronGrid  properties
                    VerifyToggleButtonChevronGridProperties(part_ChevronGrid, expectedProperties);
                    //validate textblock properties
                    VerifyToggleButtonTextblockProperties(part_ControlChevronIcon, expectedProperties);
                }
                //validate content presenter border properties
                VerifyContentPresenterBorderProperties(part_contentPresenterBorder, expectedProperties);
                //validate content presenter properties
                VerifyContentPresenterProperties(part_ContentPresenter, expectedProperties);
            }
            }
        #endregion
        #region Private Methods

        private void SetUpExpander()
        {
            TestExpander = new Expander()
            {
                Header = "Header Content",
                Content = "ExpanderContent",
                //IsExpanded = true,
            };
            AddControlToView(TestWindow, TestExpander);
        }

        private static T? GetChildOfType<T>(DependencyObject parent) where T : DependencyObject
        {
            // Loop through all the child elements of the parent
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                // If the child is of the desired type, return it
                if (child is T)
                {
                    return (T)child;
                }

                // Recursively check the children of the child element
                T? childOfChild = GetChildOfType<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }

            return null; // Return null if no child of the specified type is found
        }
        private static void VerifyExpanderProperties(Expander? part_expander, ResourceDictionary expectedProperties)
        {
            part_expander.Should().NotBeNull();
            BrushComparer.Equal(part_expander.Background, (Brush)expectedProperties["Expander_Background"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_expander.Background, (Brush)expectedProperties["Expander_Background"]))
            {
                Console.WriteLine("part_expander.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_expander.Background, (Brush)expectedProperties["Expander_Background"]);
            }
            BrushComparer.Equal(part_expander.Foreground, (Brush)expectedProperties["Expander_Foreground"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_expander.Foreground, (Brush)expectedProperties["Expander_Foreground"]))
            {
                Console.WriteLine("part_expander.Foreground does not match expected value");
                BrushComparer.LogBrushDifference(part_expander.Foreground, (Brush)expectedProperties["Expander_Foreground"]);
            }
            BrushComparer.Equal(part_expander.BorderBrush, (Brush)expectedProperties["Expander_BorderBrush"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_expander.BorderBrush, (Brush)expectedProperties["Expander_BorderBrush"]))
            {
                Console.WriteLine("part_expander.BorderBrush does not match expected value");
                BrushComparer.LogBrushDifference(part_expander.BorderBrush, (Brush)expectedProperties["Expander_BorderBrush"]);
            }
            part_expander.BorderThickness.Should().Be((Thickness)expectedProperties["Expander_BorderThickness"]);
            part_expander.Padding.Should().Be(expectedProperties["Expander_Padding"]);
            part_expander.HorizontalAlignment.Should().Be((HorizontalAlignment?)expectedProperties["Expander_HorizontalAlignment"]);
            part_expander.VerticalAlignment.Should().Be((VerticalAlignment?)expectedProperties["Expander_VerticalAlignment"]);
            part_expander.HorizontalContentAlignment.Should().Be((HorizontalAlignment?)expectedProperties["Expander_HorizontalContentAlignment"]);
            part_expander.VerticalContentAlignment.Should().Be((VerticalAlignment?)expectedProperties["Expander_VerticalContentAlignment"]);
            part_expander.FontWeight.Should().Be(expectedProperties["Expander_FontWeight"]);
        }
        private static void VerifyToggleButtonBorderProperties(Border? part_ToggleButtonBorder, ResourceDictionary expectedProperties)
        {
            part_ToggleButtonBorder.Should().NotBeNull();
            BrushComparer.Equal(part_ToggleButtonBorder.Background, (Brush)expectedProperties["ToggleButtonBorder_Background"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ToggleButtonBorder.Background, (Brush)expectedProperties["ToggleButtonBorder_Background"]))
            {
                Console.WriteLine("part_ToggleButtonBorder.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_ToggleButtonBorder.Background, (Brush)expectedProperties["ToggleButtonBorder_Background"]);
            }
            BrushComparer.Equal(part_ToggleButtonBorder.BorderBrush, (Brush)expectedProperties["ToggleButtonBorder_BorderBrush"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ToggleButtonBorder.BorderBrush, (Brush)expectedProperties["ToggleButtonBorder_BorderBrush"]))
            {
                Console.WriteLine("part_ToggleButtonBorder.BorderBrush does not match expected value");
                BrushComparer.LogBrushDifference(part_ToggleButtonBorder.BorderBrush, (Brush)expectedProperties["ToggleButtonBorder_BorderBrush"]);
            }
            part_ToggleButtonBorder.BorderThickness.Should().Be((Thickness)expectedProperties["ToggleButtonBorder_BorderThickness"]);
            part_ToggleButtonBorder.CornerRadius.Should().Be((CornerRadius)expectedProperties["ToggleButtonBorder_CornerRadius"]);
        }
        private static void VerifyToggleButtonProperties(ToggleButton? part_ToggleButton, ResourceDictionary expectedProperties)
        {
            part_ToggleButton.Should().NotBeNull();
            BrushComparer.Equal(part_ToggleButton.Foreground, (Brush)expectedProperties["ToggleButton_Foreground"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ToggleButton.Foreground, (Brush)expectedProperties["ToggleButton_Foreground"]))
            {
                Console.WriteLine("part_ToggleButton.Foreground does not match expected value");
                BrushComparer.LogBrushDifference(part_ToggleButton.Foreground, (Brush)expectedProperties["ToggleButton_Foreground"]);
            }
            part_ToggleButton.Padding.Should().Be(expectedProperties["ToggleButton_Padding"]);
            part_ToggleButton.HorizontalAlignment.Should().Be((HorizontalAlignment?)expectedProperties["ToggleButton_HorizontalAlignment"]);
            part_ToggleButton.VerticalAlignment.Should().Be((VerticalAlignment?)expectedProperties["ToggleButton_VerticalAlignment"]);
            part_ToggleButton.HorizontalContentAlignment.Should().Be((HorizontalAlignment?)expectedProperties["ToggleButton_HorizontalContentAlignment"]);
            part_ToggleButton.VerticalContentAlignment.Should().Be((VerticalAlignment?)expectedProperties["ToggleButton_VerticalContentAlignment"]);
            part_ToggleButton.FontWeight.Should().Be(expectedProperties["ToggleButton_FontWeight"]);
            part_ToggleButton.FontSize.Should().Be((Double)expectedProperties["ToggleButton_FontSize"]);
        }
        private static void VerifyToggleButtonGridProperties(Grid? part_ToggleButtonGrid, ResourceDictionary expectedProperties)
        {
            part_ToggleButtonGrid.Should().NotBeNull();
            BrushComparer.Equal(part_ToggleButtonGrid.Background, (Brush)expectedProperties["ToggleButtonGrid_Background"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ToggleButtonGrid.Background, (Brush)expectedProperties["ToggleButtonGrid_Background"]))
            {
                Console.WriteLine("part_ToggleButtonGrid.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_ToggleButtonGrid.Background, (Brush)expectedProperties["ToggleButtonGrid_Background"]);
            }
        }
        private static void VerifyToggleButtonContentPresenterProperties(ContentPresenter? part_ToggleButtonContentPresenter, ResourceDictionary expectedProperties)
        {
            part_ToggleButtonContentPresenter.Should().NotBeNull();
            part_ToggleButtonContentPresenter.HorizontalAlignment.Should().Be((HorizontalAlignment?)expectedProperties["ToggleButtonContentPresenter_HorizontalAlignment"]);
            part_ToggleButtonContentPresenter.VerticalAlignment.Should().Be((VerticalAlignment?)expectedProperties["ToggleButtonContentPresenter_VerticalAlignment"]);
            part_ToggleButtonContentPresenter.Margin.Should().Be(expectedProperties["ToggleButtonContentPresenter_Margin"]);
        }
        private static void VerifyToggleButtonChevronGridProperties(Grid? part_ChevronGrid, ResourceDictionary expectedProperties)
        {
            part_ChevronGrid.Should().NotBeNull();
            BrushComparer.Equal(part_ChevronGrid.Background, (Brush)expectedProperties["ChevronGrid_Background"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ChevronGrid.Background, (Brush)expectedProperties["ChevronGrid_Background"]))
            {
                Console.WriteLine("part_ChevronGrid.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_ChevronGrid.Background, (Brush)expectedProperties["ChevronGrid_Background"]);
            }
            part_ChevronGrid.VerticalAlignment.Should().Be((VerticalAlignment?)expectedProperties["ChevronGrid_VerticalAlignment"]);
        }
        private static void VerifyToggleButtonTextblockProperties(TextBlock? part_ControlChevronIcon, ResourceDictionary expectedProperties)
        {
            part_ControlChevronIcon.Should().NotBeNull();
            BrushComparer.Equal(part_ControlChevronIcon.Foreground, (Brush)expectedProperties["Textblock_Foreground"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ControlChevronIcon.Foreground, (Brush)expectedProperties["Textblock_Foreground"]))
            {
                Console.WriteLine("part_ControlChevronIcon.Foreground does not match expected value");
                BrushComparer.LogBrushDifference(part_ControlChevronIcon.Foreground, (Brush)expectedProperties["Textblock_Foreground"]);
            }
            part_ControlChevronIcon.FontSize.Should().Be((Double)expectedProperties["Textblock_FontSize"]);
            part_ControlChevronIcon.FontFamily.Should().Be(expectedProperties["Textblock_FontFamily"]);
            part_ControlChevronIcon.Text.Should().Be((String)expectedProperties["Textblock_Text"]);
        }
        private static void VerifyContentPresenterBorderProperties(Border? part_contentPresenterBorder, ResourceDictionary expectedProperties)
        {
            part_contentPresenterBorder.Should().NotBeNull();
            BrushComparer.Equal(part_contentPresenterBorder.Background, (Brush)expectedProperties["ContentPresenterBorder_Background"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_contentPresenterBorder.Background, (Brush)expectedProperties["ContentPresenterBorder_Background"]))
            {
                Console.WriteLine("part_contentPresenterBorder.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_contentPresenterBorder.Background, (Brush)expectedProperties["ContentPresenterBorder_Background"]);
            }
            BrushComparer.Equal(part_contentPresenterBorder.BorderBrush, (Brush)expectedProperties["ContentPresenterBorder_BorderBrush"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_contentPresenterBorder.BorderBrush, (Brush)expectedProperties["ContentPresenterBorder_BorderBrush"]))
            {
                Console.WriteLine("part_contentPresenterBorder.BorderBrush does not match expected value");
                BrushComparer.LogBrushDifference(part_contentPresenterBorder.BorderBrush, (Brush)expectedProperties["ContentPresenterBorder_BorderBrush"]);
            }
            part_contentPresenterBorder.BorderThickness.Should().Be((Thickness)expectedProperties["ContentPresenterBorder_BorderThickness"]);
            part_contentPresenterBorder.CornerRadius.Should().Be((CornerRadius)expectedProperties["ContentPresenterBorder_CornerRadius"]);
            part_contentPresenterBorder.Visibility.Should().Be((Visibility)expectedProperties["ContentPresenterBorder_Visibility"]);
        }
        private static void VerifyContentPresenterProperties(ContentPresenter? part_contentPresenter, ResourceDictionary expectedProperties)
        {
            part_contentPresenter.Should().NotBeNull();
            part_contentPresenter.HorizontalAlignment.Should().Be((HorizontalAlignment?)expectedProperties["ContentPresenter_HorizontalAlignment"]);
            part_contentPresenter.VerticalAlignment.Should().Be((VerticalAlignment?)expectedProperties["ContentPresenter_VerticalAlignment"]);
            part_contentPresenter.Margin.Should().Be(expectedProperties["ContentPresenter_Margin"]);
        }

        private void SetSolidColorBrushProperties()
        {
            TestExpander.Background= new SolidColorBrush((Color)ColorConverter.ConvertFromString("Aqua"));
            TestExpander.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Maroon"));
            TestExpander.BorderBrush= new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
        }
        private void SetCustomizedProperties()
        {
            TestExpander.BorderThickness= new Thickness(10);
            TestExpander.Padding= new Thickness(12, 0, 12, 0);
            TestExpander.HorizontalContentAlignment=HorizontalAlignment.Center;
            TestExpander.VerticalContentAlignment=VerticalAlignment.Bottom;
            TestExpander.HorizontalAlignment=HorizontalAlignment.Center;
            TestExpander.VerticalAlignment=VerticalAlignment.Bottom;
            TestExpander.FontWeight = FontWeights.Bold;
        }
        #endregion
            #region Private Properties
        private Expander TestExpander { get; set; }
        protected override string TestDataDictionaryPath => @"/Fluent.UITests;component/ControlTests/Data/ExpanderTests.xaml";
        #endregion
    }
}
