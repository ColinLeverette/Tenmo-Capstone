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


        public Transfer InsertTransfer(Transfer newTransfer)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // HARD CODING NUMBERS FOR NOW, maybe refactor
                SqlCommand cmd = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES (@transferTypeId, @transferStatusId, @accountFrom, @AccountTo, @Amount); select @@identity;", conn);
                cmd.Parameters.AddWithValue("@transferTypeId", newTransfer.TransferTypeId);
                cmd.Parameters.AddWithValue("@transferStatusId", newTransfer.TransferStatusId);
                cmd.Parameters.AddWithValue("@accountFrom", newTransfer.AccountFrom);
                cmd.Parameters.AddWithValue("@accountTo", newTransfer.AccountTo);
                cmd.Parameters.AddWithValue("@amount", newTransfer.Amount);

                
                int newId = Convert.ToInt32(cmd.ExecuteScalar());
                newTransfer.TransferId = newId;
                return newTransfer;
                
            }
        }

        public List<Transfer> GetMyTransfers(int accountUserId)
        {
            List<Transfer> listOfTransfers = new List<Transfer>();
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select * from transfers where account_from = @accountId or account_to = @accountId;", conn);
                cmd.Parameters.AddWithValue("@accountId", accountUserId);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    listOfTransfers.Add(RowToObject(rdr));
                }
            }
            return listOfTransfers;
        }

        public Transfer GetTransferById(int transferId)
        {
            Transfer transfer = new Transfer();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select * from transfers where transfer_id = @transferId;", conn);
                cmd.Parameters.AddWithValue("@transferId", transferId);
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    transfer = RowToObject(rdr);
                }
            }
            return transfer;
        }


        public List<Transfer> GetMyTransferRequests(int accountUserId)
        {
            List<Transfer> listOfTransfers = new List<Transfer>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select * from transfers where account_to = @accountId;", conn);
                cmd.Parameters.AddWithValue("@accountId", accountUserId);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    listOfTransfers.Add(RowToObject(rdr));
                }
            }
            return listOfTransfers;
        }


        private Transfer RowToObject(SqlDataReader rdr)
        {
            Transfer transfer = new Transfer();

            transfer.TransferId = Convert.ToInt32(rdr["transfer_id"]);
            transfer.TransferTypeId = (TransferType)Convert.ToInt32(rdr["transfer_type_id"]);
            transfer.TransferStatusId = (TransferStatus)Convert.ToInt32(rdr["transfer_status_id"]);
            transfer.AccountFrom = Convert.ToInt32(rdr["account_from"]);
            transfer.AccountTo = Convert.ToInt32(rdr["account_to"]);
            transfer.Amount = Convert.ToDecimal(rdr["amount"]);


            return transfer;
        }
    }
}