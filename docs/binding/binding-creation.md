## What happens when we create a binding ? 

### General Outline of how  DataBinding is used with a control / property 
1. We create a description of a Binding, this is like a method signature / definition of a binding ( similar to how delegates are defined ).
   1. This is done be doing `Binding bd = new Binding(...)`.
2. Once, binding definition is done, we attach the binding to a control's DependencyProperty using a call to `SetBinding` method.
   1. In this step, using the definition of a binding, an instance is created. This is an object of type `BindingExpresseionBase`.
   2. Once we have the instance of binding ( i.e. `BindingExpression` ), we set this as the value of the DependencyProperty.
      1. This is done by calling a method CreateBindingExpression on the definition we created above. Like this : `bd.CreateBindingExpression(target /*control*/, dp /*property*/)`.
      2. The next major step is attaching the binding to target, source and a DataBindEngine. While the Attach is happening, and all the necessary fields are set, we get to activation of the binding, using the `BindingExpression.Activate` method.
      3. While activation, we create a BindingWorker, which stores another worker PropertyPathWorker. The usage of these come in the next section.
      4. Once attach is done, we evaluate the expression and update the value of the DP.


### In this we will look at the steps that take place when we create and set a binding ?

```
PropertyPathWorker = PPW

Binding binding = new Binding(path) { ... };

control.SetBinding(dp, binding)            
  BindingOperations.SetBinding(target, dp, binding)
    binding.CreateBindingExpression(target, dp)
      BindingExpression.CreateBindingExpression(target, dp, this, owner) - static
        bindExpr = new BindingExpression()

    target.SetValue(dp, bindExpr)
      target.SetValueCommon(...)
        target.SetEffectiveValue()
          entry = new EffectiveEntryValue()
          target.InsertEntry(index, entry);
          entry.ResetValue(value);
          _effectiveValues[index] = entry;
        
        target.SetExpressionValue()     // to default before attaching the binding.

        bindExpr.OnAttach()
          bindExpr.Attach() 
            bindExpr.AttachOverride()                      // attaches the expression to source and target
              BindingExpressionBase.AttachOverride()       // attaches the target, property and engine
                bindExpr._engine = CurrentDataBindEngine
                bindExpr._target = ...
                bindExpr._source = ...
                AttachToContext()                          // attach to context in the tree if any 
                                                           //     ( guessing relative source, template parent)
                  SetStatus(Inactive)
                  Activate(source)
                    bindExpr._dataItem = WeakReference(source)
                    CreateWorker()
                      _worker = new ClrBindingWorker(this, Engine)
                    SetStatus(Active)
                    _worker.AttachDataItem()
                      PPW.AttachToRootItem(source)
                      _rootItem = BindingExpression.CreateReference(source)
                      PPW.UpdateSourceValueState()

        entryIndex = CheckEntryIndex()                      // refers to the index of the value in 
                                                            //    the _effectiveValues ( store for all the DP values )
        newEntry = target.EvaluateExpression(entryIndex, dp, newExpr, mt, oldEntry)  
                                                            // type : EffectiveValueEntry
        target.UpdateEffectiveValue(index, dp, mt, old, newEntry)
          target.SetEffectiveValue()
            _effectiveValues[index] = newEntry
           
```


### What happens when we change the value in the target ( for TwoWay binding ) ?

For this, I changed the text for a TextBox ( by pressing Backspace ). I am removing the extra KeyDown event call stack from here.

```
    UIElement.RaiseEvent
    ...
    UIElement.OnKeyDownThunk
    ...
    CommandManager.Execute
    ...
    TextEditorTyping.OnBackspace
    ...
    TextBox.OnTextContainerChanged ( TextBox ~ tb )
    ...


    tb.SetValueCommon(dp, value = { object(string("Sourc")) }, mt, ...)
      newExpr = value as Expression ( == null )
      oldEntry = GetValueEntry()
      currExpr = oldEntry.LocalValue as Expression
      currExpr.SetValue(target=this, dp, value)
        currExpr.Value.Set(value)
          ChangeValue(value,)
          base.Dirty()
            base.ProcessDirty()
              base.Update()
                UpdateOverride()
                  base.UpdateValue()
                    
                    UpdateSource(value)
                      BeginSourceUpdate()
                      BindingWorker.UpdateValue(value)
                        PPW.SetValue(source, value)         // uses reflection to get Setter for property
                          appCode.Prop.Setter
                            viewModel.OnPropertyChanged()
                              ...
                              PPW.OnPropertyChanged()
                                BindingWorker.OnPropertyChanged()  
                                                            // the request is ignored here as the value is coming
                                                            // from the target itself.
                      EndSourceUpdate()
                      OnSourceUpdated()

                    CommitSource(value)
                    NotifyCommitManager()

      newEntry = _effectiveValues[index]
      UpdateEffectiveValue()
```


### What happens when we change the value in the source ( for TwoWay binding ) ?

To investigate this, what I did was that I updated the source using a button click. Here is the stack that goes on from the property's setter at the beginning.

```
    prop.Set()
      viewModel.OnPropertyChanged()
        ...
        PPW.OnPropertyChanged()
          BindingWorker.OnPropertyChanged()
            PPW.OnPropertyChangedAtLevel(level : int = 0)
              PPW.UpdateSourceValueState()
                target = BindingWorker.CheckTarget()
                // figure out if we want to transfer this value to target or not
                _host.NewValueAvailable()
                  BindingExpression.ScheduleTransfer()
                    TransferValue( unsetValue )             // we pass unsetValue here, if we pass this value the function
                                                            // get's the value from Worker itself.
                      Worker.RawValue()
                        PPW.RawValue()
                          PPW.GetValue()                    // Uses reflection to get the value

                      ChangeValue()
                      Invalidate()                          // invalidates the target property. Causing it to reevaluate it's value
                                                            // 
                                                            // From here, property engine takes care of resolving the value 
                                                            // from the expression and other stuff
```
