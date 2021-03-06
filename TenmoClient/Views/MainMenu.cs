using AuctionApp.Exceptions;
using MenuFramework;
using RestSharp;
using RestSharp.Authenticators;
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
        MainMenuMethods methods = new MainMenuMethods();

        private readonly static string API_TRANSFER_URL = "https://localhost:44315/transfers/";
        private readonly static string API_ACCOUNT_URL = "https://localhost:44315/accounts/";
        private readonly static string API_USERS_URL = "https://localhost:44315/users/";
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

        private MenuOptionResult ViewBalance()
        {
            int userId = UserService.GetUserId();

            Account account = methods.GetUserAccount(userId);
          
            Console.WriteLine($"Your current account balance is: {account.Balance:c}");
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult ViewTransfers()
        {
            string token = UserService.GetToken();
            client.Authenticator = new JwtAuthenticator(token);
            int userId = UserService.GetUserId();
            RestRequest request = new RestRequest(API_TRANSFER_URL + userId);
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            List<Transfer> listOfTransfers = response.Data;

            Account account = methods.GetUserAccount(userId);
            
            Console.WriteLine("------------------------------------");
            Console.WriteLine("Transfer ID            From  /  To           Amount ");

            // loop thru the transfers and CW each transfer in list 
            foreach (Transfer transfer in listOfTransfers)
            {
                //Get From username
                int accountFromAccountId = transfer.AccountFrom;
                User userFrom = methods.GetUser(accountFromAccountId);

                //Get To username
                int accountToAccountId = transfer.AccountTo;
                User userTo = methods.GetUser(accountToAccountId);
               
                //////////////////////////////////////////////////////

                Console.WriteLine($"{transfer.TransferId}              From: {userFrom.Username}  to:  {userTo.Username}      {transfer.Amount}");
            }

            Console.WriteLine("-------------------");
            
            //return result;


            // FIX TRY CATCH TO ONLY ALLOW ENTERING DISPLAYED ITEMS 


            bool gotTransferId = false;
            int transferId = 0;
            while (!gotTransferId)
            {
                try
                {
                    Console.Write("Please enter transfer ID to view details: ");
                     transferId = int.Parse(Console.ReadLine());
                    gotTransferId = true;
                }
                catch
                {
                    Console.WriteLine("You entered an invalid value, please try again");
                }
            }
            // Part 6 - Transfer Details Page

            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Transfer Details");
            Console.WriteLine("------------------------------------------");

            string token2 = UserService.GetToken();
            client.Authenticator = new JwtAuthenticator(token2);

            RestRequest request3 = new RestRequest(API_TRANSFER_URL + "lookup/" + transferId);
            IRestResponse<Transfer> response3 = client.Get<Transfer>(request3);
            Transfer transfer3 = response3.Data;

            //Print out all of the transfer details
            Console.WriteLine($"Id: {transfer3.TransferId}");

            //Get From username
            int accountFromId = transfer3.AccountFrom;
            RestRequest requestAccountFrom = new RestRequest(API_USERS_URL + "account/" + accountFromId);
            IRestResponse<User> response4 = client.Get<User>(requestAccountFrom);
            User usernameFrom = response4.Data;

            Console.WriteLine($"From: {usernameFrom.Username}");

            //Get To username
            int accountToId = transfer3.AccountTo;
            RestRequest requestAccountTo = new RestRequest(API_USERS_URL + "account/" + accountToId);
            IRestResponse<User> response5 = client.Get<User>(requestAccountTo);
            User usernameTo = response5.Data;

            Console.WriteLine($"To: {usernameTo.Username}");
            Console.WriteLine($"Type: {transfer3.TransferTypeId}");
            Console.WriteLine($"Status: {transfer3.TransferStatusId}");
            Console.WriteLine($"Amount: {transfer3.Amount:c}");

            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult ViewRequests()
        {
            string token = UserService.GetToken();
            client.Authenticator = new JwtAuthenticator(token);
            int userId = UserService.GetUserId();
            RestRequest request = new RestRequest(API_TRANSFER_URL + userId);
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            List<Transfer> listOfReceivedTransfers = new List<Transfer>();

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                MainMenuMethods.ProcessErrorResponse(response);
            }
            else
            {
                listOfReceivedTransfers = response.Data;
            }
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        // 4.1: Show a list of users to send TE Bucks to
        private MenuOptionResult SendTEBucks()
        {
            // Part 1 - List out all userIds and userNames
            string token = UserService.GetToken();
            client.Authenticator = new JwtAuthenticator(token);
            RestRequest request = new RestRequest(API_USERS_URL);
            IRestResponse<List<User>> response = client.Get<List<User>>(request);
            List<User> user = new List<User>();
            int userId = UserService.GetUserId();

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                MainMenuMethods.ProcessErrorResponse(response);
            }
            else
            {
                user = response.Data;
            }

            Console.WriteLine("------------------------------------");
            Console.WriteLine("Users ID           Name");
            Console.WriteLine("------------------------------------");

            foreach (User u in user)
            {
                if (u.UserId != userId)
                {
                    Console.WriteLine($"{u.UserId}                   {u.Username}");
                }
            }


            //bool gotTransferId = false;
            //int transferId = 0;
            //while (!gotTransferId)
            //{
            //    try
            //    {
            //        Console.Write("Please enter transfer ID to view details: ");
            //        transferId = int.Parse(Console.ReadLine());
            //        gotTransferId = true;
            //    }
            //    catch
            //    {
            //        Console.WriteLine("You entered an invalid value, please try again");
            //    }
            //}


            // Part 2 - User selects dollar amount and userId to transfer money to
            bool gotRecipient = false;
            int recipientUserId = 0;
            while (!gotRecipient)
            {
                try
                {
                    Console.Write("Please enter the Id of User you'd like to send TE bucks to: ");
                    recipientUserId = int.Parse(Console.ReadLine());
                    gotRecipient = true;
                }
                catch
                {
                    Console.WriteLine("You entered an invalid User ID, please try again");
                }
            }
            

            // FIX TRY CATCH TO ONLY ALLOW DISPLAYED VALUES


            //Recipient's user Id
            bool gotTransferAmount = false;
            decimal transferAmount = 0;
            while (!gotTransferAmount)
            {
                try
                {
                    Console.Write("Please enter the amount: ");
                     transferAmount = decimal.Parse(Console.ReadLine());
                    gotTransferAmount = true;
                }
                catch
                {
                    Console.WriteLine("You entered an invalid amount, please try again");
                }
            }
            
            // Transfer amount
            

            //Get recipient's AccountId - Use GetAccounts method
            RestRequest req = new RestRequest(API_ACCOUNT_URL);
            IRestResponse<List<Account>> res = client.Get<List<Account>>(req);
            List<Account> listOfAccounts = null;

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                MainMenuMethods.ProcessErrorResponse(response);
            }
            else
            {
                listOfAccounts = res.Data;
            }

            int recipientAccountId = 0;
            decimal recipientBalance = 0;
            foreach(Account acct in listOfAccounts)
            {
                if (recipientUserId == acct.UserId)
                {
                    recipientAccountId = acct.AccountId;
                    recipientBalance = acct.Balance;
                }
            }

            //Get current user's id and balance
          Account account = methods.GetUserAccount(userId);

            //**********************************
            decimal senderBalance = account.Balance;
            int senderAccountId = account.AccountId;
            // *********************************

            // Here we are performing the transfer

            //Create a copy of the list
            List<Account> listOfAccts = new List<Account>();


            //1. Create a list of two accounts, which are the sender and recipient
            foreach (Account acct in listOfAccounts)
            {
                if (acct.AccountId == senderAccountId || acct.AccountId == recipientAccountId)
                {
                    listOfAccts.Add(acct);
                }
            }
            if (senderBalance >= transferAmount)
            {
                //2. For the sender account, we have to set account.Balance = senderBalance - transferAmount
                RestRequest rr = new RestRequest(API_ACCOUNT_URL + senderAccountId + "/transfer/" + recipientAccountId + "/" + senderBalance + "/" +
                recipientBalance + "/" + transferAmount);
                rr.AddJsonBody(listOfAccts);
                IRestResponse<List<Account>> re = client.Put<List<Account>>(rr);
                List<Account> result = null;
                decimal updatedBalance = senderBalance - transferAmount;
                Console.WriteLine($"Your updated balance after transaction: {updatedBalance:c} ");
                Console.WriteLine("Thank you for doing business with Tenmo");  // TM? 

                Transfer newTransfer = new Transfer();
                newTransfer.TransferStatusId = (TransferStatus)2;
                newTransfer.TransferTypeId = (TransferType)2;
                newTransfer.AccountTo = recipientAccountId;
                newTransfer.AccountFrom = senderAccountId;
                newTransfer.Amount = transferAmount;

                RestRequest transferRequest = new RestRequest(API_TRANSFER_URL);
                transferRequest.AddJsonBody(newTransfer);
                IRestResponse<Transfer> transferResponse = client.Post<Transfer>(transferRequest);
                Transfer dataFromPost = new Transfer();

                if (transferResponse.ResponseStatus != ResponseStatus.Completed || !transferResponse.IsSuccessful)
                {
                    MainMenuMethods.ProcessErrorResponse(transferResponse);
                }
                else
                {
                    dataFromPost = transferResponse.Data;
                }

            }
            else
            {
                Console.WriteLine("You do not have enough TE Bucks to complete transaction");
            }
           
            //if (re.ResponseStatus != ResponseStatus.Completed || !re.IsSuccessful)
            //{
            //    ProcessErrorResponse(re);
            //}
            //else
            //{
            //    result = re.Data;
            //}

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

        

    }
}
