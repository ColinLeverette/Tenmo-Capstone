using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        int InsertTransfer(Transfer transferToAdd);
    }
}