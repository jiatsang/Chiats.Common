namespace Chiats.SQL
{
    /// <summary>
    /// 條件式物件共用介面.  作為  SQL Model 中的 SQLCTL 條件管理
    /// </summary>
    /// <remarks>
    /// SqlModel 分別實作條件式物件(SelectModel.ModelCondition/UpdateModel.ModelCondition/DeleteModel.ModelCondition).
    /// SelectModel 同時包含 Where/Having 條件式 以 SelectModel.ModelCondition 實作 <br/>
    /// UpdateModel/DeleteModel 包含 Where 條件式 分別以  UpdateModel.ModelCondition 和 DeleteModel.ModelCondition <br/>
    /// </remarks>
    public interface IConditionModel : IVariantName
    {
        /// <summary>
        ///  指示條件是否啟用
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// 指定參數是否所屬於的目前的條件式 (Condition)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool BelongParameter(string name);

        /// <summary>
        /// 傳回指定參數是否所屬於的目前的條件(Condition)
        /// </summary>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        bool BelongParameter(Parameter Parameter);
    }
}