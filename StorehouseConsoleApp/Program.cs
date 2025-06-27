using StorehouseConsoleApp.Services;

try
{
    Console.WriteLine("Store Console Application Test Task\n");
    
    //var settings = new GenerationSettings();
    // Определяем настройки генерации (по умолчанию были европейские поддоны, переопределим на американские)
    var settings = new GenerationSettings
    {
        MaxBoxesPerPallet = 3,
        Pallet = new GenerationSettings.Dimensions
        {
            Width = 101.6,
            Height = 20,
            Depth = 121.9
        },
        MinBox = new GenerationSettings.Dimensions
        {
            Width = 30,
            Height = 20,
            Depth = 30,
        },
        MaxBox = new GenerationSettings.Dimensions
        {
            Width =100,
            Height = 10,
            Depth = 120,
        }
    };
    
    // Создаём генератор с этими настройками
    var generator = new DataGenerationService(settings); 
    
    // Генерируем паллеты
    var pallets = generator.GetPallets(20);

    // Выводим результаты генерации
    Console.WriteLine("======= Результат генерации тестовых значений =======");
    HandleDataService.DisplayPalletList(pallets);
    
    //Группируем по сроку годности, группы - по возрастанию срока годности, внутри группы - по весу.
    Console.WriteLine("#1 Сгруппировать паллеты по сроку годности, отсортировать по" +
                      " возрастанию срока годности, в группе отсортировать паллеты по весу внутри группы\n" +
                      "(Пустые паллеты будут проигнорированны, т.к. у них нет срока годности.)");
    HandleDataService.DisplayPalletList(HandleDataService.SortByExpirationDateLessWeightFirst(pallets));
    
    //Три паллеты с наибольшим сроком годности, паллеты по возрастанию объема
    Console.WriteLine("#2 Три паллеты, отсортированные по возрастанию объема, которые содержат коробки " +
                      "с наибольшим сроком годности");
    HandleDataService.DisplayPalletList(HandleDataService.TopThreeLongestLifePallets(pallets));
    
}
catch (Exception exception)
{
    Console.WriteLine(exception.Message);
}



