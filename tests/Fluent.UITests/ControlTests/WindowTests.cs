﻿using Fluent.UITests.TestUtilities;
using FluentAssertions.Execution;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Xunit.Abstractions;

namespace Fluent.UITests.ControlTests
{
    public class WindowTests :BaseControlTests
    {
        public WindowTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            SetupWindow();
        }

        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Window_Initialization_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.Show();

            ResourceDictionary rd = GetTestDataDictionary(colorMode, "");
            VerifyControlProperties(TestWindow, rd);
        }
     
        [WpfTheory]
        [MemberData(nameof(ColorModes_TestData))]
        public void Window_ResizeMode_Test(ColorMode colorMode)
        {
            SetColorMode(TestWindow, colorMode);
            TestWindow.ResizeMode = ResizeMode.CanResizeWithGrip;
            TestWindow.WindowState = WindowState.Normal;
            TestWindow.Show();

            ResourceDictionary rd = GetTestDataDictionary(colorMode, "CanResizeGrip_NormalWindow");
            VerifyControlProperties(TestWindow, rd);
        }        

        #region Override Methods

        public override List<FrameworkElement> GetStyleParts(Control element)
        {
            List<FrameworkElement> templateParts = new List<FrameworkElement>();
            templateParts.Add(element);            

            ResizeGrip? windowResizeGrip = element.Template.FindName("WindowResizeGrip", element) as ResizeGrip;
            if (windowResizeGrip != null)
            {
                windowResizeGrip.Should().NotBeNull();
                templateParts.Add(windowResizeGrip);
            }
            return templateParts;
        }

        public override void VerifyControlProperties(FrameworkElement element, ResourceDictionary expectedProperties)
        {
            Window? window = element as Window;
            if (window is null) return;

            List<FrameworkElement> parts = GetStyleParts(window);

            Window? part_Window = parts[0] as Window;
            ResizeGrip? part_ResizeGrip = null;
           
            if (parts.Count > 1) { 
            part_ResizeGrip = parts[1] as ResizeGrip;
            }
            using (new AssertionScope())
            {                
                //validate window properties
                VerifyWindowProperties(part_Window, expectedProperties);                
                if(part_ResizeGrip != null)
                {
                    //validate window resize grip properties
                    VerifyWindowResizeGripProperties(part_ResizeGrip, expectedProperties);
                }                
            }
        }

        public static void VerifyWindowProperties(Window? part_Window, ResourceDictionary expectedProperties)
        {
            part_Window.Should().NotBeNull();
            BrushComparer.Equal(part_Window.Background, (Brush)expectedProperties["Window_Background"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_Window.Background, (Brush)expectedProperties["Window_Background"]))
            {
                Console.WriteLine("part_Window.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_Window.Background, (Brush)expectedProperties["Window_Background"]);
            }

            BrushComparer.Equal(part_Window.Foreground, (Brush)expectedProperties["Window_Foreground"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_Window.Foreground, (Brush)expectedProperties["Window_Foreground"]))
            {
                Console.WriteLine("part_Window.Foreground does not match expected value");
                BrushComparer.LogBrushDifference(part_Window.Foreground, (Brush)expectedProperties["Window_Foreground"]);
            }            
        }

        public static void VerifyWindowResizeGripProperties(ResizeGrip part_ResizeGrip, ResourceDictionary expectedProperties)
        {
            part_ResizeGrip.Should().NotBeNull();
            BrushComparer.Equal(part_ResizeGrip.Background, (Brush)expectedProperties["Window_Background"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ResizeGrip.Background, (Brush)expectedProperties["Window_Background"]))
            {
                Console.WriteLine("part_Window.Background does not match expected value");
                BrushComparer.LogBrushDifference(part_ResizeGrip.Background, (Brush)expectedProperties["Window_Background"]);
            }
            BrushComparer.Equal(part_ResizeGrip.Foreground, (Brush)expectedProperties["Window_Foreground"]).Should().BeTrue();
            if (!BrushComparer.Equal(part_ResizeGrip.Foreground, (Brush)expectedProperties["Window_Foreground"]))
            {
                Console.WriteLine("part_Window.Foreground does not match expected value");
                BrushComparer.LogBrushDifference(part_ResizeGrip.Foreground, (Brush)expectedProperties["Window_Foreground"]);
            }
            part_ResizeGrip.Visibility.Should().Be((Visibility)expectedProperties["WindowResizeGrip_Visibility"]);
            part_ResizeGrip.IsTabStop.Should().Be((bool)expectedProperties["WindowResizeGrip_IsTabStop"]);
            part_ResizeGrip.HorizontalAlignment.Should().Be((HorizontalAlignment)expectedProperties["WindowResizeGrip_HorizontalAlignment"]);
            part_ResizeGrip.VerticalAlignment.Should().Be((VerticalAlignment)expectedProperties["WindowResizeGrip_VerticalAlignment"]);           
        }
        #endregion

        #region Private Methods      

        private void SetupWindow()
        {
            TestWindow = new Window() { Content = "TestWindow" };          
        }

        #endregion

        #region Private Properties                
        protected override string TestDataDictionaryPath => @"/Fluent.UITests;component/ControlTests/Data/WindowTests.xaml";

        #endregion
        private ITestOutputHelper _outputHelper;
    }
}
