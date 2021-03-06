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
            //Get userId
            int userId = UserService.GetUserId();

            //Use userId to get the accountId for user
            Account account = methods.GetUserAccount(userId);
          
            //Print out current balance 
            Console.WriteLine($"Your current account balance is: {account.Balance:c}");
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult ViewTransfers()
        {
            //Get userId of logged in user
            int userId = UserService.GetUserId();

            //Use that userId to retrieve a list of transfers where user was either sender or recipient
            List<Transfer> listOfTransfers = methods.GetListOfTransfers(userId);

            //Also user that userId to get the account for that user
            Account account = methods.GetUserAccount(userId);
            
            //Display menu
            Console.WriteLine("------------------------------------");
            Console.WriteLine("Transfer ID            From  /  To           Amount ");

            //Display each transfer in list 
            foreach (Transfer t in listOfTransfers)
            {
                //Get From username
                int accountFromAccountId = t.AccountFrom;
                User userFrom = methods.GetUser(accountFromAccountId);

                //Get To username
                int accountToAccountId = t.AccountTo;
                User userTo = methods.GetUser(accountToAccountId);

                Console.WriteLine($"{t.TransferId}              From: {userFrom.Username}  to:  {userTo.Username}      {t.Amount}");
            }

            Console.WriteLine("");
            Console.WriteLine("-------------------");
            Console.WriteLine("");


            // TRY/CATCH for when users are looking up transfer Ids

            int gotTransferId = 0;
            int transferId = 0;
            int counter = 0;
            int maxAttempts = 3;
            while (gotTransferId != 1)
            {
                try
                {
                    if (counter < maxAttempts)
                    {
                        Console.Write("Please enter transfer ID to view details: ");
                    }
                    transferId = int.Parse(Console.ReadLine());
                    for (int i = 0; i < listOfTransfers.Count; i++)
                    {
                        if (listOfTransfers[i].TransferId == transferId)
                        {
                            gotTransferId = 1;
                        }
                    }

                        if ( (counter < maxAttempts) && (gotTransferId == 0) )
                        {
                            Console.WriteLine("That transfer ID does not exist. Please enter a valid ID.");
                            counter++;
                            Console.WriteLine($"You have {3 - counter} attempts left.");
                        }
                        else if (counter == maxAttempts)
                        {
                            Console.WriteLine("You are being logged out ... (for your own good).");
                            Logout();
                            return MenuOptionResult.ExitAfterSelection;
                    }
                }
                catch
                {
                    if (counter < maxAttempts)
                        {
                            Console.WriteLine("You entered an invalid ID, please try again.");
                            counter++;
                            Console.WriteLine($"You have {3 - counter} attempts left.");
                        }
                    if (counter == maxAttempts)
                    {
                        Console.WriteLine("You are being logged out ... (for your own good).");
                        Logout();
                        return MenuOptionResult.ExitAfterSelection;
                    }
                }
            }



            // Transfer Details Page

            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Transfer Details");
            Console.WriteLine("------------------------------------------");

            //Get transfer properties by passing in a transfer ID
            Transfer transfer = methods.GetTransferFromTransferId(transferId);

            //Get From username
            int accountFromId = transfer.AccountFrom;
            User usernameFrom = methods.GetUser(accountFromId);

            //Get To username
            int accountToId = transfer.AccountTo;
            User usernameTo = methods.GetUser(accountToId);

            Console.WriteLine($"Id: {transfer.TransferId}");
            Console.WriteLine($"From: {usernameFrom.Username}");
            Console.WriteLine($"To: {usernameTo.Username}");
            Console.WriteLine($"Type: {transfer.TransferTypeId}");
            Console.WriteLine($"Status: {transfer.TransferStatusId}");
            Console.WriteLine($"Amount: {transfer.Amount:c}");

            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult ViewRequests()
        {
            Console.WriteLine("Coming soon.");
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult SendTEBucks()
        {
            // Get a list of all users
            List<User> listOfUsers = methods.GetListOfUsers();
            
            // Get logged-in user's Id
            int userId = UserService.GetUserId();

            Console.WriteLine("------------------------------------");
            Console.WriteLine("Users ID           Name");
            Console.WriteLine("------------------------------------");

            //Print out all users that are not you
            foreach (User u in listOfUsers)
            {
                if (u.UserId != userId)
                {
                    Console.WriteLine($"{u.UserId}                   {u.Username}");
                }
            }


            // User selects dollar amount and userId to transfer money to
            int maxAttempts = 3;
            int recipientUserId = 0;
            int counter = 0;
            int gotrecipientUserId = 0;
            while (gotrecipientUserId != 1)
            {
                try
                {
                    if (counter < maxAttempts)
                    {
                        Console.Write("Please enter the Id of the user you'd like to send TE bucks to: ");
                    }

                    recipientUserId = int.Parse(Console.ReadLine());
                    for (int i = 0; i < listOfUsers.Count; i++)
                    {
                        if (listOfUsers[i].UserId == recipientUserId)
                        {
                            gotrecipientUserId = 1;
                        }
                    }

                    if ((counter < maxAttempts) && (gotrecipientUserId == 0))
                    {
                        Console.WriteLine("That transfer ID does not exist. Please enter a valid ID.");
                        counter++;
                        Console.WriteLine($"You have {3 - counter} attempts left.");
                    }
                    else if (counter == maxAttempts)
                    {
                        Console.WriteLine("You are being logged out ... (for your own good).");
                        Logout();
                        return MenuOptionResult.ExitAfterSelection;
                    }
                }

                catch
                {
                    if (counter < maxAttempts)
                    {
                        Console.WriteLine("You entered an invalid ID, please try again.");
                        counter++;
                        Console.WriteLine($"You have {3 - counter} attempts left.");
                    }
                    if (counter == maxAttempts)
                    {
                        Console.WriteLine("You are being logged out ... (for your own good).");
                        Logout();
                        return MenuOptionResult.ExitAfterSelection;
                    }
                }
            }

            // Try/Catch to verify that user is selecting a valid user Id

            //Get current users's balance
            Account currentUserAccount = methods.GetUserAccount(userId);
            decimal currentUserBalance = currentUserAccount.Balance;

            Console.Write("Enter Amount: ");
            decimal transferAmount = Decimal.Parse(Console.ReadLine());


            //Get recipient's AccountId - Use GetAccounts method
            List<Account> listOfAccounts = methods.GetListOfAccounts();

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
                methods.TransferBucks(senderAccountId, recipientAccountId, senderBalance, recipientBalance, transferAmount, listOfAccts);

                decimal updatedBalance = senderBalance - transferAmount;
                Console.WriteLine($"Your updated balance after transaction: {updatedBalance:c} ");
                Console.WriteLine("Thank you for doing business with Tenmo");  // TM? 

                Transfer newTransfer = new Transfer();
                newTransfer.TransferStatusId = (TransferStatus)2;
                newTransfer.TransferTypeId = (TransferType)2;
                newTransfer.AccountTo = recipientAccountId;
                newTransfer.AccountFrom = senderAccountId;
                newTransfer.Amount = transferAmount;

                Transfer transferReturned = methods.CreateTransfer(newTransfer);
            }
            else
            {
                Console.WriteLine("You do not have enough TE Bucks to complete transaction");
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

        

    }
}
