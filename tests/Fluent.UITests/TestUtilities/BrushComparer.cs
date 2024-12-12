﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Fluent.UITests.TestUtilities;

public static class BrushComparer
{
    public static bool Equal(Brush brush1, Brush brush2)
    {
        if(brush1 is null || brush2 is null)
        {
            return brush1 is null && brush2 is null;
        }

        if(brush1.GetType() != brush2.GetType())
        {
            return false;
        }

        switch(brush1)
        {
            case SolidColorBrush:
                return CompareSolidColorBrushes((SolidColorBrush)brush1, (SolidColorBrush)brush2);
            default:
                return false;
        }
    }

    public static bool CompareSolidColorBrushes(SolidColorBrush brush1, SolidColorBrush brush2)
    {
        if (brush1 is null || brush2 is null)
        {
            return brush1 is null && brush2 is null;
        }

        return brush1.Color == brush2.Color && brush1.Opacity == brush2.Opacity;
    }

    public static void LogBrushDifference(Brush brush1, Brush brush2)
    {
        if (brush1 is null || brush2 is null)
        {
            if(brush1 is null && brush2 is null)
            {

            }
            else
            {
                Console.WriteLine($"brush1 is null : {brush1 is null} , brush2 is null : {brush2 is null}");
            }
            return;
        }

        if (brush1.GetType() != brush2.GetType())
        {
            Console.WriteLine($"brush1 is of type : {brush1.GetType()} , brush2 is of type : {brush2.GetType()}");
            return;
        }

        switch (brush1)
        {
            case SolidColorBrush:
                LogSolidColorBrushDifference((SolidColorBrush)brush1, (SolidColorBrush)brush2);
                return;
            default:
                return;
        }
    }

    private static void LogSolidColorBrushDifference(SolidColorBrush brush1, SolidColorBrush brush2)
    {
        if (brush1 is null || brush2 is null)
        {
            if (brush1 is null && brush2 is null)
            {

            }
            else
            {
                Console.WriteLine($"brush1 is null : {brush1 is null} , brush2 is null : {brush2 is null}");
            }
            return;
        }

        Console.WriteLine($"brush1: Color = {brush1.Color}, Opacity = {brush1.Opacity}");
        Console.WriteLine($"brush2: Color = {brush2.Color}, Opacity = {brush2.Opacity}");
        return; 
    }
}
