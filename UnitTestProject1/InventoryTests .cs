using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows.Markup;

namespace WareMaster.Tests
{
    [TestClass]
    public class InventoryTests
    {
        [TestMethod]
        public void TestGetFirstSettleDate()
        {
            var mockSettlements = new Mock<DbSet<Settlement>>();
            var mockContext = new Mock<WareMasterEntities>();

            var data = new List<Settlement>
            {
                new Settlement { id = 1, Item_Id = 1, Quantity = 10, Total = 100, Settle_Date = new DateTime(2023, 1, 1) },
                new Settlement { id = 2, Item_Id = 2, Quantity = 20, Total = 200, Settle_Date = new DateTime(2023, 1, 2) },
            }.AsQueryable();

            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockContext.Setup(m => m.Settlements).Returns(mockSettlements.Object);

            Console.WriteLine("Mock Settlements:");
            foreach (var settlement in mockContext.Object.Settlements)
            {
                Console.WriteLine($"{settlement.id}, {settlement.Item_Id}, {settlement.Quantity}, {settlement.Total}, {settlement.Settle_Date}");
            }
            Globals.wareMasterEntities = mockContext.Object; 

            DateTime result = Inventory.GetFirstSettleDate();

            Assert.AreEqual(new DateTime(2023, 1, 1), result);
        }
        [TestMethod]
        public void TestGetLastSettleDate()
        {
            var mockSettlements = new Mock<DbSet<Settlement>>();
            var mockContext = new Mock<WareMasterEntities>();

            var data = new List<Settlement>
            {
                new Settlement { id = 1, Item_Id = 1, Quantity = 10, Total = 100, Settle_Date = new DateTime(2023, 1, 1) },
                new Settlement { id = 2, Item_Id = 2, Quantity = 20, Total = 200, Settle_Date = new DateTime(2023, 1, 2) },
            }.AsQueryable();

            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockContext.Setup(m => m.Settlements).Returns(mockSettlements.Object);

            Console.WriteLine("Mock Settlements:");
            foreach (var settlement in mockContext.Object.Settlements)
            {
                Console.WriteLine($"{settlement.id}, {settlement.Item_Id}, {settlement.Quantity}, {settlement.Total}, {settlement.Settle_Date}");
            }
            Globals.wareMasterEntities = mockContext.Object;

            DateTime result = Inventory.GetLastSettleDate();

            Assert.AreEqual(new DateTime(2023, 1, 2), result);
        }
        [TestMethod]
        public void TestGetFirstSettleDateByItem()
        {
            var mockSettlements = new Mock<DbSet<Settlement>>();
            var mockContext = new Mock<WareMasterEntities>();

            var data = new List<Settlement>
            {
                new Settlement { id = 1, Item_Id = 1, Quantity = 10, Total = 100, Settle_Date = new DateTime(2023, 1, 1) },
                new Settlement { id = 1, Item_Id = 1, Quantity = 10, Total = 100, Settle_Date = new DateTime(2023, 1, 2) },
                new Settlement { id = 2, Item_Id = 2, Quantity = 20, Total = 200, Settle_Date = new DateTime(2023, 1, 3) },
            }.AsQueryable();

            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockContext.Setup(m => m.Settlements).Returns(mockSettlements.Object);

            Console.WriteLine("Mock Settlements:");
            foreach (var settlement in mockContext.Object.Settlements)
            {
                Console.WriteLine($"{settlement.id}, {settlement.Item_Id}, {settlement.Quantity}, {settlement.Total}, {settlement.Settle_Date}");
            }
            Globals.wareMasterEntities = mockContext.Object;
            Item item = new Item { id = 1 };
            DateTime result = Inventory.GetFirstSettleDateByItem(item);

            Assert.AreEqual(new DateTime(2023, 1, 1), result);
        }

