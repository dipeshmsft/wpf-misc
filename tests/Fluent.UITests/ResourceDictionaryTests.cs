using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace Fluent.UITests;
public class ResourceDictionaryTests
{
    [WpfTheory]
    [MemberData(nameof(ThemeDictionarySourceList))]
    public void Fluent_ResourceDictionary_LoadTests(string source)
    {
        LoadFluentResourceDictionary(source);
    }

    [WpfTheory]
    [MemberData(nameof(GetColorDictionary_MatchKeys_TestData))]
    public void Fluent_ColorDictionary_MatchKeysTest(string firstSource, string secondSource)
    {
        ResourceDictionary dictionary1 = LoadFluentResourceDictionary(firstSource);
        ResourceDictionary dictionary2 = LoadFluentResourceDictionary(secondSource);

        int colorDictionary1KeysCount = GetResourceKeysFromResourceDictionary(dictionary1,
            out List<string> dictionary1StringKeys, out List<object> dictionary1ObjectKeys);

        int colorDictionary2KeysCount = GetResourceKeysFromResourceDictionary(dictionary2,
            out List<string> dictionary2StringKeys, out List<object> dictionary2ObjectKeys);

        List<string> dictionary1ExtraStringKeys = dictionary1StringKeys.Except(dictionary2StringKeys).ToList();
        List<string> dictionary2ExtraStringKeys = dictionary2StringKeys.Except(dictionary1StringKeys).ToList();

        List<object> dictionary1ExtraObjectKeys = dictionary1ObjectKeys.Except(dictionary2ExtraStringKeys).ToList();
        List<object> dictionary2ExtraObjectKeys = dictionary2ObjectKeys.Except(dictionary1ObjectKeys).ToList();

        Log_ExtraKeys(dictionary1ExtraStringKeys, $"Dictionary 1 : {firstSource} extra keys");
        Log_ExtraKeys(dictionary2ExtraStringKeys, $"Dictionary 2 : {secondSource} extra keys");

        using (new AssertionScope())
        {
            dictionary1ExtraStringKeys.Should().BeEmpty();
            dictionary2ExtraStringKeys.Should().BeEmpty();
            dictionary1ExtraObjectKeys.Should().BeEmpty();
            dictionary2ExtraObjectKeys.Should().BeEmpty();
        }
    }

    private void Log_ExtraKeys(List<string> dictionary1ExtraStringKeys, string v)
    {
        Console.WriteLine(v);
        if(dictionary1ExtraStringKeys.Count == 0)
        {
            Console.WriteLine("None\n");
            return;
        }

        foreach(string key in dictionary1ExtraStringKeys)
        {
            Console.WriteLine(key);
        }
        Console.WriteLine();
    }


    #region Helper Methods

    private static ResourceDictionary LoadFluentResourceDictionary(string source)
    {
        var uri = new Uri(source, UriKind.RelativeOrAbsolute);
        ResourceDictionary? resourceDictionary = Application.LoadComponent(uri) as ResourceDictionary;
        resourceDictionary.Should().NotBeNull();
        return resourceDictionary;
    }

    private static int GetResourceKeysFromResourceDictionary(ResourceDictionary resourceDictionary,
        out List<string> stringResourceKeys,
        out List<object> objectResourceKeys)
    {
        ArgumentNullException.ThrowIfNull(resourceDictionary, nameof(resourceDictionary));
        stringResourceKeys = new List<string>();
        objectResourceKeys = new List<object>();

        int resourceDictionaryKeysCount = resourceDictionary.Count;

        foreach (object key in resourceDictionary.Keys)
        {
            if (key is string skey)
            {
                stringResourceKeys.Add(skey);
            }
            else
            {
                objectResourceKeys.Add(key);
            }
        }

        return resourceDictionaryKeysCount;
    }

    #endregion


    #region Test Data

    public static IEnumerable<object[]> GetColorDictionary_MatchKeys_TestData()
    {
        int count = ColorDictionarySourceList.Count;
        for (int i = 0; i < count; i++)
        {
            for (int j = i + 1; j < count; j++)
            {
                yield return new object[] { ColorDictionarySourceList[i], ColorDictionarySourceList[j] };
            }
        }
    }

    public static IList<object[]> ThemeDictionarySourceList
        = new List<object[]>
            {
                new object[] { $"{ThemeDictionaryPath}/Fluent.xaml" },
                new object[] { $"{ThemeDictionaryPath}/Fluent.Light.xaml" },
                new object[] { $"{ThemeDictionaryPath}/Fluent.Dark.xaml" },
                new object[] { $"{ThemeDictionaryPath}/Fluent.HC.xaml" },
            };

    public static IList<string> ColorDictionarySourceList
        = new List<string>
        {
            $"{ColorDictionaryPath}/Light.xaml",
            $"{ColorDictionaryPath}/Dark.xaml",
            $"{ColorDictionaryPath}/HC.xaml"
        };

    private const string ThemeDictionaryPath = @"/PresentationFramework.Fluent;component/Themes";
    private const string ColorDictionaryPath = @"/PresentationFramework.Fluent;component/Resources/Theme";

    #endregion

}
