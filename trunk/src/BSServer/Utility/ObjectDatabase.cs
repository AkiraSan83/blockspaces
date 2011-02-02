using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ninject.Extensions.Logging;
using System.Runtime.Serialization;

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
        Guid GetObjectsUniqueId(object obj);
    }
    public class ObjectDatabase : IObjectDatabase, IDisposable
    {

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

        public IEnumerable<IDatabaseRecord<T>> GetAll<T>()
        {
            throw new NotImplementedException();
        }

        public IDatabaseRecord<T> Get<T>()
        {
            throw new NotImplementedException();
        }

        public IDatabaseRecord<T> Get<T>(Guid uniqueId)
        {
            throw new NotImplementedException();
        }

        public IDatabaseRecord<T> Save<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDatabaseRecord<T>> SaveAll<T>(IEnumerable<T> obj)
        {
            throw new NotImplementedException();
        }

        public Guid GetObjectsUniqueId(object obj)
        {
            throw new NotImplementedException();
        }
    }
}