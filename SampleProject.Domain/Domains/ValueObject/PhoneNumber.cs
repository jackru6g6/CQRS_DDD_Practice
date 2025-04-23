namespace SampleProject.Domain.Domains.ValueObject
{
    public record PhoneNumber
    {
        public string CountryCode { get; }

        public string Number { get; }

        public PhoneNumber(string countryCode, string number)
        {
            if (string.IsNullOrEmpty(countryCode))
            {
                //throw new ValidationException("countryCode不能为空");
                throw new Exception("countryCode 不能为空");
            }

            if (string.IsNullOrEmpty(number))
            {
                //throw new ValidationException("number不能为空");
                throw new Exception("number 不能为空");
            }

            CountryCode = countryCode;
            Number = number;
        }

        public bool IsAreaCode()
        {
            string[] areas = ["886", "86"];

            return areas.Contains(CountryCode);
        }
    }
}