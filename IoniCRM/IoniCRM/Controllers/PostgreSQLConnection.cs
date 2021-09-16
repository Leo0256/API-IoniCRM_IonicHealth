using System;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace IoniCRM.Controllers
{
    public class PostgreSQLConnection
    {
        readonly private string connString = String.Format(
            "Server={0}; Database={1}; User id={2}; Password={3}; SslMode=Require; Trust Server Certificate=true",
            "", /* Host */
            "", /*  DataBase  */
            "", /* User */
            ""); /* Pass */


        NpgsqlConnection conn;

        public PostgreSQLConnection() => conn = new NpgsqlConnection(connString);

        public DataTable ExecuteCmd(string query)
        {
            DataTable table = new();

            try
            {
                using (conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    using NpgsqlDataAdapter Adpt = new(query, conn);
                    Adpt.Fill(table);
                }
            }
            catch (Exception ex)
            {
                /*Mensagem de erro : ("Erro na Execução: " + ex.Message);*/
            }
            finally
            {
                conn.Close();
            }

            return table;
        }

        public void Open()
        {
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Close();
                /*Mensagem de erro : ("Erro: " + ex.Message);*/
            }
        }

        public void Close() => conn.Close();
    }
}
