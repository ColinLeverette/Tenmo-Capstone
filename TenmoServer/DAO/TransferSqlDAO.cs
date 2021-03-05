using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string connectionString;

        public TransferSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }


        public int InsertTransfer(Transfer transferToAdd)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // HARD CODING NUMBERS FOR NOW, maybe refactor
                SqlCommand cmd = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES (@transferTypeId, @transferStatusId, @accountFrom, @AccountTo, @Amount); @@identity;", conn);
                cmd.Parameters.AddWithValue("@transferTypeId", transferToAdd.TransferTypeId);
                cmd.Parameters.AddWithValue("@transferStatus", transferToAdd.TransferStatusId);
                cmd.Parameters.AddWithValue("@accountFrom", transferToAdd.AccountFrom);
                cmd.Parameters.AddWithValue("@accountTo", transferToAdd.AccountTo);
                cmd.Parameters.AddWithValue("@amount", transferToAdd.Amount);

                int newId = Convert.ToInt32(cmd.ExecuteScalar());
                return newId;

            }

        }
        private Transfer RowToObject(SqlDataReader rdr)
        {
            Transfer transfer = new Transfer();

            transfer.TransferId = Convert.ToInt32(rdr["transfer_id"]);
            transfer.TransferTypeId = Convert.ToInt32(rdr["transfer_type_id"]);
            transfer.TransferStatusId = Convert.ToInt32(rdr["transfer_status_id"]);
            transfer.AccountFrom = Convert.ToInt32(rdr["account_from"]);
            transfer.AccountTo = Convert.ToInt32(rdr["account_to"]);
            transfer.Amount = Convert.ToDecimal(rdr["amount"]);


            return transfer;
        }
    }
}