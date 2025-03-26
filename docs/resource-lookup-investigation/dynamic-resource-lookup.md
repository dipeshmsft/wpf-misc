## What happens when we refer a resource using DynamicResource extension ?

DynamicResource extension is a way of referring resources in WPF, which reflects the changes in the resource at the runtime. i.e. during the runtime, if the value of the resource changes, the property using this resource will get the new value and update itself accordingly.

#### Here is a simple example of using DynamicResource ?

**App.xaml**
```
<Application x:Class="ResourceExtensionResolution.App">
    <Application.Resources>
        <ResourceDictionary>
            <SolidColorBrush x:Key="RedBrush" Color="Red" />
        </ResourceDictionary>
    </Application.Resources>
</Application>

```

**MainWindow.xaml**
```
<Window>
    <Grid> 
        <Border Width="100" Height="50" BorderThickness="2"
                BorderBrush="{DynamicResource RedBrush}" />
    </Grid>
</Window>
```

Now when we use the app, the window will load with a border having a red border. Now let's see how does this resolution happen.

#### Here are the steps in which resolution happens for DynamicResource

1. **Application load (Same as StaticResource)**

```
  MainWindow.ctor()
    ...
      Application.LoadComponent()
        XamlReader.LoadBaml()
          WpfXamlLoader.LoadBaml()
            WpfXamlLoader.TransformNodes()
              Baml2006Reader.Read()                 // Baml2006Reader : B6R
                B6R.Process_BamlRecords()
                  B6R.Process_OneBamlRecord()
                    B6R.Process_PropertyWithExtension()
```

Here, Process_PropertyWithExtensions matches which kind of extension is used here and depending on the value the code diverges.

2. **Creating DynamicResourceExtension**

```
[#1]
  DynamicResourceExtension(object resourceKey)
```

This resource key is used to find the resource in the dictionaries.

3. **Resolving Property Value at Load (Same as StaticResource)** 

```
  [#1 (till WpfXamlLoader.LoadBaml)]
    WpfXamlLoader.TransformNodes()
      XamlWriter.WriteNodes()
        XamlObjectWriter.WriteEndMember()         // XamlObjectWriter : XOW
          XOW.Logic_AssignProvidedValue()
            XOW.Logic_ProvideValue()
              ClrObjectRuntime.CallProvideValue()
                MarkupExtension.ProvideValue()    // Here MarkupExtension is StaticResourceExtension
                                                  // created above.
```

In here, we see that once the BAML is processed and we are writing the `object` that uses the resource ( here Border ) in memory, the flow goes as above. At the end we reach `DynamicResourceExtension`'s ProvideValue.

4. **Resolving Resource Value**

```
  [#3]
    DynamicResourceExtension.ProvideValue()        // StaticResourceExtension : SRE
      return new ResourceReferenceExpression(resourceKey)
```

Here in this case, we get the ResourceReferrenceExpression with the base type Expression ( similar to what happens in Binding ), and the expression is set as the property value. Whenever, we need to calculate the actual value of the property, expression.EvaluateExpression() is called, which fetches and caches the value in the expression.