        [TestMethod]
        public void TestGetLastSettleDateByItem()
        {
            var mockSettlements = new Mock<DbSet<Settlement>>();
            var mockContext = new Mock<WareMasterEntities>();

            var data = new List<Settlement>
            {
                new Settlement { id = 1, Item_Id = 1, Quantity = 10, Total = 100, Settle_Date = new DateTime(2023, 1, 1) },
                new Settlement { id = 1, Item_Id = 1, Quantity = 10, Total = 100, Settle_Date = new DateTime(2023, 1, 2) },
                new Settlement { id = 2, Item_Id = 2, Quantity = 20, Total = 200, Settle_Date = new DateTime(2023, 1, 3) },
            }.AsQueryable();

            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockContext.Setup(m => m.Settlements).Returns(mockSettlements.Object);

            Console.WriteLine("Mock Settlements:");
            foreach (var settlement in mockContext.Object.Settlements)
            {
                Console.WriteLine($"{settlement.id}, {settlement.Item_Id}, {settlement.Quantity}, {settlement.Total}, {settlement.Settle_Date}");
            }
            Globals.wareMasterEntities = mockContext.Object;
            Item item = new Item { id = 1 };
            DateTime result = Inventory.GetLastSettleDateByItem(item);

            Assert.AreEqual(new DateTime(2023, 1, 2), result);
        }
        [TestMethod]
        public void TestGetLastSettlementByItem()
        {
            var mockSettlements = new Mock<DbSet<Settlement>>();
            var mockContext = new Mock<WareMasterEntities>();

            var settlementData = new List<Settlement>
    {
        new Settlement { id = 1, Item_Id = 1, Quantity = 10, Total = 100, Settle_Date = new DateTime(2023, 1, 1) },
        new Settlement { id = 2, Item_Id = 1, Quantity = 20, Total = 200, Settle_Date = new DateTime(2023, 1, 2) },
    }.AsQueryable();

            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Provider).Returns(settlementData.Provider);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Expression).Returns(settlementData.Expression);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.ElementType).Returns(settlementData.ElementType);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.GetEnumerator()).Returns(settlementData.GetEnumerator());

            mockContext.Setup(m => m.Settlements).Returns(mockSettlements.Object);

            Globals.wareMasterEntities = mockContext.Object;

            Item item = new Item { id = 1 };
            DateTime dateBefore = new DateTime(2023, 1, 3); 
            Settlement result = Inventory.GetLastSettlementByItem(item, dateBefore);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.id); 
            Assert.AreEqual(new DateTime(2023, 1, 2), result.Settle_Date);
        }

        [TestMethod]
        public void TestGetInboundsByItem()
        {
            var mockTransactions = new Mock<DbSet<Transaction>>();
            var mockContext = new Mock<WareMasterEntities>();

            var transactionData = new List<Transaction>
            {
                new Transaction { id = 1, Item_Id = 1, Quantity = 10, Total = 100, Transaction_Date = new DateTime(2023, 1, 1) },
                new Transaction { id = 2, Item_Id = 1, Quantity = -5, Total = -50, Transaction_Date = new DateTime(2023, 1, 2) },
                new Transaction { id = 3, Item_Id = 1, Quantity = 20, Total = 200, Transaction_Date = new DateTime(2023, 1, 3) },
                new Transaction { id = 4, Item_Id = 2, Quantity = 15, Total = 150, Transaction_Date = new DateTime(2023, 1, 4) },
            }.AsQueryable();

            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.Provider).Returns(transactionData.Provider);
            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.Expression).Returns(transactionData.Expression);
            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.ElementType).Returns(transactionData.ElementType);
            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.GetEnumerator()).Returns(transactionData.GetEnumerator());

            mockContext.Setup(m => m.Transactions).Returns(mockTransactions.Object);

            Globals.wareMasterEntities = mockContext.Object;

            Item item = new Item { id = 1, Itemname = "TestItem" };
            DateTime dateBegin = new DateTime(2022, 12, 31);
            DateTime dateEnd = new DateTime(2023, 1, 3);

            List<InventoryData> result = Inventory.GetInboundsByItem(item, dateBegin, dateEnd);

            Assert.AreEqual(2, result.Count); 

            Assert.AreEqual(1, result[0].id);
            Assert.AreEqual(item.Itemname, result[0].Name);
            Assert.AreEqual(10, result[0].Quantity);
            Assert.AreEqual(100, result[0].Total);
            Assert.AreEqual(new DateTime(2023, 1, 1), result[0].Date);

            Assert.AreEqual(3, result[1].id);
            Assert.AreEqual(item.Itemname, result[1].Name);
            Assert.AreEqual(20, result[1].Quantity);
            Assert.AreEqual(200, result[1].Total);
            Assert.AreEqual(new DateTime(2023, 1, 3), result[1].Date);

            dateBegin = new DateTime(2023, 1, 1);
            result = Inventory.GetInboundsByItem(item, dateBegin, dateEnd);

            Assert.AreEqual(1, result.Count); 
        }

        [TestMethod]
        public void TestGetOutboundsByItem()
        {
            var mockTransactions = new Mock<DbSet<Transaction>>();
            var mockContext = new Mock<WareMasterEntities>();

            var transactionData = new List<Transaction>
            {
                new Transaction { id = 1, Item_Id = 1, Quantity = -10, Total = -100, Transaction_Date = new DateTime(2023, 1, 1), Item = new Item { id = 1, Itemname = "TestItem1", Category_Id = 1, Unit = "TestUnit1", Location = "TestLocation1", Description = "TestDescription1" } },
                new Transaction { id = 2, Item_Id = 1, Quantity = 5, Total = 50, Transaction_Date = new DateTime(2023, 1, 2), Item = new Item { id = 1, Itemname = "TestItem1", Category_Id = 1, Unit = "TestUnit1", Location = "TestLocation1", Description = "TestDescription1" } },
                new Transaction { id = 3, Item_Id = 1, Quantity = -20, Total = -200, Transaction_Date = new DateTime(2023, 1, 3), Item = new Item { id = 1, Itemname = "TestItem1", Category_Id = 1, Unit = "TestUnit1", Location = "TestLocation1", Description = "TestDescription1" } },
                new Transaction { id = 4, Item_Id = 2, Quantity = 15, Total = 150, Transaction_Date = new DateTime(2023, 1, 4), Item = new Item { id = 2, Itemname = "TestItem2", Category_Id = 1, Unit = "TestUnit2", Location = "TestLocation2", Description = "TestDescription2" } },
            }.AsQueryable();


            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.Provider).Returns(transactionData.Provider);
            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.Expression).Returns(transactionData.Expression);
            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.ElementType).Returns(transactionData.ElementType);
            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.GetEnumerator()).Returns(transactionData.GetEnumerator());

            mockContext.Setup(m => m.Transactions).Returns(mockTransactions.Object);

            Globals.wareMasterEntities = mockContext.Object;

            Item item = new Item { id = 1, Itemname = "TestItem" };
            DateTime dateBegin = new DateTime(2022, 12, 31);
            DateTime dateEnd = new DateTime(2023, 1, 3);

            List<InventoryData> result = Inventory.GetOutboundsByItem(item, dateBegin, dateEnd);

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(1, result[0].id);
            Assert.AreEqual(item.Itemname, result[0].Name);
            Assert.AreEqual(-10, result[0].Quantity);
            Assert.AreEqual(-100, result[0].Total);
            Assert.AreEqual(new DateTime(2023, 1, 1), result[0].Date);

            Assert.AreEqual(3, result[1].id);
            Assert.AreEqual(item.Itemname, result[1].Name);
            Assert.AreEqual(-20, result[1].Quantity);
            Assert.AreEqual(-200, result[1].Total);
            Assert.AreEqual(new DateTime(2023, 1, 3), result[1].Date);

            dateBegin = new DateTime(2023, 1, 1);
            result = Inventory.GetInboundsByItem(item, dateBegin, dateEnd);
            Assert.AreEqual(1, result.Count);
        }
        [TestMethod]
        public void TestGetInboundsByCategory()
        {
            var mockTransactions = new Mock<DbSet<Transaction>>();
            var mockContext = new Mock<WareMasterEntities>();
            var transactionData = new List<Transaction>
            {
                new Transaction { id = 1, Item_Id = 1, Quantity = -10, Total = -100, Transaction_Date = new DateTime(2023, 1, 1), Item = new Item { id = 1, Itemname = "TestItem1", Category_Id = 1, Unit = "TestUnit1", Location = "TestLocation1", Description = "TestDescription1" } },
                new Transaction { id = 2, Item_Id = 1, Quantity = 5, Total = 50, Transaction_Date = new DateTime(2023, 1, 2), Item = new Item { id = 1, Itemname = "TestItem1", Category_Id = 1, Unit = "TestUnit1", Location = "TestLocation1", Description = "TestDescription1" } },
                new Transaction { id = 3, Item_Id = 1, Quantity = 20, Total = 200, Transaction_Date = new DateTime(2023, 1, 3), Item = new Item { id = 1, Itemname = "TestItem1", Category_Id = 1, Unit = "TestUnit1", Location = "TestLocation1", Description = "TestDescription1" } },
                new Transaction { id = 4, Item_Id = 2, Quantity = 15, Total = 150, Transaction_Date = new DateTime(2023, 1, 4), Item = new Item { id = 2, Itemname = "TestItem2", Category_Id = 1, Unit = "TestUnit2", Location = "TestLocation2", Description = "TestDescription2" } },
            }.AsQueryable();

            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.Provider).Returns(transactionData.Provider);
            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.Expression).Returns(transactionData.Expression);
            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.ElementType).Returns(transactionData.ElementType);
            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.GetEnumerator()).Returns(transactionData.GetEnumerator());

            mockContext.Setup(m => m.Transactions).Returns(mockTransactions.Object);

            Globals.wareMasterEntities = mockContext.Object;

            Category category = new Category { id = 1, Category_Name = "TestCategory" };
            DateTime dateBegin = new DateTime(2022, 12, 31);
            DateTime dateEnd = new DateTime(2023, 1, 3);

            List<InventoryData> result = Inventory.GetInboundsByCategory(category, dateBegin, dateEnd);
            Assert.AreEqual(2, result.Count); 
            Assert.AreEqual(2, result[0].id);
            Assert.AreEqual("TestItem1", result[0].Name);
            Assert.AreEqual(5, result[0].Quantity);
            Assert.AreEqual(50, result[0].Total);
            Assert.AreEqual(new DateTime(2023, 1, 2), result[0].Date);

            Assert.AreEqual(3, result[1].id);
            Assert.AreEqual("TestItem1", result[1].Name);
            Assert.AreEqual(20, result[1].Quantity);
            Assert.AreEqual(200, result[1].Total);
            Assert.AreEqual(new DateTime(2023, 1, 3), result[1].Date);
        }
        [TestMethod]
        public void TestGetOutboundsByCategory()
        {
            var mockTransactions = new Mock<DbSet<Transaction>>();
            var mockContext = new Mock<WareMasterEntities>();
            var transactionData = new List<Transaction>
            {
                new Transaction { id = 1, Item_Id = 1, Quantity = 10, Total = 100, Transaction_Date = new DateTime(2023, 1, 1), Item = new Item { id = 1, Itemname = "TestItem1", Category_Id = 1, Unit = "TestUnit1", Location = "TestLocation1", Description = "TestDescription1" } },
                new Transaction { id = 2, Item_Id = 1, Quantity = -5, Total = -50, Transaction_Date = new DateTime(2023, 1, 2), Item = new Item { id = 1, Itemname = "TestItem1", Category_Id = 1, Unit = "TestUnit1", Location = "TestLocation1", Description = "TestDescription1" } },
                new Transaction { id = 3, Item_Id = 1, Quantity = -20, Total = -200, Transaction_Date = new DateTime(2023, 1, 3), Item = new Item { id = 1, Itemname = "TestItem1", Category_Id = 1, Unit = "TestUnit1", Location = "TestLocation1", Description = "TestDescription1" } },
                new Transaction { id = 4, Item_Id = 2, Quantity = -15, Total = -150, Transaction_Date = new DateTime(2023, 1, 4), Item = new Item { id = 2, Itemname = "TestItem2", Category_Id = 1, Unit = "TestUnit2", Location = "TestLocation2", Description = "TestDescription2" } },
            }.AsQueryable();

            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.Provider).Returns(transactionData.Provider);
            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.Expression).Returns(transactionData.Expression);
            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.ElementType).Returns(transactionData.ElementType);
            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.GetEnumerator()).Returns(transactionData.GetEnumerator());

            mockContext.Setup(m => m.Transactions).Returns(mockTransactions.Object);

            Globals.wareMasterEntities = mockContext.Object;

            Category category = new Category { id = 1, Category_Name = "TestCategory" };
            DateTime dateBegin = new DateTime(2022, 12, 31);
            DateTime dateEnd = new DateTime(2023, 1, 3);

            List<InventoryData> result = Inventory.GetOutboundsByCategory(category, dateBegin, dateEnd);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result[0].id);
            Assert.AreEqual("TestItem1", result[0].Name);
            Assert.AreEqual(-5, result[0].Quantity);
            Assert.AreEqual(-50, result[0].Total);
            Assert.AreEqual(new DateTime(2023, 1, 2), result[0].Date);

            Assert.AreEqual(3, result[1].id);
            Assert.AreEqual("TestItem1", result[1].Name);
            Assert.AreEqual(-20, result[1].Quantity);
            Assert.AreEqual(-200, result[1].Total);
            Assert.AreEqual(new DateTime(2023, 1, 3), result[1].Date);
        }
        [TestMethod]
        public void TestGetInventoryChangeDetailsByItem()
        {
            var mockTransactions = new Mock<DbSet<Transaction>>();
            var mockContext = new Mock<WareMasterEntities>();
            var transactionData = new List<Transaction>
            {
                new Transaction { id = 1, Item_Id = 1, Quantity = 10, Total = 100, Transaction_Date = new DateTime(2023, 1, 1), Item = new Item { id = 1, Itemname = "TestItem1", Category_Id = 1, Unit = "TestUnit1", Location = "TestLocation1", Description = "TestDescription1" } },
                new Transaction { id = 2, Item_Id = 1, Quantity = -5, Total = -50, Transaction_Date = new DateTime(2023, 1, 2), Item = new Item { id = 1, Itemname = "TestItem1", Category_Id = 1, Unit = "TestUnit1", Location = "TestLocation1", Description = "TestDescription1" } },
                new Transaction { id = 3, Item_Id = 1, Quantity = -20, Total = -200, Transaction_Date = new DateTime(2023, 1, 3), Item = new Item { id = 1, Itemname = "TestItem1", Category_Id = 1, Unit = "TestUnit1", Location = "TestLocation1", Description = "TestDescription1" } },
                new Transaction { id = 4, Item_Id = 2, Quantity = -15, Total = -150, Transaction_Date = new DateTime(2023, 1, 4), Item = new Item { id = 2, Itemname = "TestItem2", Category_Id = 1, Unit = "TestUnit2", Location = "TestLocation2", Description = "TestDescription2" } },
            }.AsQueryable();

            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.Provider).Returns(transactionData.Provider);
            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.Expression).Returns(transactionData.Expression);
            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.ElementType).Returns(transactionData.ElementType);
            mockTransactions.As<IQueryable<Transaction>>().Setup(m => m.GetEnumerator()).Returns(transactionData.GetEnumerator());

            mockContext.Setup(m => m.Transactions).Returns(mockTransactions.Object);
            Globals.wareMasterEntities = mockContext.Object;

            Item item1 = new Item { id = 1, Itemname = "TestItem1", Category_Id = 1, Unit = "TestUnit1", Location = "TestLocation1", Description = "TestDescription1" };
            var result = Inventory.GetInventoryChangeDetailsByItem(item1, new DateTime(2022, 12, 31), new DateTime(2023, 1, 3));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }

    }
}
