using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMDataManager.Library.Internal
{
    internal class SqlDataAccess : IDisposable
    {
        public string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using IDbConnection connection = new SqlConnection(connectionString);
            List<T> rows = connection.Query<T>(
                storedProcedure, parameters, commandType: CommandType.StoredProcedure).ToList();
            return rows;
        }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using IDbConnection connection = new SqlConnection(connectionString);
            connection.Execute(
                storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        private IDbConnection _connection;
        private IDbTransaction _transaction;
        // Open connect/start transaction method
        public void StartTransaction(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            _connection = new SqlConnection(connectionString);
            _connection.Open();

            _transaction = _connection.BeginTransaction();

        }

        // Close connection/stop transaction method
        public void CommitTransaction()
        {
            if(_transaction != null)
            {
                _transaction?.Commit();
                _transaction?.Dispose();
                _transaction = null;
            }
            _connection?.Close();
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _connection?.Close();
        }

        // Dispose
        public void Dispose()
        {
            CommitTransaction();
        }

        // Load using transaction
        // Save using transaction
        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            List<T> rows = _connection.Query<T>(
                storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction).ToList();
            return rows;
        }

        public void SaveDataInTransaction<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            _connection.Execute(
                storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);
        }
    }

}
