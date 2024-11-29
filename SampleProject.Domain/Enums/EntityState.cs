namespace SampleProject.Domain.Enums
{
    public enum EntityState
    {
        /// <summary>
        /// The entity is not being tracked by the context.
        /// 實體未被上下文追蹤
        /// </summary>
        Detached = 0,

        /// <summary>
        /// The entity is being tracked by the context and exists in the database. Its property
        /// values have not changed from the values in the database.
        /// 實體正在被上下文追蹤，並且存在於資料庫中。其屬性值與資料庫中的值沒有改變。
        /// </summary>
        Unchanged = 1,

        /// <summary>
        /// The entity is being tracked by the context and exists in the database. It has been marked
        /// for deletion from the database.
        /// 實體正在被上下文追蹤，並且存在於資料庫中。它已被標記為將從資料庫中刪除
        /// </summary>
        Deleted = 2,

        /// <summary>
        /// The entity is being tracked by the context and exists in the database. Some or all of its
        /// property values have been modified.
        /// 實體正在被上下文追蹤，並且存在於資料庫中。其部分或全部屬性值已被修改
        /// </summary>
        Modified = 3,

        /// <summary>
        /// The entity is being tracked by the context but does not yet exist in the database.
        /// 實體正在被上下文追蹤，但尚未存在於資料庫中
        /// </summary>
        Added = 4
    }
}