using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EducationDE.AllWindows;
using EducationDE.AllUserControl;
using EducationDE.Entities;

namespace EducationDE;

public partial class App : Application
{
    public static MainWindow? MainWindow { get; set; }
    public static UserControl? PrewiewUC { get; set; }  // ← Оставляем как есть (с опечаткой "Prewiew")
    public static User? LoginUser { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow = new MainWindow();
            desktop.MainWindow = MainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}