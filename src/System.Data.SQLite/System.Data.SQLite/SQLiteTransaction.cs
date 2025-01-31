﻿/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 * 
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace System.Data.SQLite
{
  using System;
  using System.Data;
  using System.Data.Common;
  using System.Threading;

  /// <summary>
  /// SQLite implementation of DbTransaction.
  /// </summary>
  public sealed class SQLiteTransaction : DbTransaction
  {
    /// <summary>
    /// The connection to which this transaction is bound
    /// </summary>
    internal SQLiteConnection _cnn;
    internal long _version; // Matches the version of the connection
    private IsolationLevel _level;

    /// <summary>
    /// Constructs the transaction object, binding it to the supplied connection
    /// </summary>
    /// <param name="connection">The connection to open a transaction on</param>
    /// <param name="deferredLock">TRUE to defer the writelock, or FALSE to lock immediately</param>
    internal SQLiteTransaction(SQLiteConnection connection, bool deferredLock)
    {
      _cnn = connection;
      _version = _cnn._version;

      _level = (deferredLock == true) ? IsolationLevel.ReadCommitted : IsolationLevel.Serializable;

      if (_cnn._transactionLevel++ == 0)
      {
        try
        {
          using (SQLiteCommand cmd = _cnn.CreateCommand())
          {
            if (!deferredLock)
              cmd.CommandText = "BEGIN IMMEDIATE";
            else
              cmd.CommandText = "BEGIN";

            cmd.ExecuteNonQuery();
          }
        }
        catch (SQLiteException)
        {
          _cnn._transactionLevel--;
          _cnn = null;
          throw;
        }
      }
    }

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    public override void Commit()
    {
      IsValid(true);

      if (_cnn._transactionLevel - 1 == 0)
      {
        using (SQLiteCommand cmd = _cnn.CreateCommand())
        {
          cmd.CommandText = "COMMIT";
          cmd.ExecuteNonQuery();
        }
      }
      _cnn._transactionLevel--;
      _cnn = null;
    }

    /// <summary>
    /// Returns the underlying connection to which this transaction applies.
    /// </summary>
    public new SQLiteConnection Connection
    {
      get { return _cnn; }
    }

    /// <summary>
    /// Forwards to the local Connection property
    /// </summary>
    protected override DbConnection DbConnection
    {
      get { return Connection; }
    }

    /// <summary>
    /// Disposes the transaction.  If it is currently active, any changes are rolled back.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (IsValid(false))
        {
          IssueRollback();
        }
      }
      base.Dispose(disposing);
    }

    /// <summary>
    /// Gets the isolation level of the transaction.  SQLite only supports Serializable transactions.
    /// </summary>
    public override IsolationLevel IsolationLevel
    {
      get { return _level; }
    }

    /// <summary>
    /// Rolls back the active transaction.
    /// </summary>
    public override void Rollback()
    {
      IsValid(true);
      IssueRollback();
    }

    internal void IssueRollback()
    {
      SQLiteConnection cnn = Interlocked.Exchange(ref _cnn, null);

      if (cnn != null)
      {
        using (SQLiteCommand cmd = cnn.CreateCommand())
        {
          cmd.CommandText = "ROLLBACK";
          cmd.ExecuteNonQuery();
        }
        cnn._transactionLevel = 0;
      }
    }

    internal bool IsValid(bool throwError)
    {
      if (_cnn == null)
      {
        if (throwError == true) throw new ArgumentNullException("No connection associated with this transaction");
        else return false;
      }

      if (_cnn._version != _version)
      {
        if (throwError == true) throw new SQLiteException((int)SQLiteErrorCode.Misuse, "The connection was closed and re-opened, changes were already rolled back");
        else return false;
      }
      if (_cnn.State != ConnectionState.Open)
      {
        if (throwError == true) throw new SQLiteException((int)SQLiteErrorCode.Misuse, "Connection was closed");
        else return false;
      }

      if (_cnn._transactionLevel == 0 || _cnn._sql.AutoCommit == true)
      {
        _cnn._transactionLevel = 0; // Make sure the transaction level is reset before returning
        if (throwError == true) throw new SQLiteException((int)SQLiteErrorCode.Misuse, "No transaction is active on this connection");
        else return false;
      }

      return true;
    }
  }
}
