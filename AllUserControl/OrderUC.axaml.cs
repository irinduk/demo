using Avalonia.Controls;
using Avalonia.Interactivity;
using EducationDE.Entities;
using EducationDE.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using EducationDE.AllWindows;

namespace EducationDE.AllUserControl;

public partial class OrderUC : UserControl
{
    public List<Order> OrderList { get; set; } = new();

    public OrderUC()
    {
        InitializeComponent();

        LoadOrders();

        DataContext = this;

        if (MainWindow.MainWindowInstance != null)
        {
            var titleBlock = MainWindow.MainWindowInstance.FindControl<TextBlock>("TitleTextBlock");
            if (titleBlock != null)
                titleBlock.Text = "Управление заказами";
        }

        App.PrewiewUC = this;
    }

    private void LoadOrders()
    {
        if (Context.Connect?.Orders == null) return;

        OrderList = Context.Connect.Orders
            .Include(o => o.OrderstatusNavigation)
            .Include(o => o.OrderaddressNavigation)
            .Include(o => o.OrderuserNavigation)
            .OrderByDescending(o => o.Orderdate)
            .ToList();
    }

    private void AddButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigate(new EditAddOrderUC());
    }

    private void EditButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var listBox = this.FindControl<ListBox>("OrderListBox");

        if (listBox?.SelectedItem is Order selected)
            Navigate(new EditAddOrderUC(selected));
    }

    private void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var listBox = this.FindControl<ListBox>("OrderListBox");
        if (listBox?.SelectedItem is Order selected)
        {
            Context.Connect.Orders.Remove(selected);
            Context.Connect.SaveChanges();
            LoadOrders();
            var lb = this.FindControl<ListBox>("OrderListBox");
            if (lb != null) lb.ItemsSource = OrderList;
        }
    }

    private void Navigate(UserControl control)
    {
        var mainWindow = MainWindow.MainWindowInstance;

        if (mainWindow != null)
        {
            var content = mainWindow
                .FindControl<ContentControl>("MainContentControl");

            if (content != null)
                content.Content = control;
        }
    }

    private void OrderListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
    }
}
