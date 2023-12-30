namespace Chiats.Data
{
    /// <summary>
    /// 表示該資料列的狀態
    /// </summary>
    public enum TableRowState
    {
        /// <summary>
        /// 資料列
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 新增旳資料列.
        /// </summary>
        Newer = 1,
        /// <summary>
        /// 修改中的資料列. 
        /// </summary>
        EnableUpdateBuffer = 2,
        ///// <summary>
        ///// 表示己被刪除的資料列. 
        ///// </summary>
        Deleted = 3,
        /// <summary>
        /// 表示為空資料列
        /// </summary>
        Empty = 4

    }
}
