namespace StorehouseConsoleApp.Models;

/// <summary>
/// Представляет паллету - подставку для размещения коробок на складе.
/// </summary>
public class Pallet
{
    /// <summary>
    /// Уникальный идентификатор паллеты.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Ширина паллеты (должна быть положительной).
    /// </summary>
    public double Width { get; init; }

    /// <summary>
    /// Высота паллеты (должна быть положительной).
    /// </summary>
    public double Height { get; init; }

    /// <summary>
    /// Глубина паллеты (должна быть положительной).
    /// </summary>
    public double Depth { get; init; }

    /// <summary>
    /// Собственный вес паллеты (без коробок). По умолчанию 30 кг.
    /// </summary>
    public double SelfWeight { get; init; }

    /// <summary>
    /// Список коробок, размещенных на паллете.
    /// </summary>
    public List<Box>? Boxes { get; private set; } = [];
    
    /// <summary>
    /// Общий вес паллеты с учетом всех коробок.
    /// </summary>
    public double Weight => (Boxes?.Sum(b => b.Weight) ?? 0) + SelfWeight;
    
    /// <summary>
    /// Общий объем паллеты (включая объем самих коробок и занимаемое ими место).
    /// </summary>
    public double Volume => (Boxes?.Sum(b => b.Width * b.Height * b.Depth) ?? 0) 
        + Width * Height * Depth;
    
    /// <summary>
    /// Минимальная дата истечения срока годности среди всех коробок на паллете.
    /// Если коробок нет, возвращает null.
    /// </summary>
    public DateOnly? ExpirationDate => Boxes?.Count > 0 
        ? Boxes.Min(b => b.ExpirationDate) 
        : null;

    /// <summary>
    /// Создает новую паллету с указанными параметрами.
    /// </summary>
    /// <param name="id">Идентификатор паллеты.</param>
    /// <param name="width">Ширина паллеты (должна быть > 0).</param>
    /// <param name="height">Высота паллеты (должна быть > 0).</param>
    /// <param name="depth">Глубина паллеты (должна быть > 0).</param>
    /// <param name="selfWeight">Собственный вес паллеты (по умолчанию 30 кг).</param>
    /// <exception cref="ArgumentException">Выбрасывается, если размеры или вес некорректны.</exception>
    public Pallet(int id, double width, double height, double depth, double selfWeight = 30)
    {
        if (width <= 0 || height <= 0 || depth <= 0 || selfWeight <=0)
            throw new ArgumentException("Размеры и вес паллеты должны быть положительными");

        Id = id;
        Width = width;
        Height = height;
        Depth = depth;
        SelfWeight = selfWeight;
    }

    /// <summary>
    /// Добавляет коробку на паллету.
    /// </summary>
    /// <param name="box">Коробка для добавления.</param>
    /// <exception cref="InvalidOperationException">Выбрасывается, если коробка ни в одной ориентации не помещается на паллету.</exception>
    public void AddBox(Box box)
    {
        Boxes ??= [];
    
        var fits =
            // Ставим на дно (по обычному)
            (box.Width <= Width && box.Depth <= Depth) ||
            // Переворачиваем на бок
            (box.Width <= Width && box.Height <= Depth) ||
            // Ставим торцем
            (box.Height <= Width && box.Depth <= Depth);
    
        if (!fits)
            throw new InvalidOperationException("Коробка не помещается на эту паллету.");
        
        Boxes.Add(box);
    }
}