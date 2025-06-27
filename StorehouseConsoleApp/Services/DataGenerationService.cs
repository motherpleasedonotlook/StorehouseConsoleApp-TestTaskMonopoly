using StorehouseConsoleApp.Models;

namespace StorehouseConsoleApp.Services;

/// <summary>
/// Сервис для генерации тестовых данных паллет и коробок
/// </summary>
/// <param name="settings">Настройки генерации данных</param>
public class DataGenerationService(GenerationSettings settings)
{
    private readonly Random _random = new();
    private int _nextBoxId = 1;

    /// <summary>
    /// Генерирует случайное число double в заданном диапазоне с округлением до 1 знака
    /// </summary>
    /// <param name="minValue">Минимальное значение</param>
    /// <param name="maxValue">Максимальное значение</param>
    /// <returns>Случайное число в диапазоне [minValue, maxValue]</returns>
    private double GetRandomDouble(double minValue, double maxValue) =>
        Math.Round(minValue + _random.NextDouble() * (maxValue - minValue), 1);
    
    /// <summary>
    /// Генерирует список паллет с коробками
    /// </summary>
    /// <param name="quantity">Количество паллет для генерации</param>
    /// <returns>Список сгенерированных паллет </returns>
    public List<Pallet?> GetPallets(int quantity)
    {
        return Enumerable.Range(1, quantity)
            .Select(i => {
                try { return GeneratePallet(i); }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка генерации паллеты №{i}: {ex.Message}");
                    return null;
                }
            })
            .Where(p => p != null)
            .ToList();
    }
    
    /// <summary>
    /// Генерирует одну паллету с случайным количеством коробок
    /// </summary>
    /// <param name="id">Идентификатор паллеты</param>
    /// <returns>Сгенерированная паллета</returns>
    private Pallet GeneratePallet(int id)
    {
        var pallet = new Pallet(
            id,
            settings.Pallet.Width,
            settings.Pallet.Height,
            settings.Pallet.Depth
        );
        if (settings.MaxBoxesPerPallet == 0)
            return pallet;
        var boxesCount = _random.Next(0, settings.MaxBoxesPerPallet + 1);
        for (var j = 0; j < boxesCount; j++)
        {
            try
            {
                pallet.AddBox(GenerateBox(_nextBoxId));
                _nextBoxId++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка добавления коробки на паллету №{id}: {ex.Message}");
            }
        }
        return pallet;
    }

    /// <summary>
    /// Генерирует коробку со случайными параметрами
    /// </summary>
    /// <param name="id">Идентификатор коробки</param>
    /// <returns>Сгенерированная коробка</returns>
    private Box GenerateBox(int id)
    {
        var productionDate = DateTime.Now.AddDays(
            _random.Next(
                settings.MinProductionDateDaysOffset,
                settings.MaxProductionDateDaysOffset + 1));
        
        return new Box(
            id,
            GetRandomDouble(settings.MinBox.Width, settings.MaxBox.Width),
            GetRandomDouble(settings.MinBox.Height, settings.MaxBox.Height),
            GetRandomDouble(settings.MinBox.Depth, settings.MaxBox.Depth),
            GetRandomDouble(settings.MinBoxWeight, settings.MaxBoxWeight),
            DateOnly.FromDateTime(productionDate),
            expirationDate: null
        );
    }
}