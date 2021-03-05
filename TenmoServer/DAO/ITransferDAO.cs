using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        int InsertTransfer(Transfer newTransfer);
        List<Transfer> GetMyTransfers(int accountUserId);

        List<Transfer> GetMyTransferRequests(int accountUserId);
    }
}