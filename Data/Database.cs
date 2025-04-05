using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace WebAppCS.Data
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public DataTable EjecutarConsulta(string query)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        public void EjecutarComando(string query)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
