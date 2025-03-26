## What happens when we refer a resource using StaticResource extension ?

StaticResource is a way of referring to resources in WPF, which resolves the value of the resource only once at the time when the element is being loaded.

#### Here is a simple example of using StaticResource ?

**App.xaml**
```
<Application x:Class="StaticResourceResolution.App">
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
                BorderBrush="{StaticResource RedBrush}" />
    </Grid>
</Window>
```

Now when we use the app, the window will load with a border having a red border. Now let's see how does this resolution happen.

#### Here are the steps in which resolution happens for StaticResources

1. **Application load**

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

2. **Creating StaticResourceExtension**

```
[#1]

  StaticResourceExtension(object resourceKey)
```

This resource key is used to find the resource in the dictionaries.

3. **Resolving Property Value at Load**

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

In here, we see that once the BAML is processed and we are writing the object using the resource ( here Border ) in memory, the flow goes as above. At the end we reach `StaticResourceExtension`'s ProvideValue which is actually responsible for resolving the value.

4. **Resolving Resource Value**

```
  [#3]
    StaticResourceExtension.ProvideValue()        // StaticResourceExtension : SRE
      SRE.ProvideValueInternal()
        SRE.TryProvideValueInternal()
          SRE.TryProvideValueImpl()
            SRE.FindResourceInEnvironment()
              ResourceDictinary rd = FindTheResourceDictionary()
              if(rd is not null)
              {
                return rd.Lookup()
              }

              return FindResourceInAppOrSystem()
```