namespace SampleProject.Domain.Enums
{
    public record Currency : Enumeration
    {
        /// <summary>
        /// 人民幣
        /// </summary>
        public static Currency CNY = new(1, nameof(CNY));

        public static IEnumerable<Currency> List() => [CNY];

        public Currency(int id, string name) : base(id, name) { }

        public static Currency FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state is null)
            {
                throw new Exception($"Possible values for Currency: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
