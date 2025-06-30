using System;
using NUnit.Framework;
using StorehouseConsoleApp.Models;
namespace StorehouseConsoleAppTests;

[TestFixture]
public class BoxCreatingTests
{
    [Test]
    // Создание коробки с валидными параметрами
    public void Constructor_WithPositiveParameters_ShouldCreateBox()
    {
        var productionDate = new DateOnly(2025, 1, 1);

        var box = new Box(1, 10, 10, 10, 5, productionDate, null);

        Assert.That(box.Id, Is.EqualTo(1));
        Assert.That(box.Width, Is.EqualTo(10));
        Assert.That(box.Height, Is.EqualTo(10));
        Assert.That(box.Depth, Is.EqualTo(10));
        Assert.That(box.Weight, Is.EqualTo(5));
        Assert.That(box.ProductionDate, Is.EqualTo(productionDate));
        Assert.That(box.ExpirationDate, Is.EqualTo(productionDate.AddDays(100)));
    }

    [Test]
    // Неположительная ширина
    public void Constructor_WithNonPositiveWidth_ShouldThrowArgumentException()
    {
        var productionDate = new DateOnly(2025, 1, 1);
            
        Assert.Throws<ArgumentException>(() => 
            new Box(1, -1, 10, 10, 5, productionDate, null));
        Assert.Throws<ArgumentException>(() => 
            new Box(1, 0, 10, 10, 5, productionDate, null));
    }

    [Test]
    // Неположительная высота
    public void Constructor_WithNonPositiveHeight_ShouldThrowArgumentException()
    {
        var productionDate = new DateOnly(2025, 1, 1);
            
        Assert.Throws<ArgumentException>(() => 
            new Box(1, 10, -1, 10, 5, productionDate, null));
        Assert.Throws<ArgumentException>(() => 
            new Box(1, 10, 0, 10, 5, productionDate, null));
    }

    [Test]
    // Неположительная глубина
    public void Constructor_WithNonPositiveDepth_ShouldThrowArgumentException()
    {
        var productionDate = new DateOnly(2025, 1, 1);
            
        Assert.Throws<ArgumentException>(() => 
            new Box(1, 10, 10, -1, 5, productionDate, null));
        Assert.Throws<ArgumentException>(() => 
            new Box(1, 10, 10, 0, 5, productionDate, null));
    }

    [Test]
    // Неположительный вес
    public void Constructor_WithNonPositiveWeight_ShouldThrowArgumentException()
    {
        var productionDate = new DateOnly(2025, 1, 1);
            
        Assert.Throws<ArgumentException>(() => 
            new Box(1, 10, 10, 10, 0, productionDate, null));
        Assert.Throws<ArgumentException>(() => 
            new Box(1, 10, 10, 10, -1, productionDate, null));
    }

    [Test]
    // Неположительный срок годности в днях
    public void Constructor_WithNonPositiveExpirationDays_ShouldThrowArgumentException()
    {
        var productionDate = new DateOnly(2025, 1, 1);
            
        Assert.Throws<ArgumentException>(() => 
            new Box(1, 10, 10, 10, 5, productionDate, null, 0));
        Assert.Throws<ArgumentException>(() => 
            new Box(1, 10, 10, 10, 5, productionDate, null, -1));
    }

    [Test]
    // Не указан ни срок годности, ни дата производства
    public void Constructor_WithBothDatesNull_ShouldThrowArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => 
            new Box(1, 10, 10, 10, 5, null, null));
        Assert.That(ex?.Message, Does.Contain("Должна быть указана"));
    }

    [Test]
    // Срок годности истекает раньше даты производства
    public void Constructor_WithExpirationDateBeforeProductionDate_ShouldThrowArgumentException()
    {
        var productionDate = new DateOnly(2023, 1, 1);
        var expirationDate = new DateOnly(2022, 12, 31);
            
        var ex = Assert.Throws<ArgumentException>(() => 
            new Box(1, 10, 10, 10, 5, productionDate, expirationDate));
        Assert.That(ex?.Message, Does.Contain("не может быть раньше"));
    }

    [Test]
    // Указана только дата истечения срока годности
    public void Constructor_WithOnlyExpirationDate_ShouldCreateBox()
    {
        var expirationDate = new DateOnly(2023, 12, 31);

        var box = new Box(1, 10, 10, 10, 5, null, expirationDate);

        Assert.That(box.ProductionDate, Is.Null);
        Assert.That(box.ExpirationDate, Is.EqualTo(expirationDate));
    }

    [Test]
    // Указаны обе даты и они корректны
    public void Constructor_WithBothDates_ShouldCreateBox()
    {
        var productionDate = new DateOnly(2025, 1, 1);
        var expirationDate = new DateOnly(2025, 12, 31);
            
        var box = new Box(1, 10, 10, 10, 5, productionDate, expirationDate);
            
        Assert.That(box.ProductionDate, Is.EqualTo(productionDate));
        Assert.That(box.ExpirationDate, Is.EqualTo(expirationDate));
    }

    [Test]
    // Проверка корректности подсчета объема коробки
    public void Volume_ShouldBeCalculatedCorrectly()
    {
        var box = new Box(1, 2, 3, 4, 5, new DateOnly(2023, 1, 1), null);
            
        Assert.That(box.Volume, Is.EqualTo(24)); // 2 * 3 * 4 = 24
    }
}