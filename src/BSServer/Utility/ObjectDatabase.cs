using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ninject.Extensions.Logging;
using System.Runtime.Serialization;
using System.Data.SQLite;
using System.Data;

namespace JollyBit.BS.Core.Utility
{
    public interface IUniquelyIdentifiable
    {
        Guid UniqueId { get; }
    }
    public interface IDatabaseRecord<T> : IUniquelyIdentifiable
    {
        T Value { get; }
    }
    public interface IDatabaseObject<T> : IDatabaseRecord<T>
    {
        T Value { get; set; }
        Guid UniqueId { get; set; }
    }
    public interface IObjectDatabase
    {
        IEnumerable<IDatabaseRecord<T>> GetAll<T>();
        IDatabaseRecord<T> Get<T>();
        IDatabaseRecord<T> Get<T>(Guid uniqueId);
        IDatabaseRecord<T> Save<T>(T obj);
        IEnumerable<IDatabaseRecord<T>> SaveAll<T>(IEnumerable<T> obj);
        Guid? GetObjectsUniqueId(object obj);
    }
    public class ObjectDatabase : IObjectDatabase, IDisposable
    {
        private SQLiteConnection _conn;
        private int executeNonQuery(string queryTxt)
        {
            SQLiteCommand command = _conn.CreateCommand();
            command.CommandText = queryTxt;
            return command.ExecuteNonQuery();
        }
        private DataTable executeQuery(string queryTxt)
        {
            SQLiteCommand command = _conn.CreateCommand();
            SQLiteDataAdapter da = new SQLiteDataAdapter(queryTxt, _conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds.Tables[0];
        }
        public ObjectDatabase(string pathToDatabaseFile, bool isNewDatabase)
        {
            //Connect
            _conn = new SQLiteConnection(string.Format("data source={0}; new={1};", pathToDatabaseFile, isNewDatabase));
            _conn.Open();
            //Ensure 'Objects' table exists. Create it if it does not
            if (_conn.GetSchema("Tables").Select("Table_Name = 'Objects'").Length == 0)
            {
                executeNonQuery(
                    @"-- Original table schema
                        CREATE TABLE [Objects] (
                            [Id] guid PRIMARY KEY NOT NULL,
                            [Type] VARCHAR(300) NOT NULL,
                            [Data] blob NOT NULL
                        );
                        CREATE INDEX [IX_Type] ON [Objects] ([Type]);");
            }
        }
        private DictionaryWithWeaklyReferencedKey<object, Guid> _objToGuid = new DictionaryWithWeaklyReferencedKey<object, Guid>();
        private DictionaryWithWeaklyReferencedValues<Guid, object> _guidToObj = new DictionaryWithWeaklyReferencedValues<Guid, object>();
        public IEnumerable<IDatabaseRecord<T>> GetAll<T>()
        {
            JsonExSerializer.Serializer serializer = new JsonExSerializer.Serializer(typeof(T));
            string query = string.Format("SELECT Id, Data FROM Objects WHERE Type = {0}", typeof(T).FullName);
            DataTable table = executeQuery(query);
            foreach (DataRow item in table.Rows)
            {
                Guid id = new Guid((string)item["Id"]);
                object value;
                if (!_guidToObj.TryGetValue(id, out value))
                {
                    //Object is not in memory and deserialized... deserialize it
                    value = serializer.Deserialize((string)item["Data"]);
                }
                if (value is IDatabaseObject<T>)
                {
                    //value is a DatabaseObject return it
                    IDatabaseObject<T> dataObj = value as IDatabaseObject<T>;
                    dataObj.UniqueId = id;
                    yield return dataObj;
                }
                else //value is not a DatabaseObject... wrap it
                {
                    yield return new DatabaseRecord<T>(id, (T)value);
                }
            }
        }
        public IDatabaseRecord<T> Get<T>()
        {
            return GetAll<T>().FirstOrDefault();
        }
        public IDatabaseRecord<T> Get<T>(Guid uniqueId)
        {
            object value = null;
            //check if already loaded
            if (!_guidToObj.TryGetValue(uniqueId, out value))
            {
                //not already loaded... check database
                string query = string.Format("SELECT Id, Data FROM Objects WHERE Type = {0} AND Id = '{1}'", typeof(T).FullName, uniqueId.ToString());
                DataTable table = executeQuery(query);
                if (table.Rows.Count == 1)
                {
                    JsonExSerializer.Serializer serializer = new JsonExSerializer.Serializer(typeof(T));
                    value = serializer.Deserialize((string)table.Rows[0]["Data"]);
                }
            }
            //Return DatabaseRecord
            if (value == null) return null;
            else if (value is IDatabaseObject<T>)
            {
                IDatabaseObject<T> dataObj = value as IDatabaseObject<T>;
                dataObj.UniqueId = uniqueId;
                return dataObj;
            }
            else return new DatabaseRecord<T>(uniqueId, (T)value);
        }
        public IDatabaseRecord<T> Save<T>(T obj)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<IDatabaseRecord<T>> SaveAll<T>(IEnumerable<T> obj)
        {
            return obj.Select(o => Save<T>(o));
        }
        public Guid? GetObjectsUniqueId(object obj)
        {
            Guid id;
            if (_objToGuid.TryGetValue(obj, out id))
            {
                return id;
            }
            return null;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _conn.Close();
                _disposed = true;
            }
        }
        ~ObjectDatabase()
        {
            Dispose(false);
        }
        protected class DatabaseRecord<T> : IDatabaseRecord<T>
        {
            private readonly T _value;
            private readonly Guid _uniqueId;
            public DatabaseRecord(Guid uniqueId, T value)
            {
                _value = value;
                _uniqueId = uniqueId;
            }
            public T Value
            {
                get { return _value; }
            }
            public Guid UniqueId
            {
                get { return _uniqueId; }
            }
        }
    }
}