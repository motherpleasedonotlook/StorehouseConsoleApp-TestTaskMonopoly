using NUnit.Framework;
using StorehouseConsoleApp.Models;
using StorehouseConsoleApp.Services;
using System;
using System.Collections.Generic;

namespace StorehouseConsoleAppTests;

[TestFixture]
public class HandleDataServiceTests
{
    private List<Pallet> _emptyPallets;
    private List<Pallet> _palletsWithBoxes;

    [SetUp]
    public void Setup()
    {
        // Пустые паллеты (без коробок)
        _emptyPallets = 
        [
            new Pallet(1, 100, 100, 100),
            new Pallet(2, 100, 100, 100)
        ];

        // Создаем коробки 
        var box1 = new Box(1, 10, 10, 10, 5, null, new DateOnly(2023, 1, 1));
        var box2 = new Box(2, 10, 10, 10, 10, null, new DateOnly(2023, 1, 1));
        var box3 = new Box(3, 10, 10, 10, 15, null, new DateOnly(2023, 12, 2));
        var box4 = new Box(4, 10, 10, 10, 20, null, new DateOnly(2023, 12, 2));
        var box5 = new Box(5, 5, 10, 10, 5, null, new DateOnly(2024, 1, 4));
        var box6 = new Box(6, 30, 5, 15, 10, null, new DateOnly(2024, 2, 8));
        var box7 = new Box(7, 10, 15, 10, 15, null, new DateOnly(2024, 3, 1));
        var box8 = new Box(8, 10, 15, 10, 20, null, new DateOnly(2024, 4, 1));

        // Создаем паллеты
        _palletsWithBoxes =
        [
            new Pallet(3, 100, 100, 100),
            new Pallet(4, 100, 100, 100),
            new Pallet(5, 100, 100, 100),
            new Pallet(6, 100, 100, 100),
            new Pallet(7, 100, 100, 100)
        ];
        
        // Добавляем коробки на паллеты
        _palletsWithBoxes[0].AddBox(box7);
        _palletsWithBoxes[0].AddBox(box8);//2
        _palletsWithBoxes[1].AddBox(box1);
        _palletsWithBoxes[1].AddBox(box6);//3
        _palletsWithBoxes[2].AddBox(box2);
        _palletsWithBoxes[2].AddBox(box3);
        _palletsWithBoxes[3].AddBox(box5);//1
        _palletsWithBoxes[4].AddBox(box4);
        
    }
    

    [Test]
    // Проверка группировки - передан пустой список
    public void SortByExpirationDateLessWeightFirst_EmptyList_ThrowsArgumentException()
    {
        List<Pallet> emptyList = [];
        
        var ex = Assert.Throws<ArgumentException>(() => HandleDataService.SortByExpirationDateLessWeightFirst(emptyList));
        Assert.That(ex.Message, Is.EqualTo("Список пуст"));
    }

    [Test]
    // Проверка группировки - передан список, в котором все паллеты пустые
    public void SortByExpirationDateLessWeightFirst_PalletsWithoutBoxes_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => HandleDataService.SortByExpirationDateLessWeightFirst(_emptyPallets));
        Assert.That(ex.Message, Is.EqualTo("Передан список пустых паллет"));
    }

    [Test]
    // Проверка группировки - корректная группировка
    public void SortByExpirationDateLessWeightFirst_PalletsWithBoxes_ReturnsCorrectlySortedList()
    {
        var result = HandleDataService.SortByExpirationDateLessWeightFirst(_palletsWithBoxes);

        Assert.That(result.Count, Is.EqualTo(5)); // Все добавились
        Assert.That(result[0].Id, Is.EqualTo(4)); // Срок 01.01.23, вес 45 кг
        Assert.That(result[1].Id, Is.EqualTo(5)); // Срок 01.01.23, вес 65 кг
        Assert.That(result[2].Id, Is.EqualTo(7)); // Срок 02.12.23, вес 50 кг
        Assert.That(result[3].Id, Is.EqualTo(6)); // Срок 04.01.24, вес 35 кг
        Assert.That(result[4].Id, Is.EqualTo(3)); // Срок 01.03.24, вес 65 кг
    }
    
    [Test]
    // Проверка группировки - корректная группировка при исключении пустых паллет
    public void SortByExpirationDateLessWeightFirst_EmptyBoxes_ReturnsCorrectlySortedList()
    {
        var newList = _emptyPallets;
        newList.AddRange(_palletsWithBoxes);
        var result = HandleDataService.SortByExpirationDateLessWeightFirst(newList);

        Assert.That(result.Count, Is.EqualTo(5)); // Добавились только непустые
        Assert.That(result[0].Id, Is.EqualTo(4)); // Срок 01.01.23, вес 45 кг
        Assert.That(result[1].Id, Is.EqualTo(5)); // Срок 01.01.23, вес 65 кг
        Assert.That(result[2].Id, Is.EqualTo(7)); // Срок 02.12.23, вес 50 кг
        Assert.That(result[3].Id, Is.EqualTo(6)); // Срок 04.01.24, вес 35 кг
        Assert.That(result[4].Id, Is.EqualTo(3)); // Срок 01.03.24, вес 65 кг
    }

    [Test]
    // Проверка выборки - передан пустой список
    public void TopThreeLongestLifeBoxPallets_EmptyList_ThrowsArgumentException()
    {
        var emptyList = new List<Pallet>();

        var ex = Assert.Throws<ArgumentException>(() => HandleDataService.TopThreeLongestLifeBoxPallets(emptyList));
        Assert.That(ex.Message, Is.EqualTo("Список пуст"));
    }

    [Test]
    // Проверка выборки - передан список пустых паллет
    public void TopThreeLongestLifeBoxPallets_PalletsWithoutBoxes_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => HandleDataService.TopThreeLongestLifeBoxPallets(_emptyPallets));
        
        Assert.That(ex.Message, Is.EqualTo("Передан список пустых паллет"));
    }

    [Test]
    // Проверка выборки - корректные данные
    public void TopThreeLongestLifeBoxPallets_ReturnsResult()
    {
        var result = HandleDataService.TopThreeLongestLifeBoxPallets(_palletsWithBoxes);

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result[0].Id, Is.EqualTo(6)); // Коробка 5 срок 04.01.24, объем 500
        Assert.That(result[1].Id, Is.EqualTo(3)); // Коробка 8 срок 01.04.24, объем 1500
        Assert.That(result[2].Id, Is.EqualTo(4)); // Коробка 6 срок 08.02.24, объем 2250
    }

}