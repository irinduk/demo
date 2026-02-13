using Avalonia.Controls;
using Avalonia.Interactivity;
using EducationDE.Entities;
using EducationDE.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;          // Для List<>
using System.Linq;                         // Для .ToList(), .Where()
using Avalonia.Media.Imaging;              // Для работы с изображениями
using Avalonia.Platform;
using System;
using EducationDE.AllWindows;

namespace EducationDE.AllUserControl;

public partial class EditAddProductUC : UserControl
{

    public Product _product{get; set;} = new Product();
    public List<Category> categories {get; set;}
    public List<Supplier> suppliers {get; set;}
    public List<Manufacturer> manufacturers {get; set;}




    public EditAddProductUC()
    {
        InitializeComponent();
        _product = new Product();
        categories=Context.Connect.Categories.ToList();
        suppliers=Context.Connect.Suppliers.ToList();
        manufacturers=Context.Connect.Manufacturers.ToList();
        DataContext=this;

        var titleBlock = MainWindow.MainWindowInstance?.FindControl<TextBlock>("TitleTextBlock");
        if (titleBlock != null)
            titleBlock.Text = "Добавление товара";
        
        App.PrewiewUC = App.MainWindow?.FindControl<ContentControl>("MainContentControl")?.Content as UserControl;
    }

     public EditAddProductUC(Product product)
    {
        InitializeComponent();
        _product = product;
        categories=Context.Connect.Categories.ToList();
        suppliers=Context.Connect.Suppliers.ToList();
        manufacturers=Context.Connect.Manufacturers.ToList();
        DataContext = this;

        var titleBlock = MainWindow.MainWindowInstance?.FindControl<TextBlock>("TitleTextBlock");
        if (titleBlock != null)
            titleBlock.Text = "Редактирование товара";
        
        App.PrewiewUC = App.MainWindow?.FindControl<ContentControl>("MainContentControl")?.Content as UserControl;
    }

    private void OkButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var catCombo = this.FindControl<ComboBox>("CategoryComboBox");
            var manuCombo = this.FindControl<ComboBox>("ManufacturerComboBox");
            var suppCombo = this.FindControl<ComboBox>("SupplierComboBox");

            if (catCombo?.SelectedItem is Category cat) { _product.CategoryNavigation = cat; _product.Category = cat.Categoryid; }
            if (manuCombo?.SelectedItem is Manufacturer m) { _product.ManufacturerNavigation = m; _product.Manufacturer = m.Manufacturerid; }
            if (suppCombo?.SelectedItem is Supplier s) { _product.SupplierNavigation = s; _product.Supplier = s.Supplierid; }

            if (_product.Productarticul == null)
            {
                var lastProduct = Context.Connect.Products.OrderByDescending(p => p.Productarticul).FirstOrDefault();
                if (lastProduct != null && lastProduct.Productarticul != null)
                    _product.Productarticul = (int.Parse(lastProduct.Productarticul) + 1).ToString();
                else
                    _product.Productarticul = "1";
                Context.Connect.Products.Add(_product);
            }

            Context.Connect.SaveChanges();
            
            var mainContent = MainWindow.MainWindowInstance?.FindControl<ContentControl>("MainContentControl");
            if (mainContent != null)
                mainContent.Content = new MainUC(App.LoginUser);
        }
        catch (Exception ex)
        {
            var mainText = MainWindow.MainWindowInstance?.FindControl<TextBlock>("MainTextBlock");
            if (mainText != null)
                mainText.Text = $"Ошибка сохранения: {ex.Message}";
            Console.WriteLine($"EditAddProductUC Save ERROR: {ex.Message}");
        }
    }

}