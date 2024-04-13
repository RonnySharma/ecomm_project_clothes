using Twilio.Rest.Api.V2010.Account;

namespace Laptop_Ecommerce.Twilio
{
    public interface ISMSService
    {
        MessageResource Send(string mobileNumber, string body);
    }
}
