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
    [Route("users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private IUserDAO userDAO;

        public UsersController(IUserDAO userDAO)
        {
            this.userDAO = userDAO;
        }

        [HttpGet]
        public List<User> GetAllUsers()
        {
            return userDAO.GetUsers();
        }

        [HttpGet("account/{accountId}")]
        public User GetUsername(int accountId)
        {
            return userDAO.GetUsername(accountId);
        }

    }
}

