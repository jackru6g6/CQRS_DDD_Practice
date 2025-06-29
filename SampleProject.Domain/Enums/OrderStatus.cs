namespace SampleProject.Domain.Enums
{
    public record OrderStatus : Enumeration // class
    {
        /// <summary>
        /// 異常單
        /// </summary>
        public static OrderStatus Abnormal = new(-99, nameof(Abnormal));

        /// <summary>
        /// 處理失敗
        /// </summary>
        public static OrderStatus Failed = new(1, nameof(Failed));

        /// <summary>
        /// 未處理
        /// </summary>
        public static OrderStatus Unprocessed = new(0, nameof(Unprocessed));

        /// <summary>
        /// 已處理
        /// </summary>
        public static OrderStatus Succeeded = new(1, nameof(Succeeded));

        /// <summary>
        /// 正在處理
        /// </summary>
        public static OrderStatus Processing = new(2, nameof(Processing));

        /// <summary>
        /// 取消
        /// </summary>
        public static OrderStatus Cancel = new(5, nameof(Cancel));

        /// <summary>
        /// 審核中
        /// </summary>
        public static OrderStatus Review = new(6, nameof(Review));

        /// <summary>
        /// 已到帳待確認
        /// </summary>
        public static OrderStatus TransactionToBeConfirmed = new(7, nameof(TransactionToBeConfirmed));

        public OrderStatus(int id, string name) : base(id, name) { }

        public static IEnumerable<OrderStatus> List() => [Abnormal, Failed, Unprocessed, Succeeded, Processing, Cancel, Review, TransactionToBeConfirmed];

        public static OrderStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state is null)
            {
                //throw new OrderingDomainException($"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static OrderStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state is null)
            {
                throw new Exception($"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
