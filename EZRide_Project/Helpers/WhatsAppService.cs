
using RestSharp;

namespace EZRide_Project.Helpers
{
    public class WhatsAppService
    {
        private readonly string _instanceUrl = "https://api.ultramsg.com/instance124193/messages/chat";
        private readonly string _token = "z5lhl4h70gppj4k8";

        public async Task<bool> SendMessageAsync(string phone, string message)
        {
            var client = new RestClient(_instanceUrl);
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("token", _token);
            request.AddParameter("to", phone);
            request.AddParameter("body", message);

            var response = await client.ExecuteAsync(request);
            return response.IsSuccessful;
        }

    }
}
