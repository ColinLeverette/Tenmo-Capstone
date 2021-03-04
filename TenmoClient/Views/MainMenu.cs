using AuctionApp.Exceptions;
using MenuFramework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TenmoClient.Data;
using TenmoClient.Models;

namespace TenmoClient.Views
{
    public class MainMenu : ConsoleMenu
    {
    
        private readonly static string API_ACCOUNT_URL = "https://localhost:44315/accounts/";
        private readonly IRestClient client = new RestClient();

        public MainMenu()
        {
            AddOption("View your current balance", ViewBalance)
                .AddOption("View your past transfers", ViewTransfers)
                .AddOption("View your pending requests", ViewRequests)
                .AddOption("Send TE bucks", SendTEBucks)
                .AddOption("Request TE bucks", RequestTEBucks)
                .AddOption("Log in as different user", Logout)
                .AddOption("Exit", Exit);
        }

        protected override void OnBeforeShow()
        {
            Console.WriteLine($"TE Account Menu for User: {UserService.GetUserName()}");
        }

        //Ask about authorization
        private MenuOptionResult ViewBalance()
        {
            int userId = UserService.GetUserId();
            RestRequest request = new RestRequest(API_ACCOUNT_URL + userId);
            IRestResponse<Account> response = client.Get<Account>(request);
            Account account = null;

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                account = response.Data;
            }

            Console.WriteLine($"Your current account balance is: {account.Balance:c}");
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult ViewTransfers()
        {
            Console.WriteLine("Not yet implemented!");
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult ViewRequests()
        {
            Console.WriteLine("Not yet implemented!");
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        // 4.1: Show a list of users to send TE Bucks to
        private MenuOptionResult SendTEBucks()
        {
            RestRequest request = new RestRequest(API_ACCOUNT_URL);
            IRestResponse<List<Account>> response = client.Get<List<Account>>(request);
            List<Account> account = new List<Account>();

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                account = response.Data;
            }

            Console.WriteLine("------------------------------------");
            Console.WriteLine("Users ID           Name");
            Console.WriteLine("------------------------------------");

            foreach (Account acct in account)
            {
                Console.WriteLine($"{acct.AccountId}");
            }
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult RequestTEBucks()
        {
            Console.WriteLine("Not yet implemented!");
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult Logout()
        {
            UserService.SetLogin(new API_User()); //wipe out previous login info
            return MenuOptionResult.CloseMenuAfterSelection;
        }

        public void ProcessErrorResponse(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new NoResponseException("Error occurred - unable to reach server.");
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException("Unauthorized to access");
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new ForbiddenException("Forbidden from accessing");
            }
            else if (!response.IsSuccessful)
            {
                throw new NonSuccessException();
            }
        }

    }
}
