using System;
using NUnit.Framework;
using StorehouseConsoleApp.Models;

namespace StorehouseConsoleAppTests;

[TestFixture]
public class PalletCreatingTests
{
    [Test]
    //Создание паллеты с валидными параметрами
    public void Pallet_Creation_WithValidParameters_Succeeds()
    {
        var pallet = new Pallet(1, 100, 150, 80);
            
        Assert.That(pallet.Id, Is.EqualTo(1));
        Assert.That(pallet.Width, Is.EqualTo(100));
        Assert.That(pallet.Height, Is.EqualTo(150));
        Assert.That(pallet.Depth, Is.EqualTo(80));
        Assert.That(pallet.Weight, Is.EqualTo(30));
        Assert.That(pallet.Volume, Is.EqualTo(100 * 150 * 80));
        Assert.That(pallet.Boxes, Is.Empty);
    }

    [Test]
    // Попытка создания паллет с невалидными параметрами
    public void Pallet_Creation_WithInvalidParameters_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new Pallet(1, -100, 150, 80));
        Assert.Throws<ArgumentException>(() => new Pallet(1, 100, -150, 80));
        Assert.Throws<ArgumentException>(() => new Pallet(1, 100, 150, -80));
        Assert.Throws<ArgumentException>(() => new Pallet(1, 100, 150, 80, -30));
    }

    [Test]
    // Коробки должны успешно добавиться + корректные данные рассчитываются
    public void AddBox_ValidBox_ShouldBeAdded()
    {
        var pallet = new Pallet(1, 100, 100, 100);
        pallet.AddBox(new Box(1, 50, 50, 50, 10, null, new DateOnly(2025, 2, 23)));
        pallet.AddBox(new Box(1, 100, 100, 100, 24, null, new DateOnly(2025, 4, 1)));
            
        Assert.That(pallet.Boxes, Has.Count.EqualTo(2));
        Assert.That(pallet.Weight, Is.EqualTo(64)); // 30 + 10 + 24
        Assert.That(pallet.ExpirationDate == new DateOnly(2025, 2, 23));
    }

    [Test]
    // Вписывается впритык лежа на основании
    public void AddBox_FitsBottomEndToEnd_ShouldBeAdded()
    {
        var pallet = new Pallet(1, 100, 100, 100);
        var box = new Box(1, 100, 101, 100, 5, null, new DateOnly(2025, 2, 24));
    
        Assert.DoesNotThrow(() => pallet.AddBox(box));
    }
        
    [Test]
    // Вписывается впритык на боку
    public void AddBox_FitsSideEndToEnd_ShouldBeAdded()
    {
        var pallet = new Pallet(1, 100, 100, 100);
        var box = new Box(1, 100, 100, 101, 5, null, new DateOnly(2025, 2, 24));
    
        Assert.DoesNotThrow(() => pallet.AddBox(box));
    }
        
    [Test]
    // Вписывается впритык на торце
    public void AddBox_FitsEndEndToEnd_ShouldBeAdded()
    {
        var pallet = new Pallet(1, 100, 100, 100);
        var box = new Box(1, 101, 100, 100, 5, null, new DateOnly(2025, 2, 24));
    
        Assert.DoesNotThrow(() => pallet.AddBox(box));
    }
        
    [Test]
    // Не вписывается - только ширина в рамках
    public void AddBox_DoesNotFitOnlyWidth_ShouldThrowInvalidOperationException()
    {
        var pallet = new Pallet(1, 100, 100, 100);
        var box = new Box(1, 100, 101, 101, 5, null, new DateOnly(2025, 2, 24));
    
        Assert.Throws<InvalidOperationException>(() => pallet.AddBox(box));
    }
        
    [Test]
    // Не вписывается - только высота в рамках
    public void AddBox_DoesNotFitOnlyHeight_ShouldThrowInvalidOperationException()
    {
        var pallet = new Pallet(1, 100, 100, 100);
        var box = new Box(1, 101, 100, 101, 5, null, new DateOnly(2025, 2, 24));
    
        Assert.Throws<InvalidOperationException>(() => pallet.AddBox(box));
    }
        
    [Test]
    // Не вписывается - только глубина в рамках
    public void AddBox_DoesNotFitOnlyDepth_ShouldThrowInvalidOperationException()
    {
        var pallet = new Pallet(1, 100, 100, 100);
        var box = new Box(1, 101, 101, 100, 5, null, new DateOnly(2025, 2, 24));
    
        Assert.Throws<InvalidOperationException>(() => pallet.AddBox(box));
    }
        
    [Test]
    // Не вписывается - все три стороны больше нужного
    public void AddBox_DoesNotFitAllSides_ShouldThrowInvalidOperationException()
    {
        var pallet = new Pallet(1, 100, 100, 100);
        var box = new Box(1, 101, 101, 101, 5, null, new DateOnly(2025, 2, 24));
    
        Assert.Throws<InvalidOperationException>(() => pallet.AddBox(box));
    }
}