using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using EducationDE.Entities;
using EducationDE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using EducationDE.AllWindows;
using System;

namespace EducationDE.AllUserControl;

public partial class AuthUC : UserControl
{
    public AuthUC()
    {
        InitializeComponent();
        App.PrewiewUC = this; // ← как в методичке (с опечаткой, но консистентно)

        // Устанавливаем заголовок
        if (MainWindow.MainWindowInstance != null)
        {
            var title = MainWindow.MainWindowInstance.FindControl<TextBlock>("TitleTextBlock");
            if (title != null) title.Text = "Авторизация";
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoginButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // 🔑 КРИТИЧЕСКОЕ ИСПРАВЛЕНИЕ: "PasswordBox" вместо "PasswordTextBox"!
        var loginTextBox = this.FindControl<TextBox>("LoginTextBox");
        var passwordBox = this.FindControl<TextBox>("PasswordBox"); // ← ТОЧНОЕ ИМЯ ИЗ XAML!

        // Защита от пустого ввода
        if (loginTextBox == null || passwordBox == null || 
            string.IsNullOrWhiteSpace(loginTextBox.Text) || 
            string.IsNullOrWhiteSpace(passwordBox.Text))
        {
            ShowError("Введите логин и пароль");
            return;
        }

        try
        {
            // Проверка контекста БД
            if (Context.Connect == null)
            {
                ShowError("Ошибка: не удалось подключиться к базе данных");
                Console.WriteLine("AUTH ERROR: Context.Connect is null");
                return;
            }

            if (Context.Connect.Users == null)
            {
                ShowError("Ошибка: таблица пользователей недоступна");
                Console.WriteLine("AUTH ERROR: Context.Connect.Users is null");
                return;
            }

            // Поиск пользователя с загрузкой роли для отображения
            var user = Context.Connect.Users
                .Include(u => u.RoleNavigation)
                .FirstOrDefault(u => 
                    u.Login == loginTextBox.Text && 
                    u.Password == passwordBox.Text);

            if (user != null)
            {
                App.LoginUser = user;
                
                var mainWindow = MainWindow.MainWindowInstance;
                if (mainWindow != null)
                {
                    var mainText = mainWindow.FindControl<TextBlock>("MainTextBlock");
                    if (mainText != null)
                    {
                        var roleName = user.RoleNavigation?.Rolename ?? "Гость";
                        mainText.Text = $"{user.Lastname} {user.Firstname} {user.Middlename} ({roleName})";
                    }

                    var content = mainWindow.FindControl<ContentControl>("MainContentControl");
                    if (content != null)
                    {
                        try
                        {
                            content.Content = new MainUC(user);
                            (mainWindow as MainWindow)?.UpdateNavigationVisibility();
                        }
                        catch (Exception mainUCEx)
                        {
                            ShowError($"Ошибка создания каталога: {mainUCEx.Message}");
                            Console.WriteLine($"AUTH ERROR in MainUC constructor: {mainUCEx.Message}");
                            Console.WriteLine($"Stack trace: {mainUCEx.StackTrace}");
                        }
                    }
                    else
                    {
                        ShowError("Ошибка: не найден контейнер контента");
                        Console.WriteLine("AUTH ERROR: MainContentControl is null");
                    }
                }
                else
                {
                    ShowError("Ошибка: главное окно не найдено");
                    Console.WriteLine("AUTH ERROR: MainWindowInstance is null");
                }
            }
            else
            {
                ShowError("Неверный логин или пароль");
            }
        }
        catch (Exception ex)
        {
            // Диагностика — не даём упасть приложению
            ShowError($"Ошибка: {ex.Message}");
            Console.WriteLine($"AUTH ERROR: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private void ShowError(string message)
    {
        var mainWindow = MainWindow.MainWindowInstance;
        if (mainWindow != null)
        {
            var mainText = mainWindow.FindControl<TextBlock>("MainTextBlock");
            if (mainText != null)
                mainText.Text = $"Ошибка: {message}";
        }
    }
}