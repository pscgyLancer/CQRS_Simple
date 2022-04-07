using System;
using System.Data;
using System.Data.SqlClient;
using MySqlConnector;

namespace CQRS_Simple.Infrastructure.Dapper
{
    public class SqlConnectionFactory : ISqlConnectionFactory, IDisposable
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public SqlConnectionFactory(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public IDbConnection GetOpenConnection()
        {
            if (this._connection == null || this._connection.State != ConnectionState.Open)
            {
                //this._connection = new SqlConnection(_connectionString);
                this._connection = new MySqlConnection(_connectionString)
                {
                    Site = null,
                    ProvideClientCertificatesCallback = null,
                    ProvidePasswordCallback = null,
                    RemoteCertificateValidationCallback = null
                };
                this._connection.Open();
            }

            return this._connection;
        }

        public void Dispose()
        {
            if (this._connection != null && this._connection.State == ConnectionState.Open)
            {
                this._connection.Dispose();
            }
        }
    }
}