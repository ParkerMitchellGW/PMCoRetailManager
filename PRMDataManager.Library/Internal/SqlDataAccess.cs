using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    public class SqlDataAccess : IDisposable, ISqlDataAccess
    {
        public SqlDataAccess(IConfiguration config, ILogger<SqlDataAccess> logger)
        {
            _config = config;
            _logger = logger;
        }

        public string GetConnectionString(string name)
        {
            return _config.GetConnectionString(name);
            //return @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PRMData;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            //return ConfigurationManager.ConnectionStrings[name].ConnectionString;
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

            _isClosed = false;

        }
        private bool _isClosed = false;
        private readonly IConfiguration _config;
        private readonly ILogger<SqlDataAccess> _logger;

        // Close connection/stop transaction method
        public void CommitTransaction()
        {
            _transaction?.Commit();
            _connection?.Close();

            _isClosed = true;
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _connection?.Close();

            _isClosed = true;
        }

        // Dispose
        public void Dispose()
        {
            if (_isClosed == false)
            {
                try
                {
                    CommitTransaction();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Commit transaction failed in the dispose method.");
                }
            }

            _transaction = null;
            _connection = null;
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
