namespace StorehouseConsoleApp.Models;

public class Box
{
    private const int DefaultExpirationDays = 100;
    /// <summary>
    /// Уникальный идентификатор коробки.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Ширина коробки (должна быть положительной).
    /// </summary>
    public double Width { get; init; }

    /// <summary>
    /// Высота коробки (должна быть положительной).
    /// </summary>
    public double Height { get; init; }

    /// <summary>
    /// Глубина коробки (должна быть положительной).
    /// </summary>
    public double Depth { get; init; }

    /// <summary>
    /// Вес коробки (должен быть положительным).
    /// </summary>
    public double Weight { get; init; }

    /// <summary>
    /// Объем коробки, вычисляемый как: Ширина * Глубина * Высота.
    /// </summary>
    public double Volume => Width * Height * Depth;

    /// <summary>
    /// Дата производства коробки (может быть не указана).
    /// </summary>
    public DateOnly? ProductionDate { get; private set; }

    /// <summary>
    /// Дата истечения срока годности коробки.
    /// </summary>
    public DateOnly ExpirationDate { get; private set; }
    
    /// <summary>
    /// Создает новый экземпляр коробки.
    /// </summary>
    /// <param name="id">Уникальный идентификатор.</param>
    /// <param name="width">Ширина (должна быть > 0).</param>
    /// <param name="height">Высота (должна быть > 0).</param>
    /// <param name="depth">Глубина (должна быть > 0).</param>
    /// <param name="weight">Вес (должен быть > 0).</param>
    /// <param name="productionDate">Дата производства (либо она, либо дата истечения срока годности expirationDate должна быть указана).</param>
    /// <param name="expirationDate">Дата истечения срока (либо она, либо дата производства productionDate должна быть указана).</param>
    /// <param name="expirationDays">Срок годности в днях (используется, если указана productionDate, но не expirationDate). По умолчанию 100 дней.</param>
    /// <exception cref="ArgumentException">Выбрасывается при некорректных параметрах.</exception>
    public Box(
        int id,
        double width,
        double height,
        double depth,
        double weight,
        DateOnly? productionDate,
        DateOnly? expirationDate,
        int expirationDays = DefaultExpirationDays)
    {
        if (width <= 0 || height <= 0 || depth <= 0 || weight <= 0)
            throw new ArgumentException("Размеры и вес коробки должны иметь положительные значения.");
        if (expirationDays <= 0)
            throw new ArgumentException("Срок годности должен быть положительным.");

        Id = id;
        Width = width;
        Height = height;
        Depth = depth;
        Weight = weight;
        
        if (productionDate != null && expirationDate == null)
        {
            ProductionDate = productionDate;
            ExpirationDate = productionDate.Value.AddDays(expirationDays);
        }
        else if (productionDate == null && expirationDate != null)
        {
            ExpirationDate = expirationDate.Value;
        }
        else if(productionDate != null && expirationDate != null)
        {
            if (expirationDate.Value < productionDate.Value)
                throw new ArgumentException("Срок годности не может быть раньше даты производства.");

            ProductionDate = productionDate;
            ExpirationDate = expirationDate.Value;
        }
        else
        {
            throw new ArgumentException("Должна быть указана дата производства или дата истечения срока годности.");
        }
    }
}