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
            "ec2-54-158-247-97.compute-1.amazonaws.com", /* Host */
            "d92sq4j455edga", /*  DataBase  */
            "nndzqgrvsuzmke", /* User */
            "a53e6d7023c936e92dfb20d83c73c08fa2a882847fd8439f369802426a4becf8"); /* Pass */


        NpgsqlConnection conn;

        public PostgreSQLConnection() => conn = new NpgsqlConnection(connString);

        public async Task<DataTable> ExecuteCmdAsync(string query)
        {
            DataTable table = new();

            try
            {
                await using (conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    using NpgsqlDataAdapter Adpt = new(query, conn);
                    Adpt.Fill(table);
                }
            }
            catch (Exception ex)
            {
                /*Mensagem de erro : ("Erro na Execução: " + ex.Message);*/
                throw ex;
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
                throw ex;
                /*Mensagem de erro : ("Erro: " + ex.Message);*/
            }
        }

        public void Close() => conn.Close();
    }
}
