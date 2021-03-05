using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("transfers")]
    [ApiController]
    [Authorize]

    public class TransfersController : ControllerBase
    {
        private ITransferDAO transferDAO;

        public TransfersController(ITransferDAO transferDAO)
        {
            this.transferDAO = transferDAO;
        }

        [HttpPost]
        public Transfer InsertTransfer(Transfer newTransfer)
        {
            return transferDAO.InsertTransfer(newTransfer);
        }

       [HttpGet("{userId}")]
        public List<Transfer> GetMyTransfers(int userId)
        {
            return transferDAO.GetMyTransfers(userId);
        }


        //[HttpGet("{accountId}/sent")]
        //public List<Transfer> GetMyTransfersSent(int accountId)
        //{
        //    return transferDAO.GetMyTransfers(accountId);
        //}

        //[HttpGet("{accountId}/received")]
        //public List<Transfer> GetMyTransferRequests(int accountId)
        //{
        //    return transferDAO.GetMyTransferRequests(accountId);
        //}



    }
}
