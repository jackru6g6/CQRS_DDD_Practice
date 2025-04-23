namespace SampleProject.Domain.Repositories.Entity
{
    /// <summary>
    /// 訂單歷程
    /// </summary>
    public class OrderEventEntity
    {
        /// <summary>
        /// 序列號
        /// </summary>
        /// <remarks>
        /// 不重要，不是商業邏輯一部分，為了符合 DB 規範才有的，是資料庫新增遞增，可以考慮不加入 Entity
        /// </remarks>
        //public int Seq { get; set; }

        public Guid OrderId { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
