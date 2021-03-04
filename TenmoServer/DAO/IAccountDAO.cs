using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDAO
    {
        Account GetAccount(int userId);
        List<Account> GetAllAccounts();
        bool TransferBucks(int senderAcctId, int recipientAcctId, decimal senderBalance, decimal recipientBalance, decimal transferAmount);
    }
}