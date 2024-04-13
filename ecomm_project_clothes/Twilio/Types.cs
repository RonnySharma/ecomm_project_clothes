namespace Laptop_Ecommerce.Twilio
{
    public class Types
    {
        public class PhoneNumber:global::Twilio.Types.PhoneNumber
        {
            public PhoneNumber(string number):base(number)
            {

            }
        }
    }
}
