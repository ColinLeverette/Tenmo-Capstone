using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDAO : IAccountDAO
    {
        private string connectionString;

        public AccountSqlDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Account GetAccount(int userId)
        {
            Account account = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("Select * from accounts where user_Id = @userId", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    account = RowToObject(rdr);
                }

                return account;

            }
        }

        public List<Account> GetAllAccounts()
        {
            List<Account> listOfAccounts = new List<Account>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("Select * from accounts", conn);
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    listOfAccounts.Add(RowToObject(rdr));
                }
            }
            return listOfAccounts;
        }

        public void TransferBucks(int senderAcctId, int recipientAcctId, decimal senderBalance, decimal recipientBalance, decimal transferAmount)
        {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("Begin transaction Update accounts Set balance = @senderBalance where account_id = @senderId;  " +
                        "update accounts set balance = @recipientBalance where account_id = @recipientId Commit transaction", conn);
      
                    cmd.Parameters.AddWithValue("@senderBalance", senderBalance - transferAmount);
                    cmd.Parameters.AddWithValue("@senderId", senderAcctId);
                    cmd.Parameters.AddWithValue("@recipientBalance", recipientBalance + transferAmount);
                    cmd.Parameters.AddWithValue("@recipientId", recipientAcctId);
                    SqlDataReader rdr = cmd.ExecuteReader();

                }
        }


        private Account RowToObject(SqlDataReader rdr)
        {
            Account account = new Account();

            account.AccountId = Convert.ToInt32(rdr["account_Id"]);
            account.Balance = Convert.ToDecimal(rdr["balance"]);
            account.UserId = Convert.ToInt32(rdr["user_id"]);

            return account;
        }

    }
}
