namespace EducationDE.Entities;

/// <summary>
/// Свойства для отображения в UI (статус и адрес всегда имеют текст).
/// </summary>
public partial class Order
{
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string StatusDisplay => OrderstatusNavigation?.Statusname ?? "—";

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string AddressDisplay => OrderaddressNavigation?.Addressname ?? "—";
}
