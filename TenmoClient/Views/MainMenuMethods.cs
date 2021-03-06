using AuctionApp.Exceptions;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TenmoClient.Models;

namespace TenmoClient.Views
{
  public   class MainMenuMethods 
    {
        private readonly static string API_TRANSFER_URL = "https://localhost:44315/transfers/";
        private readonly static string API_ACCOUNT_URL = "https://localhost:44315/accounts/";
        private readonly static string API_USERS_URL = "https://localhost:44315/users/";
        private readonly IRestClient client = new RestClient();
        private string token = UserService.GetToken();
        
        public  Account GetUserAccount(int userId)
        {
            client.Authenticator = new JwtAuthenticator(token);

            RestRequest request = new RestRequest(API_ACCOUNT_URL + userId);
            IRestResponse<Account> response = client.Get<Account>(request);
            Account account = null;

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                MainMenuMethods.ProcessErrorResponse(response);
            }
            else
            {
                account = response.Data;
            }
            return account;
        }

        public User GetUser(int accountId)
        {
            client.Authenticator = new JwtAuthenticator(token);
            User user = new User();
            RestRequest request = new RestRequest(API_USERS_URL + "account/" + accountId);
            IRestResponse<User> response = client.Get<User>(request);
            user= response.Data;

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                MainMenuMethods.ProcessErrorResponse(response);
            }
            else
            {
                user = response.Data;
            }
            return user;
        }


        public List<Transfer> GetListOfTransfers(int userId)
        {
            client.Authenticator = new JwtAuthenticator(token);
            RestRequest request = new RestRequest(API_TRANSFER_URL + userId);
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            List<Transfer> listOfTransfers = new List<Transfer>();

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                MainMenuMethods.ProcessErrorResponse(response);
            }
            else
            {
                listOfTransfers= response.Data;
            }
            return listOfTransfers;            
        }

        public Transfer GetTransferFromTransferId (int transferId)
        {
            client.Authenticator = new JwtAuthenticator(token);
            RestRequest request = new RestRequest(API_TRANSFER_URL + "lookup/" + transferId);
            IRestResponse<Transfer> response = client.Get<Transfer>(request);
            Transfer transfer = new Transfer();
            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                MainMenuMethods.ProcessErrorResponse(response);
            }
            else
            {
                transfer = response.Data;
            }
            return transfer;
        }
        

        public List<User> GetListOfUsers()
        {
            client.Authenticator = new JwtAuthenticator(token);
            RestRequest request = new RestRequest(API_USERS_URL);
            IRestResponse<List<User>> response = client.Get<List<User>>(request);
            List<User> listOfUsers = new List<User>();
            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                MainMenuMethods.ProcessErrorResponse(response);
            }
            else
            {
                listOfUsers = response.Data;
            }
            return listOfUsers;
        }

        public List<Account> GetListOfAccounts()
        {
            client.Authenticator = new JwtAuthenticator(token);
            RestRequest request = new RestRequest(API_ACCOUNT_URL);
            IRestResponse<List<Account>> response = client.Get<List<Account>>(request);
            List<Account> listOfAccounts = new List<Account>();
            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                MainMenuMethods.ProcessErrorResponse(response);
            }
            else
            {
                listOfAccounts = response.Data;
            }
            return listOfAccounts;
        }


        public void TransferBucks(int senderAccountId, int recipientAccountId, decimal senderBalance, decimal recipientBalance, decimal transferAmount, List<Account> listOfAccts)
        {
            client.Authenticator = new JwtAuthenticator(token);
            RestRequest request = new RestRequest(API_ACCOUNT_URL + senderAccountId + "/transfer/" + recipientAccountId + "/" + senderBalance + "/" +
                recipientBalance + "/" + transferAmount);
            request.AddJsonBody(listOfAccts);
  
        }

        public Transfer CreateTransfer(Transfer transfer)
        {
            client.Authenticator = new JwtAuthenticator(token);
            RestRequest request = new RestRequest(API_TRANSFER_URL);
            request.AddJsonBody(transfer);
            IRestResponse<Transfer> response = client.Post<Transfer>(request);
            Transfer returnTransfer = new Transfer();
            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                MainMenuMethods.ProcessErrorResponse(response);
            }
            else
            {
                returnTransfer = response.Data;
            }
            return returnTransfer;
        }


        public static void ProcessErrorResponse(IRestResponse response)
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
