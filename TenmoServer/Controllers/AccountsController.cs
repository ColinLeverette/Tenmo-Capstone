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
    [Route("accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountDAO accountDAO;

        public AccountsController(IAccountDAO accountDAO)
        {
            this.accountDAO = accountDAO;
        }

        //[Authorize]
        [HttpGet("{userId}")]
        public Account GetAccount(int userId)
        {
            return accountDAO.GetAccount(userId);
        }

        [HttpGet]
        public List<Account> GetAllAccounts()
        {
            return accountDAO.GetAllAccounts();
        }

    }
}
