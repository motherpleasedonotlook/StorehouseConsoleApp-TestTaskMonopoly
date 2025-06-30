using StorehouseConsoleApp.Models;

namespace StorehouseConsoleApp.Services;

/// <summary>
/// Сервис для обработки, сортировки и вывода данных о паллетах и коробках
/// </summary>
public static class HandleDataService
{
    /// <summary>
    /// Группирует все паллеты по сроку годности, сортирует группы по возрастанию срока годности, в каждой группе сортирует паллеты по весу.
    /// </summary>
    /// <param name="pallets">Список паллет для сортировки</param>
    /// <returns>Отсортированный список паллет</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если передан пустой список</exception>
    public static List<Pallet> SortByExpirationDateLessWeightFirst(List<Pallet> pallets)
    {
        if (pallets.Count == 0)
            throw new ArgumentException("Список пуст");
        
        var palletsWithBoxes = pallets.Where(p => p.Boxes?.Count > 0).ToList();
        
        if (palletsWithBoxes.Count == 0)
            throw new ArgumentException("Передан список пустых паллет");
        
        return palletsWithBoxes
            .GroupBy(p => p.ExpirationDate)
            .OrderBy(g => g.Key) 
            .SelectMany(g => g
                .OrderBy(p => p.Weight))
            .ToList();
    }

    /// <summary>
    /// Сортирует паллеты по максимальному сроку годности коробок внутри них (от меньшего к большему), находит максимальную дату истечения срока среди всех коробок на паллете
    /// и возвращает первые 3 паллеты из отсортированного списка.
    /// </summary>
    /// <param name="pallets">Список паллет для обработки</param>
    /// <returns>Список из 3 паллет с коробками наибольшего срока годности</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если передан пустой список или список, состоящий из пустых паллет</exception>
    public static List<Pallet> TopThreeLongestLifeBoxPallets(List<Pallet> pallets)
    {
        if (pallets.Count == 0)
            throw new ArgumentException("Список пуст");
        
        var palletsWithBoxes = pallets.Where(p => p.Boxes?.Count > 0).ToList();
        
        if (palletsWithBoxes.Count == 0)
            throw new ArgumentException("Передан список пустых паллет");
        
        return palletsWithBoxes
            .OrderByDescending(p => p.Boxes.Max(b => b.ExpirationDate))
            .Take(3)
            .OrderBy(p => p.Volume)
            .ToList();
    }

    /// <summary>
    /// Выводит в консоль подробную информацию о паллетах и их содержимом
    /// </summary>
    /// <param name="pallets">Список паллет для вывода</param>
    /// <exception cref="ArgumentException">Выбрасывается, если передан пустой список или null </exception>
    public static void DisplayPalletList(List<Pallet?>? pallets)
    {
        if (pallets == null || pallets.Count == 0)
            throw new ArgumentException("Передан пустой список");

        foreach (var pallet in pallets)
        {
            Console.WriteLine($"ПАЛЛЕТА ID: {pallet.Id}\n" +
                              $"- Ширина: {pallet.Width} см, высота: {pallet.Height} см, глубина: {pallet.Depth} см\n" +
                              $"- Вес: {Math.Round(pallet.Weight,2)} кг\n" +
                              $"- Объем: {Math.Round(pallet.Volume,2)} см^3\n" +
                              $"- Срок годности оканчивается: {pallet.ExpirationDate?.ToString("dd.MM.yyyy") ?? "нельзя определить"}\n");
            
            if (pallet.Boxes == null || pallet.Boxes.Count == 0)
            {
                Console.WriteLine("- На паллете нет коробок");
            }
            else
            {
                Console.WriteLine($"   - Количество коробок: {pallet.Boxes.Count}\n" +
                                  $"       Коробки:");
                
                foreach (var box in pallet.Boxes)
                {
                    Console.WriteLine($"       КОРОБКА ID: {box.Id}\n" +
                                      $"       - Ширина: {box.Width} см, высота: {box.Height} см, глубина: {box.Depth} см\n" +
                                      $"       - Объем: {Math.Round(box.Volume,2)} см^3\n" +
                                      $"       - Вес: {box.Weight} кг\n" +
                                      $"       - Дата производства: {box.ProductionDate?.ToString("dd.MM.yyyy") ?? "не указана"}\n" +
                                      $"       - Срок годности оканчивается: {box.ExpirationDate.ToString("dd.MM.yyyy")}\n" +
                                      $"     ---------------------------------------------------------------------------");
                    
                }
            }
            Console.WriteLine("=======================================================================================");
        }
    }
}