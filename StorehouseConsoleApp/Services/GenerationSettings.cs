namespace StorehouseConsoleApp.Services;

/// <summary>
/// Класс настроек для генерации тестовых данных паллет и коробок
/// </summary>
public class GenerationSettings
{
    /// <summary>
    /// Максимальное количество коробок на одной паллете (по умолчанию 5)
    /// </summary>
    public int MaxBoxesPerPallet { get; set; } = 5;
    
    /// <summary>
    /// Класс, представляющий габаритные размеры (ширина, высота, глубина)
    /// </summary>
    public class Dimensions
    {
        /// <summary>
        /// Ширина (по умолчанию 120)
        /// </summary>
        public double Width { get; set; } = 120;
        
        /// <summary>
        /// Высота (по умолчанию 20)
        /// </summary>
        public double Height { get; set; } = 20;
        
        /// <summary>
        /// Глубина (по умолчанию 80)
        /// </summary>
        public double Depth { get; set; } = 80;
    }
    
    /// <summary>
    /// Размеры паллеты (по умолчанию Width: 120, Height: 20, Depth: 80)
    /// </summary>
    public Dimensions PalletDimensions { get; set; } = new();
    
    /// <summary>
    /// Максимальные размеры коробок (по умолчанию Width: 120, Height: 20, Depth: 80)
    /// </summary>
    public Dimensions MaxBoxDimensions { get; set; } = new();
    
    /// <summary>
    /// Минимальные размеры коробок (автоматически устанавливаются как 1/3 от MaxBoxDimensions при создании, если не переопределять)
    /// </summary>
    public Dimensions MinBoxDimensions { get; set; }
    
    /// <summary>
    /// Минимальный вес коробки в кг (по умолчанию 0.1)
    /// </summary>
    public double MinBoxWeight { get; set; } = 0.1;
    
    /// <summary>
    /// Максимальный вес коробки в кг (по умолчанию 100)
    /// </summary>
    public double MaxBoxWeight { get; set; } = 100;
    
    /// <summary>
    /// Смещение даты в днях от текущей даты для генерации самой давней даты производства (по умолчанию -365)
    /// </summary>
    public int MinProductionDateDaysOffset { get; set; } = -365;
    
    /// <summary>
    /// Смещение даты в днях от текущей даты для генерации самой ближней даты производства (по умолчанию 0)
    /// </summary>
    public int MaxProductionDateDaysOffset { get; set;} = 0;

    /// <summary>
    /// Конструктор по умолчанию. Инициализирует минимальные размеры коробок как 1/3 от максимальных размеров, чтобы избежать противоречий
    /// </summary>
    public GenerationSettings()
    {
        // Минимум 1/3 от максимального размера
        MinBoxDimensions = new Dimensions
        {
            Width = Math.Max(1, MaxBoxDimensions.Width / 3),
            Height = Math.Max(1, MaxBoxDimensions.Height / 3),
            Depth = Math.Max(1, MaxBoxDimensions.Depth / 3)
        };
    }
}