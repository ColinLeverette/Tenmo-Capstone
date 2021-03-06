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
