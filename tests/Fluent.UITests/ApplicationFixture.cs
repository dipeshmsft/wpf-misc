using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Fluent.UITests;


public class ApplicationFixture : IDisposable
{
    public ApplicationFixture()
    {
        var waitForApplicationRun = new TaskCompletionSource<bool>();
        Thread thread = new Thread(() =>
        {
            Application application = new Application();
            application.Startup += (s, e) => { waitForApplicationRun.SetResult(true); };
            application.Run();
            //Dispatcher.Run();
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        waitForApplicationRun.Task.Wait();
        App = Application.Current;
    }

    public void ResetApplicationInstance()
    {
        Execute(() =>
        {
            App.ThemeMode = ThemeMode.None;
            App.MainWindow = null;
            if (App.Resources.Count > 0
                || App.Resources.MergedDictionaries.Count > 0)
            {
                App.Resources.Clear();
            }
        });
    }

    public void Execute(Action action)
    {
        Execute(Application.Current.Dispatcher, action);
    }

    private void Execute(Dispatcher dispatcher, Action action)
    {
        Exception exception = null;
        if (dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            var workComplete = new AutoResetEvent(false);
            dispatcher.InvokeAsync(() =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    exception = e;
                    throw;
                }
                finally // Unblock calling thread even if action() throws
                {
                    workComplete.Set();
                }
            });

            workComplete.WaitOne();
            if (exception != null)
            {
                throw exception;
            }
        }

    }

    private T ExecuteFunc<T>(Func<T> func)
    {
        return ExecuteFunc(Application.Current.Dispatcher, func);
    }

    private T? ExecuteFunc<T>(Dispatcher dispatcher, Func<T> func)
    {
        Exception exception = null;
        T? result = default(T);
        if (dispatcher.CheckAccess())
        {
            result = func();
        }
        else
        {
            var workComplete = new AutoResetEvent(false);
            dispatcher.InvokeAsync(() =>
            {
                try
                {
                    result = func();
                }
                catch (Exception e)
                {
                    exception = e;
                    throw;
                }
                finally // Unblock calling thread even if action() throws
                {
                    workComplete.Set();
                }
            });

            workComplete.WaitOne();
            if (exception != null)
            {
                throw exception;
            }

        }
        return result;
    }

    public Window GetMainWindow()
    {

        Window window = ExecuteFunc<Window>(() =>
        {
            return Application.Current.MainWindow;
        });
        return window;
    }

    public void Dispose()
    {
        Execute(() =>
        {
            Application.Current.Shutdown();
        });
    }

    public Application App { get; private set; }
}


[CollectionDefinition("Application Tests Collection", DisableParallelization = true)]
public class ApplicationTestsCollection : ICollectionFixture<ApplicationFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

