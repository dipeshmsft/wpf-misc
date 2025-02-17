using Fluent.UITests.TestUtilities;
using FluentAssertions.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Xunit.Abstractions;

namespace Fluent.UITests.ControlTests
{
    public class ComboBoxTests: BaseControlTests
    {
        private ITestOutputHelper _outputHelper;

        public ComboBoxTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            SetupTestComboBox();
            TestComboBox.Should().NotBeNull();
        }

        #region Default Tests

        [WpfFact]
        public void ComboBox_InitialState_Test()
        {
            TestWindow.Show();
            var selectedItem = TestComboBox.SelectedItem;
            Assert.Null(selectedItem);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void ComboBox_Initialization_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "");
            VerifyControlProperties(TestComboBox, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void ComboBox_HasItemsFalse_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            TestComboBox.Items.Clear();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "HasItemsFalse");
            VerifyControlProperties(TestComboBox, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void ComboBox_PopupAllowsTransparencyFalse_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            Popup? popup=TestComboBox.Template.FindName("PART_Popup", TestComboBox) as Popup;
            popup.Should().NotBeNull();
            popup.AllowsTransparency = false;
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "AllowsTransparencyFalse");
            VerifyControlProperties(TestComboBox, rd);
        }

        //[WpfTheory]
        //[MemberData(nameof(ColorModes_TestData))]
        //public void ComboBox_IsEditableTrue_Test(ColorMode colorMode)
        //{
        //    SetColorMode(TestWindow, colorMode);
        //    TestWindow.Show();
        //    TestComboBox.IsEditable = true;
        //    ResourceDictionary rd = GetTestDataDictionary(colorMode, "IsEditable");
        //    VerifyControlProperties(TestComboBox, rd);
        //}

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void ComboBox_Disabled_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            TestComboBox.IsEnabled = false;
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "Disabled");
            VerifyControlProperties(TestComboBox, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void ComboBoxItem_Initialization_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            TestComboBox.SelectedIndex = 1;
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "");
            VerifyControlProperties(TestComboBox, rd);
        }
        #endregion

        #region Custom Tests
        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void ComboBox_CustomSolidColorBrush_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            SetSolidColorBrushProperties();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "CustomSolidBrush");
            VerifyControlProperties(TestComboBox, rd);
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void ComboBox_Custom_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();
            SetCustomizedProperties();
            ResourceDictionary rd = GetTestDataDictionary(colorMode, "Custom");
            VerifyControlProperties(TestComboBox, rd);
        }
        #endregion

        #region Override Methods

        public override List<FrameworkElement> GetStyleParts(Control element)
        {
            List<FrameworkElement> templateParts = [element];

            Border? contentBorder = element.Template.FindName("ContentBorder", element) as Border;
            contentBorder.Should().NotBeNull();

            templateParts.Add(contentBorder);

            TextBlock? chevronIcon = element.Template.FindName("ChevronIcon", element) as TextBlock;
            chevronIcon.Should().NotBeNull();

            templateParts.Add(chevronIcon);

            ToggleButton? toggleButton = element.Template.FindName("ToggleButton", element) as ToggleButton;
            toggleButton.Should().NotBeNull();

            templateParts.Add(toggleButton);

            TextBox? editableTextBox = element.Template.FindName("PART_EditableTextBox", element) as TextBox;
            editableTextBox.Should().NotBeNull();

            templateParts.Add(editableTextBox);

            Popup? popup = element.Template.FindName("PART_Popup", element) as Popup;
            popup.Should().NotBeNull();

            templateParts.Add(popup);

            Border? dropDownBorder = element.Template.FindName("DropDownBorder", element) as Border;
            dropDownBorder.Should().NotBeNull();

            templateParts.Add(dropDownBorder);

            Grid? borderGrid = dropDownBorder.Child as Grid;
            borderGrid.Should().NotBeNull();
            UIElementCollection gridChildren = borderGrid.Children;
            ScrollViewer? scrollViewer = gridChildren.OfType<ScrollViewer>().FirstOrDefault();
            scrollViewer.Should().NotBeNull();
            templateParts.Add(scrollViewer);

            Border? accentBorder = element.Template.FindName("AccentBorder", element) as Border;
            accentBorder.Should().NotBeNull();

            templateParts.Add(accentBorder);

            ContentPresenter? contentPresenter = element.Template.FindName("PART_ContentPresenter", element) as ContentPresenter;
            contentPresenter.Should().NotBeNull();

            templateParts.Add(contentPresenter);

            return templateParts;
        }
        public override void VerifyControlProperties(FrameworkElement element, ResourceDictionary expectedProperties)
        {
            if (element is not ComboBox comboBox) return;

            List<FrameworkElement> parts = GetStyleParts(comboBox);

            ComboBox part_ComboBox = (ComboBox)parts[0];
            Border? part_ContentBorder = parts[1] as Border;
            TextBlock? part_Textblock = parts[2] as TextBlock;
            ToggleButton? part_ToggleButton = parts[3] as ToggleButton;
            TextBox? part_EditableTextBox = parts[4] as TextBox;
            Popup? part_Popup = parts[5] as Popup;
            Border? part_DropDownBorder = parts[6] as Border;
            ScrollViewer? part_scrollViewer = parts[7] as ScrollViewer;
            Border? part_AccentBorder = parts[8] as Border;
            ContentPresenter? part_ContentPresenter = parts[9] as ContentPresenter;

            using (new AssertionScope())
            {
                //validate ComboBox properties
                VerifyComboBoxProperties(part_ComboBox, expectedProperties);
                //validate ContentBorder properties
                VerifyContentBorderProperties(part_ContentBorder, expectedProperties);
                //validate Textblock properties
                VerifyTextBlockProperties(part_Textblock, expectedProperties);
                //validate ToggleButton properties
                VerifyToggleButtonProperties(part_ToggleButton, expectedProperties);
                //validate Textbox properties
                VerifyTextBoxProperties(part_EditableTextBox, expectedProperties);
                //validate popup properties
                VerifyPopupProperties(part_Popup, expectedProperties);
                //validate DropDownBorder properties
                VerifyDropDownBorderProperties(part_DropDownBorder, expectedProperties);
                //validate ScrollViewer properties
                VerifyScrollViewerProperties(part_scrollViewer, expectedProperties);
                //validate AccentBorder properties
                VerifyAccentBorderProperties(part_AccentBorder, expectedProperties);
                //validate ContentBorder properties
                VerifyContentPresenterProperties(part_ContentPresenter, expectedProperties);
                //validate ComboBoxItem properties
                //if (part_ComboBox.SelectedItem!=null)
                //{
                //    ComboBoxItem? part_ComboBoxItem = part_ComboBox.SelectedItem as ComboBoxItem;
                //    VerifyComboBoxItemProperties(part_ComboBoxItem, expectedProperties);
                //    Control comboBoxItem = part_ComboBox;
                //    comboBoxItem.Should().NotBeNull();
                //    Border? ComboBoxItem_contentBorder = comboBoxItem.Template.FindName("ContentBorder", comboBoxItem) as Border;
                //    VerifyComboBoxItemContentBorderProperties(ComboBoxItem_contentBorder, expectedProperties);
                //    ContentPresenter? ComboBoxItem_contentPresenter = comboBoxItem.Template.FindName("PART_ContentPresenter", element) as ContentPresenter;
                //    ComboBoxItem_contentPresenter.Should().NotBeNull();
                //    VerifyComboBoxItemContentPresenterProperties(ComboBoxItem_contentPresenter, expectedProperties);
                //}                
            }
        }
        #endregion

        #region Private Methods
        private void SetupTestComboBox()
        {
            TestComboBox = new ComboBox();         
            TestComboBox.Items.Add(new ComboBoxItem { Content = "Item 1" });
            TestComboBox.Items.Add(new ComboBoxItem { Content = "Item 2" });
            TestComboBox.Items.Add(new ComboBoxItem { Content = "Item 3" });
            AddControlToView(TestWindow, TestComboBox);  
        }

        private static void VerifyComboBoxProperties(ComboBox? part_ComboBox, ResourceDictionary expectedProperties)
        {
            part_ComboBox.Should().NotBeNull();
            BrushComparer.Equal(part_ComboBox.Background, (Brush)expectedProperties["ComboBox_Background"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ComboBox.Background, (Brush)expectedProperties["ComboBox_Background"]))
            {
                Console.WriteLine("part_ComboBox.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_ComboBox.Background, (Brush)expectedProperties["ComboBox_Background"]);
            }
            BrushComparer.Equal(part_ComboBox.Foreground, (Brush)expectedProperties["ComboBox_Foreground"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ComboBox.Foreground, (Brush)expectedProperties["ComboBox_Foreground"]))
            {
                Console.WriteLine("part_ComboBox.Foreground does not match expected value");
                BrushComparer.LogBrushDifference(part_ComboBox.Foreground, (Brush)expectedProperties["ComboBox_Foreground"]);
            }
            //BrushComparer.Equal(part_ComboBox.BorderBrush, (Brush)expectedProperties["ComboBox_BorderBrush"]).Should().BeTrue();
            //if (!BrushComparer.Equal(part_ComboBox.BorderBrush, (Brush)expectedProperties["ComboBox_BorderBrush"]))
            //{
            //    Console.WriteLine("part_ComboBox.BorderBrush does not match expected value");
            //    BrushComparer.LogBrushDifference(part_ComboBox.BorderBrush, (Brush)expectedProperties["ComboBox_BorderBrush"]);
            //}
            part_ComboBox.Padding.Should().Be(expectedProperties["ComboBox_Padding"]);
            part_ComboBox.BorderThickness.Should().Be(expectedProperties["ComboBox_BorderThickness"]);
            part_ComboBox.MinWidth.Should().Be((Double)expectedProperties["ComboBox_MinWidth"]);
            //part_ComboBox.MinHeight.Should().Be((Double)expectedProperties["ComboBox_MinHeight"]);
            part_ComboBox.HorizontalAlignment.Should().Be((HorizontalAlignment?)expectedProperties["ComboBox_HorizontalAlignment"]);
            part_ComboBox.VerticalAlignment.Should().Be((VerticalAlignment?)expectedProperties["ComboBox_VerticalAlignment"]);
            part_ComboBox.HorizontalContentAlignment.Should().Be((HorizontalAlignment?)expectedProperties["ComboBox_HorizontalContentAlignment"]);
            part_ComboBox.VerticalContentAlignment.Should().Be((VerticalAlignment?)expectedProperties["ComboBox_VerticalContentAlignment"]);
         
        }
        private static void VerifyContentBorderProperties(Border? part_ContentBorder, ResourceDictionary expectedProperties)
        {
            part_ContentBorder.Should().NotBeNull();         
            part_ContentBorder.BorderThickness.Should().Be(expectedProperties["ContentBorder_BorderThickness"]);
            part_ContentBorder.CornerRadius.Should().Be(expectedProperties["ContentBorder__CornerRadius"]);           
        }
        private static void VerifyTextBlockProperties(TextBlock? part_Textblock, ResourceDictionary expectedProperties)
        {
            part_Textblock.Should().NotBeNull();
            BrushComparer.Equal(part_Textblock.Foreground, (Brush)expectedProperties["Textblock_Foreground"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_Textblock.Foreground, (Brush)expectedProperties["Textblock_Foreground"]))
            {
                Console.WriteLine("part_Textblock.Foreground does not match expected value");
                BrushComparer.LogBrushDifference(part_Textblock.Foreground, (Brush)expectedProperties["Textblock_Foreground"]);
            }
        }
        private static void VerifyToggleButtonProperties(ToggleButton? part_ToggleButton, ResourceDictionary expectedProperties)
        {
            part_ToggleButton.Should().NotBeNull();
            BrushComparer.Equal(part_ToggleButton.Background, (Brush)expectedProperties["ToggleButton_Background"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ToggleButton.Background, (Brush)expectedProperties["ToggleButton_Background"]))
            {
                Console.WriteLine("part_ToggleButton.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_ToggleButton.Background, (Brush)expectedProperties["ToggleButton_Background"]);
            }
            BrushComparer.Equal(part_ToggleButton.Foreground, (Brush)expectedProperties["ToggleButton_Foreground"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ToggleButton.Foreground, (Brush)expectedProperties["ToggleButton_Foreground"]))
            {
                Console.WriteLine("part_ToggleButton.Foreground does not match expected value");
                BrushComparer.LogBrushDifference(part_ToggleButton.Foreground, (Brush)expectedProperties["ToggleButton_Foreground"]);
            }
            BrushComparer.Equal(part_ToggleButton.BorderBrush, (Brush)expectedProperties["ToggleButton_BorderBrush"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ToggleButton.BorderBrush, (Brush)expectedProperties["ToggleButton_BorderBrush"]))
            {
                Console.WriteLine("part_ToggleButton.BorderBrush does not match expected value");
                BrushComparer.LogBrushDifference(part_ToggleButton.BorderBrush, (Brush)expectedProperties["ToggleButton_BorderBrush"]);
            }
            part_ToggleButton.HorizontalAlignment.Should().Be((HorizontalAlignment?)expectedProperties["ToggleButton_HorizontalAlignment"]);
            part_ToggleButton.VerticalAlignment.Should().Be((VerticalAlignment?)expectedProperties["ToggleButton_VerticalAlignment"]);
            part_ToggleButton.BorderThickness.Should().Be(expectedProperties["ToggleButton_BorderThickness"]);

            Border? part_ToggleButtonBorder = part_ToggleButton.Template.FindName("ContentBorder", part_ToggleButton) as Border;
            part_ToggleButtonBorder.Should().NotBeNull();
            part_ToggleButtonBorder.CornerRadius.Should().Be(expectedProperties["ToggleButton_ContentBorder_CornerRadius"]);
            part_ToggleButtonBorder.BorderThickness.Should().Be(expectedProperties["ToggleButton_ContentBorder_BorderThickness"]);
            BrushComparer.Equal(part_ToggleButtonBorder.Background, (Brush)expectedProperties["ToggleButton_ContentBorder__Background"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ToggleButtonBorder.Background, (Brush)expectedProperties["ToggleButton_ContentBorder__Background"]))
            {
                Console.WriteLine("part_ToggleButtonBorder.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_ToggleButtonBorder.Background, (Brush)expectedProperties["ToggleButton_ContentBorder__Background"]);
            }
            BrushComparer.Equal(part_ToggleButtonBorder.BorderBrush, (Brush)expectedProperties["ToggleButton_ContentBorder__BorderBrush"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ToggleButtonBorder.BorderBrush, (Brush)expectedProperties["ToggleButton_ContentBorder__BorderBrush"]))
            {
                Console.WriteLine("part_ToggleButtonBorder.BorderBrush does not match expected value");
                BrushComparer.LogBrushDifference(part_ToggleButtonBorder.BorderBrush, (Brush)expectedProperties["ToggleButton_ContentBorder__BorderBrush"]);
            }

            ContentPresenter? part_ToggleButtonContentPresenter = part_ToggleButton.Template.FindName("PART_ContentHost", part_ToggleButton) as ContentPresenter;
            part_ToggleButtonContentPresenter.Should().NotBeNull();
        }
        private static void VerifyTextBoxProperties(TextBox? part_EditableTextBox, ResourceDictionary expectedProperties)
        {
            part_EditableTextBox.Should().NotBeNull();
            BrushComparer.Equal(part_EditableTextBox.Foreground, (Brush)expectedProperties["Textbox_Foreground"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_EditableTextBox.Foreground, (Brush)expectedProperties["Textbox_Foreground"]))
            {
                Console.WriteLine("part_EditableTextBox.Foreground does not match expected value");
                BrushComparer.LogBrushDifference(part_EditableTextBox.Foreground, (Brush)expectedProperties["Textbox_Foreground"]);
            }
            if (!BrushComparer.Equal(part_EditableTextBox.Background, (Brush)expectedProperties["Textbox_Background"]))
            {
                Console.WriteLine("part_EditableTextBox.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_EditableTextBox.Background, (Brush)expectedProperties["Textbox_Background"]);
            }
            if (!BrushComparer.Equal(part_EditableTextBox.CaretBrush, (Brush)expectedProperties["Textbox_CaretBrush"]))
            {
                Console.WriteLine("part_EditableTextBox.CaretBrush does not match expected value");
                BrushComparer.LogBrushDifference(part_EditableTextBox.CaretBrush, (Brush)expectedProperties["Textbox_CaretBrush"]);
            }
            part_EditableTextBox.HorizontalAlignment.Should().Be((HorizontalAlignment?)expectedProperties["Textbox_HorizontalAlignment"]);
            part_EditableTextBox.VerticalAlignment.Should().Be((VerticalAlignment?)expectedProperties["Textbox_VerticalAlignment"]);
            part_EditableTextBox.Padding.Should().Be(expectedProperties["Textbox_Padding"]);
            part_EditableTextBox.Margin.Should().Be(expectedProperties["Textbox_Margin"]);
            part_EditableTextBox.Visibility.Should().Be((Visibility)expectedProperties["Textbox_Visibility"]);
            //part_EditableTextBox.FontSize.Should().Be((Double)expectedProperties["Textbox_FontSize"]);
            part_EditableTextBox.Cursor.Should().Be((Cursor)expectedProperties["Textbox_Cursor"]);
          
        }
        private static void VerifyPopupProperties(Popup? part_Popup, ResourceDictionary expectedProperties)
        {
            part_Popup.Should().NotBeNull();
            part_Popup.VerticalAlignment.Should().Be((VerticalAlignment?)expectedProperties["Popup_VerticalAlignment"]);
            //part_Popup.AllowsTransparency.Should().Be((Boolean)expectedProperties["Popup_AllowsTransparency"]);
            part_Popup.Focusable.Should().Be((Boolean)expectedProperties["Popup_Focusable"]);
            part_Popup.Placement.Should().Be((PlacementMode)expectedProperties["Popup_Placement"]);
            part_Popup.PopupAnimation.Should().Be((PopupAnimation)expectedProperties["Popup_Animation"]);
            part_Popup.VerticalOffset.Should().Be((Double)expectedProperties["Popup_VerticalOffset"]);
        }
        private static void VerifyDropDownBorderProperties(Border? part_DropDownBorder, ResourceDictionary expectedProperties)
        {
            part_DropDownBorder.Should().NotBeNull();
            BrushComparer.Equal(part_DropDownBorder.Background, (Brush)expectedProperties["DropDownBorder_Background"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_DropDownBorder.Background, (Brush)expectedProperties["DropDownBorder_Background"]))
            {
                Console.WriteLine("part_DropDownBorder.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_DropDownBorder.Background, (Brush)expectedProperties["DropDownBorder_Background"]);
            }
            BrushComparer.Equal(part_DropDownBorder.BorderBrush, (Brush)expectedProperties["DropDownBorder_BorderBrush"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_DropDownBorder.BorderBrush, (Brush)expectedProperties["DropDownBorder_BorderBrush"]))
            {
                Console.WriteLine("part_DropDownBorder.BorderBrush does not match expected value");
                BrushComparer.LogBrushDifference(part_DropDownBorder.BorderBrush, (Brush)expectedProperties["DropDownBorder_BorderBrush"]);
            }
            part_DropDownBorder.Padding.Should().Be(expectedProperties["DropDownBorder_Padding"]);
            part_DropDownBorder.BorderThickness.Should().Be(expectedProperties["DropDownBorder_BorderThickness"]);
            part_DropDownBorder.CornerRadius.Should().Be(expectedProperties["DropDownBorder_CornerRadius"]);
            part_DropDownBorder.MinHeight.Should().Be((Double)expectedProperties["DropDownBorder_Height"]);
        }
        private static void VerifyScrollViewerProperties(ScrollViewer? part_scrollViewer, ResourceDictionary expectedProperties)
        {
            part_scrollViewer.Should().NotBeNull();
            BrushComparer.Equal(part_scrollViewer.Foreground, (Brush)expectedProperties["ScrollViewer_Foreground"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_scrollViewer.Foreground, (Brush)expectedProperties["ScrollViewer_Foreground"]))
            {
                Console.WriteLine("part_scrollViewer.Foreground does not match expected value");
                BrushComparer.LogBrushDifference(part_scrollViewer.Foreground, (Brush)expectedProperties["ScrollViewer_Foreground"]);
            }
            part_scrollViewer.HorizontalScrollBarVisibility.Should().Be((ScrollBarVisibility)expectedProperties["ScrollViewer_HorizontalScrollBarVisibility"]);
            part_scrollViewer.VerticalScrollBarVisibility.Should().Be((ScrollBarVisibility)expectedProperties["ScrollViewer_VerticalScrollBarVisibility"]);
            part_scrollViewer.IsDeferredScrollingEnabled.Should().Be((Boolean)expectedProperties["ScrollViewer_IsDeferredScrollingEnabled"]);
        }
        private static void VerifyAccentBorderProperties(Border? part_AccentBorder, ResourceDictionary expectedProperties)
        {
            part_AccentBorder.Should().NotBeNull();
            part_AccentBorder.HorizontalAlignment.Should().Be((HorizontalAlignment?)expectedProperties["AccentBorder_HorizontalAlignment"]);
            part_AccentBorder.VerticalAlignment.Should().Be((VerticalAlignment?)expectedProperties["AccentBorder_VerticalAlignment"]);
            part_AccentBorder.CornerRadius.Should().Be(expectedProperties["AccentBorder_CornerRadius"]);
            part_AccentBorder.Visibility.Should().Be((Visibility)expectedProperties["AccentBorder_Visibility"]);
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
            part_ContentPresenter.HorizontalAlignment.Should().Be((HorizontalAlignment?)expectedProperties["ContentPresenter_HorizontalAlignment"]);
            part_ContentPresenter.VerticalAlignment.Should().Be((VerticalAlignment?)expectedProperties["ContentPresenter_VerticalAlignment"]);
                  }
        private void SetSolidColorBrushProperties()
        {
            TestComboBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Aqua"));
            TestComboBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Maroon"));
            // TestComboBox.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
        }
        private void SetCustomizedProperties()
        {
            TestComboBox.BorderThickness = new Thickness(10);
            TestComboBox.Padding = new Thickness(20, 4, 10, 10);
            TestComboBox.MinWidth = 90;
            TestComboBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            TestComboBox.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            TestComboBox.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
            TestComboBox.VerticalContentAlignment = System.Windows.VerticalAlignment.Bottom;
        }
        #endregion

        #region Private Properties

        private ComboBox TestComboBox { get; set; }
        //private ComboBox TestEmptyComboBox { get; set; }

        protected override string TestDataDictionaryPath => @"/Fluent.UITests;component/ControlTests/Data/ComboBoxTests.xaml";
        #endregion
    }
}
