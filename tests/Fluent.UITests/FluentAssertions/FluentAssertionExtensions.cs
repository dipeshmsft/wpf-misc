﻿using Fluent.UITests.TestUtilities;
using FluentAssertions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Xunit.Abstractions;

namespace Fluent.UITests.FluentAssertions;

public static class FluentAssertionExtensions
{
    public static AndConstraint<ObjectAssertions> BeEquivalentToBrush(
        this ObjectAssertions assertions,
        Brush expectedBrush,
        ITestOutputHelper outputHelper,
        string because="",
        params object[] becauseArgs)
    {
        var actualBrush = assertions.Subject as Brush;
        actualBrush.Should().NotBeNull(because, becauseArgs);

        if(!BrushComparer.Equal(actualBrush, expectedBrush))
        {
            if (actualBrush is SolidColorBrush scb1 && expectedBrush is SolidColorBrush scb2)
            {
                outputHelper.WriteLine($"Brush Comparison Failed:");
                outputHelper.WriteLine($"  Actual Color: {scb1.Color}");
                outputHelper.WriteLine($"  Expected Color: {scb2.Color}");
            }
            else
            {
                outputHelper.WriteLine($"Brush Comparison Failed: Brushes are not SolidColorBrush.");
            }
        }

        BrushComparer.Equal(actualBrush, expectedBrush).Should().BeTrue(because, becauseArgs);

        return new AndConstraint<ObjectAssertions>(assertions);
    }
}
