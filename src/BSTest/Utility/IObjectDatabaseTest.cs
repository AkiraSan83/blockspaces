using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using JollyBit.BS.Core;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Server.Utility;
using JollyBit.BS.Server;
using Ninject;
using System.Reflection;
using System.IO;

namespace JollyBit.BS.Test.Utility
{
    [TestFixture]
    public class SQLiteObjectDatabaseTest
    {
        private class TestObj
        {
            public TestObj() { }
            public TestObj(string value1, int value2)
            {
                Value1 = value1;
                Value2 = value2;
            }
            public string Value1;
            public int Value2;
        }

        private IObjectDatabase database;
        private string databaseFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "temp.sqlite";
        [SetUp]
        public void Setup()
        {
            BS.Core.Constants.Kernel = new StandardKernel(new BSServerNinjectModule());
            database = new SQLiteObjectDatabase(databaseFile);
        }
        [TearDown]
        public void TearDown()
        {
            database.Dispose();
            File.Delete(databaseFile);
        }
        [Test]
        public void TestGetAll()
        {
            List<TestObj> testObjs = new List<TestObj>();
            for (int i = 0; i < 10; i++)
            {
                testObjs.Add(new TestObj(i.ToString(), i)); 
            }
            var records = database.SaveAll(testObjs);
            foreach (var record in records)
            {
                Assert.IsTrue(testObjs.Contains(record.Value), "IObjectDatabase.SaveAll failed");
            }
            records = database.GetAll<TestObj>();
            foreach (var record in records)
            {
                Assert.IsTrue(testObjs.Contains(record.Value), "IObjectDatabase.GetAll failed to return an object from memory");
            }
            database.Dispose();
            database = new SQLiteObjectDatabase(databaseFile);
            var newRecords = database.GetAll<TestObj>();
            foreach (var record in records)
            {
                Assert.IsTrue(records.FirstOrDefault(i => newRecords.Select(g => g.UniqueId).Contains(i.UniqueId)) != null, "IObjectDatabase.GetAll failed to return an object from storage");
            }
        }
        [Test]
        public void TestGetGUID()
        {
            var testObj = new TestObj("This is a simple test hope shit does not go wrong!!", 666);
            var record = database.Save(testObj);
            Assert.IsTrue(Object.ReferenceEquals(testObj, record.Value), "IObjectDatabase.Save failed to return the same object that was passed to it in the returned record");
            record = database.Get<TestObj>(record.UniqueId);
            Assert.IsTrue(Object.ReferenceEquals(testObj, record.Value), "IObjectDatabase.Get(GUID) failed to return an object from memory");
            database.Dispose();
            database = new SQLiteObjectDatabase(databaseFile);
            var newRecord = database.Get<TestObj>(record.UniqueId);
            Assert.IsTrue(record.UniqueId == newRecord.UniqueId && testObj.Value1 == newRecord.Value.Value1 && testObj.Value2 == newRecord.Value.Value2, "IObjectDatabase.Get(GUID) failed to return an object from storage");
        }
        [Test]
        public void TestGet()
        {
            var testObj = new TestObj("This is a simple test hope shit does not go wrong!!", 666);
            var record = database.Save(testObj);
            Assert.IsTrue(Object.ReferenceEquals(testObj, record.Value), "IObjectDatabase.Save failed to return the same object that was passed to it in the returned record");
            record = database.Get<TestObj>();
            Assert.IsTrue(Object.ReferenceEquals(testObj, record.Value), "IObjectDatabase.Get() failed to return an object from memory");
            database.Dispose();
            database = new SQLiteObjectDatabase(databaseFile);
            var newRecord = database.Get<TestObj>();
            Assert.IsTrue(record.UniqueId == newRecord.UniqueId && testObj.Value1 == newRecord.Value.Value1 && testObj.Value2 == newRecord.Value.Value2, "IObjectDatabase.Get() failed to return an object from storage");
        }
        [Test]
        public void TestDelete()
        {
            var testObj = new TestObj("This is a simple test hope shit does not go wrong!!", 666);
            var record = database.Save(testObj);
            Assert.IsTrue(Object.ReferenceEquals(testObj, record.Value), "IObjectDatabase.Save failed to return the same object that was passed to it in the returned record");
            database.Delete(record.Value);
            record = database.Get<TestObj>();
            Assert.IsTrue(record == null, "IObjectDatabase.Delete() failed to delete an object from memory");
            database.Dispose();
            database = new SQLiteObjectDatabase(databaseFile);
            record = database.Get<TestObj>();
            Assert.IsTrue(record == null, "IObjectDatabase.Delete() failed to delete an object from storage");
        }
        [Test]
        public void TestGetObjectsUniqueId()
        {
            var testObj = new TestObj("This is a simple test hope shit does not go wrong!!", 666);
            var record = database.Save(testObj);
            Assert.IsTrue(Object.ReferenceEquals(testObj, record.Value), "IObjectDatabase.Save failed to return the same object that was passed to it in the returned record");
            Guid? id = database.GetObjectsUniqueId(record.Value);
            Assert.IsTrue(id != null, "IObjectDatabase.GetObjectsUniqueId failed");
            Assert.IsTrue(id.Value == record.UniqueId, "IObjectDatabase.GetObjectsUniqueId failed");
        }
    }
}
