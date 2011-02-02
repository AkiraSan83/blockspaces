using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ninject.Extensions.Logging;
using System.Runtime.Serialization;
using System.Data.SQLite;
using System.Data;

namespace JollyBit.BS.Server.Utility
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
        public ObjectDatabase(string pathToDatabaseFile, bool isNewDatabase)
        {
            //Connect
            _conn = new SQLiteConnection(string.Format("data source={0}; new={1};", pathToDatabaseFile, isNewDatabase));
            _conn.Open();
            //Ensure 'Objects' table exists. Create it if it does not
            if (_conn.GetSchema("Tables").Select("Table_Name = 'Objects'").Length == 0)
            {
                _conn.CreateCommand(
                    @"-- Original table schema
                        CREATE TABLE [Objects] (
                            [Id] guid PRIMARY KEY NOT NULL,
                            [Type] VARCHAR(300) NOT NULL,
                            [Data] blob NOT NULL
                        );
                        CREATE INDEX [IX_Type] ON [Objects] ([Type]);")
                .ExecuteNonQuery();
            }
        }
        private DictionaryWithWeaklyReferencedKey<object, Guid> _objToGuid = new DictionaryWithWeaklyReferencedKey<object, Guid>();
        private DictionaryWithWeaklyReferencedValues<Guid, object> _guidToObj = new DictionaryWithWeaklyReferencedValues<Guid, object>();
        public IEnumerable<IDatabaseRecord<T>> GetAll<T>()
        {
            JsonExSerializer.Serializer serializer = new JsonExSerializer.Serializer(typeof(T));
            string query = string.Format("SELECT Id, Data FROM Objects WHERE Type = {0}", typeof(T).FullName);
            DataTable table = _conn.CreateCommand("SELECT Id, Data FROM Objects WHERE Type = @Type")
                .AddParm("@Type", typeof(T).FullName)
                .ExecuteQuery().Tables[0];
            foreach (DataRow item in table.Rows)
            {
                Guid id = new Guid((string)item["Id"]);
                object value;
                if (!_guidToObj.TryGetValue(id, out value))
                {
                    //Object is not in memory and deserialized... deserialize it and add it to dicts
                    value = serializer.Deserialize((string)item["Data"]);
                    _guidToObj.Add(id, value);
                    _objToGuid.Add(value, id);
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
                DataTable table = _conn.CreateCommand("SELECT Id, Data FROM Objects WHERE Type = @Type AND Id = @Id")
                    .AddParm("@Type", typeof(T).FullName)
                    .AddParm("@Id", uniqueId)
                    .ExecuteQuery().Tables[0];
                if (table.Rows.Count == 1)
                {
                    JsonExSerializer.Serializer serializer = new JsonExSerializer.Serializer(typeof(T));
                    value = serializer.Deserialize((string)table.Rows[0]["Data"]);
                    _guidToObj.Add(uniqueId, value);
                    _objToGuid.Add(value, uniqueId);
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
            if (obj == null) throw new ArgumentException("", "obj");
            Guid uniqueId;
            //Check if already loaded
            if (!_objToGuid.TryGetValue(obj, out uniqueId))
            {
                //Not loaded create new id and add to dicts
                uniqueId = new Guid();
                _objToGuid.Add(obj, uniqueId);
                _guidToObj.Add(uniqueId, obj);
            }
            //Serialize
            JsonExSerializer.Serializer serializer = new JsonExSerializer.Serializer(typeof(T));
            string data = serializer.Serialize(obj);
            //Save it
            _conn.CreateCommand("REPLACE INTO Objects (Id, Type, Data) VALUES (@Id, @Type, @Data)")
                .AddParm("@Id", uniqueId.ToString())
                .AddParm("@Type", typeof(T).FullName)
                .AddParm("@Data", System.Text.Encoding.UTF8.GetBytes(data))
                .ExecuteNonQuery();
            //Return new record
            if (obj is IDatabaseObject<T>)
            {
                IDatabaseObject<T> dataObj = obj as IDatabaseObject<T>;
                dataObj.UniqueId = uniqueId;
                return dataObj;
            }
            else return new DatabaseRecord<T>(uniqueId, (T)obj);
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