using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        Transfer InsertTransfer(Transfer newTransfer);
        List<Transfer> GetMyTransfers(int userId);
        Transfer GetTransferById(int transferId);

    }
}