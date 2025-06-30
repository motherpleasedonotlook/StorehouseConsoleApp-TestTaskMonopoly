using StorehouseConsoleApp.Services;

try
{
    Console.WriteLine("Store Console Application Test Task\n");
    
    //var settings = new GenerationSettings();
    // Определяем настройки генерации (по умолчанию были европейские поддоны, переопределим на американские)
    var settings = new GenerationSettings
    {
        MaxBoxesPerPallet = 3,
        PalletDimensions = new GenerationSettings.Dimensions
        {
            Width = 101.6,
            Height = 20,
            Depth = 121.9
        },
        MinBoxDimensions = new GenerationSettings.Dimensions
        {
            Width = 30,
            Height = 20,
            Depth = 30,
        },
        MaxBoxDimensions = new GenerationSettings.Dimensions
        {
            Width =100,
            Height = 10,
            Depth = 120,
        },
        MinProductionDateDaysOffset = -15
    };
    
    // Создаём генератор с этими настройками
    var generator = new DataGenerationService(settings); 
    
    // Генерируем паллеты
    var pallets = generator.GetPallets(15);

    // Выводим результаты генерации
    Console.WriteLine("======= Результат генерации тестовых значений =======");
    HandleDataService.DisplayPalletList(pallets);
    
    //Группируем по сроку годности, группы - по возрастанию срока годности, внутри группы - по весу.
    Console.WriteLine("#1 Сгруппировать паллеты по сроку годности, отсортировать по" +
                      " возрастанию срока годности, в группе отсортировать паллеты по весу внутри группы\n" +
                      "(Пустые паллеты будут проигнорированны, т.к. у них нет срока годности.)");
    try
    {
        HandleDataService.DisplayPalletList(HandleDataService.SortByExpirationDateLessWeightFirst(pallets));
    }
    catch (ArgumentException exception)
    {
        Console.WriteLine(exception.Message);
    }

    //Три паллеты с наибольшим сроком годности, паллеты по возрастанию объема
    Console.WriteLine("#2 Три паллеты, отсортированные по возрастанию объема, которые содержат коробки " +
                      "с наибольшим сроком годности");
    try
    {
        HandleDataService.DisplayPalletList(HandleDataService.TopThreeLongestLifeBoxPallets(pallets));
    }
    catch(ArgumentException exception)
    {
        Console.WriteLine(exception.Message);
    }

}
catch (Exception exception)
{
    Console.WriteLine(exception.Message);
}



