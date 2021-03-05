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

        [HttpPost("{newTransfer}")]
        public int InsertTransfer(Transfer newTransfer)
        {
            return transferDAO.InsertTransfer(newTransfer);
        }



    }
}
