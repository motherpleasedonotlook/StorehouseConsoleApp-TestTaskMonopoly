using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StorehouseConsoleApp.Models;
using StorehouseConsoleApp.Services;

namespace StorehouseConsoleAppTests
{
    [TestFixture]
    public class PalletTests
    {
        [Test]
        public void Pallet_Creation_WithValidParameters_Succeeds()
        {
            var pallet = new Pallet(1, 100, 150, 80);
            
            Assert.That(pallet.Id, Is.EqualTo(1));
            Assert.That(pallet.Width, Is.EqualTo(100));
            Assert.That(pallet.Height, Is.EqualTo(150));
            Assert.That(pallet.Depth, Is.EqualTo(80));
            Assert.That(pallet.Weight, Is.EqualTo(30)); // Default weight
            Assert.That(pallet.Volume, Is.EqualTo(100 * 150 * 80));
            Assert.That(pallet.Boxes, Is.Empty);
        }

        [Test]
        public void Pallet_Creation_WithInvalidParameters_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new Pallet(1, -100, 150, 80));
            Assert.Throws<ArgumentException>(() => new Pallet(1, 100, -150, 80));
            Assert.Throws<ArgumentException>(() => new Pallet(1, 100, 150, -80));
            Assert.Throws<ArgumentException>(() => new Pallet(1, 100, 150, 80, -30));
        }
    }

    [TestFixture]
    public class BoxTests
    {
        [Test]
        public void Box_Creation_WithProductionDate_SetsCorrectExpirationDate()
        {
            var productionDate = new DateOnly(2023, 1, 1);
            var box = new Box(1, 10, 10, 10, 5, productionDate, null);
            
            Assert.That(box.ExpirationDate, Is.EqualTo(productionDate.AddDays(100)));
        }

        [Test]
        public void Box_Creation_WithExpirationDate_SetsCorrectProperties()
        {
            var expirationDate = new DateOnly(2023, 5, 1);
            var box = new Box(1, 10, 10, 10, 5, null, expirationDate);
            
            Assert.That(box.ExpirationDate, Is.EqualTo(expirationDate));
            Assert.That(box.ProductionDate, Is.Null);
        }

        [Test]
        public void Box_Creation_WithInvalidParameters_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new Box(1, -10, 10, 10, 5, new DateOnly(2023, 1, 1), null));
            Assert.Throws<ArgumentException>(() => new Box(1, 10, 10, 10, -5, null, new DateOnly(2023, 1, 1)));
            Assert.Throws<ArgumentException>(() => new Box(1, 10, 10, 10, 5, null, null));
        }
    }

    [TestFixture]
    public class PalletOperationsTests
    {
        [Test]
        public void AddBox_ValidBox_SuccessfullyAdded()
        {
            var pallet = new Pallet(1, 100, 100, 100);
            var box = new Box(1, 50, 50, 50, 10, null, new DateOnly(2023, 1, 1));
            
            pallet.AddBox(box);
            
            Assert.That(pallet.Boxes, Has.Count.EqualTo(1));
            Assert.That(pallet.Weight, Is.EqualTo(40)); // 30 (pallet) + 10 (box)
        }

        [Test]
        public void AddBox_OversizedBox_ThrowsException()
        {
            var pallet = new Pallet(1, 100, 100, 100);
            var box = new Box(1, 101, 50, 50, 10, null, new DateOnly(2023, 1, 1));
            
            Assert.Throws<InvalidOperationException>(() => pallet.AddBox(box));
        }
    }

    [TestFixture]
    public class HandleDataServiceTests
    {
        private Pallet CreateTestPallet(int id, DateOnly expirationDate, double boxWeight, int boxCount = 1)
        {
            var pallet = new Pallet(id, 100, 100, 100);
            for (int i = 0; i < boxCount; i++)
            {
                pallet.AddBox(new Box(
                    id: i + 1,
                    width: 50,
                    height: 50,
                    depth: 50,
                    weight: boxWeight,
                    productionDate: expirationDate.AddDays(-100),
                    expirationDate: null));
            }
            return pallet;
        }

        [Test]
        public void SortByExpirationDateLessWeightFirst_ValidPallets_ReturnsCorrectOrder()
        {
            var pallets = new List<Pallet>
            {
                CreateTestPallet(1, new DateOnly(2023, 3, 1), 20),
                CreateTestPallet(2, new DateOnly(2023, 1, 1), 30),
                CreateTestPallet(3, new DateOnly(2023, 3, 1), 40),
                CreateTestPallet(4, new DateOnly(2023, 2, 1), 25),
                CreateTestPallet(5, new DateOnly(2023, 1, 1), 50),
                CreateTestPallet(6, new DateOnly(2023, 2, 1), 30) 
            };

            var result = HandleDataService.SortByExpirationDateLessWeightFirst(pallets);

            var expectedOrder = new[] { 2, 5, 4, 6, 1, 3 };
            Assert.That(result.Select(p => p.Id), Is.EqualTo(expectedOrder));
        }

        [Test]
        public void TopThreeLongestLifePallets_ValidPallets_ReturnsCorrectPallets()
        {
            var pallets = new List<Pallet>
            {
                CreateTestPallet(1, new DateOnly(2023, 12, 31), 10, 3), // Объем 100*100*100 + 3*(50*50*50) = 1000000 + 3*125000 = 1375000
                CreateTestPallet(2, new DateOnly(2023, 6, 1), 20),
                CreateTestPallet(3, new DateOnly(2023, 12, 31), 30),
                CreateTestPallet(4, new DateOnly(2023, 12, 31), 40),
                CreateTestPallet(5, new DateOnly(2023, 9, 1), 50) 
            };

            var result = HandleDataService.TopThreeLongestLifePallets(pallets);

            // Проверяем что вернулись паллеты с самыми поздними датами
            var expectedIds = new HashSet<int> { 1, 3, 4 };
            Assert.That(result.Select(p => p.Id), Is.SubsetOf(expectedIds));
            Assert.That(result, Has.Count.EqualTo(3));
    
            // Первой должна быть паллета с наименьшим объемом (ID=3 или 4)
            Assert.That(result[0].Volume, Is.LessThanOrEqualTo(result[1].Volume));
            Assert.That(result[1].Volume, Is.LessThanOrEqualTo(result[2].Volume));
        }

        [Test]
        public void SortByExpirationDateLessWeightFirst_EmptyList_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => 
                HandleDataService.SortByExpirationDateLessWeightFirst(new List<Pallet>()));
        }

        [Test]
        public void SortByExpirationDateLessWeightFirst_PalletWithoutBoxes_ExcludedFromResult()
        {
            var pallets = new List<Pallet>
            {
                new Pallet(1, 100, 100, 100),
                CreateTestPallet(2, new DateOnly(2023, 1, 1), 10)
            };

            var result = HandleDataService.SortByExpirationDateLessWeightFirst(pallets);

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(2));
        }
    }
}