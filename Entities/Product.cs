using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Avalonia.Media.Imaging;
using System.IO;
using System.Reflection;

namespace EducationDE.Entities;

[Table("product")]
    public partial class Product
{
    [Key]
    [Column("productarticul")]
    [StringLength(10)]
    public string Productarticul { get; set; } = null!;

    [Column("productname")]
    [StringLength(50)]
    public string? Productname { get; set; }

    [Column("productunit")]
    [StringLength(10)]
    public string? Productunit { get; set; }

    [Column("productprice")]
    [Precision(10, 2)]
    public decimal? Productprice { get; set; }

    [Column("supplier")]
    public int? Supplier { get; set; }

    [Column("manufacturer")]
    public int? Manufacturer { get; set; }

    [Column("category")]
    public int? Category { get; set; }

    [Column("discount")]
    public int? Discount { get; set; }

    [Column("countinstock")]
    public int? Countinstock { get; set; }

    [Column("description")]
    [StringLength(200)]
    public string? Description { get; set; }

    [Column("photopath")]
    [StringLength(200)]
    public string? Photopath { get; set; }

    [ForeignKey("Category")]
    [InverseProperty("Products")]
    public virtual Category? CategoryNavigation { get; set; }

    [ForeignKey("Manufacturer")]
    [InverseProperty("Products")]
    public virtual Manufacturer? ManufacturerNavigation { get; set; }

    [InverseProperty("ProductNavigation")]
    public virtual ICollection<Orderproduct> Orderproducts { get; set; } = new List<Orderproduct>();

    [ForeignKey("Supplier")]
    [InverseProperty("Products")]
    public virtual Supplier? SupplierNavigation { get; set; }


    /// <summary>
    /// Безопасное вычисляемое свойство для отображения изображения товара.
    /// Не падает, если папка/файл отсутствуют.
    /// </summary>
    public Bitmap? ImagePath
    {
        get
        {
            try
            {
                // Базовая папка сборки (bin/Debug/netX)
                var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory;
                var imagesDir = Path.Combine(baseDir, "Images");

                // Имя файла: только имя файла из Photopath (без подпапок), иначе picture.png
                var fileName = string.IsNullOrWhiteSpace(Photopath)
                    ? "picture.png"
                    : Path.GetFileName(Photopath.Trim());
                var fullPath = Path.Combine(imagesDir, fileName);

                // Если файла нет — ничего не рисуем
                if (!File.Exists(fullPath))
                {
                    // Можно попробовать Photopath как относительный путь без папки Images
                    if (!string.IsNullOrWhiteSpace(Photopath))
                    {
                        var altPath = Path.Combine(baseDir, Photopath);
                        if (File.Exists(altPath))
                            return new Bitmap(altPath);
                    }

                    return null;
                }

                return new Bitmap(fullPath);
            }
            catch
            {
                // Любые ошибки при загрузке изображения не должны ломать приложение
                return null;
            }
        }
    }
}
