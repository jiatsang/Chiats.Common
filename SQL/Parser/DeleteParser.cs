// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;

namespace Chiats.SQL
{
    /// <summary>
    /// SQL 解析字串命令共用介面. 依字串命令產生 DeleteModel 物件
    /// </summary>
    internal class DeleteParser : BaseParser, ISqlParserObject
    {
        // delete from table where ...
        public static DeleteParser Default = new DeleteParser();

        /// <summary>
        /// 解析字串 (TokenList) 並依字串命令產生 DeleteModel
        /// </summary>
        /// <param name="tokenScanner">由 CommandText 經 Token Scan 後產生之 TokenList.</param>
        /// <param name="SqlModel"></param>
        /// <returns> DeleteModel </returns>
        public SqlModel PaserCommand(SQLTokenScanner tokenScanner, SqlModel SqlModel, object parameters = null)
        {
            DeleteModel currentModel = ConvertCommonModel<DeleteModel>(SqlModel);

            int currrntIndex = 1; // 直接忽略第一個關鍵字 Select/Insert/Update/Delete

            ((ICommandInitialize)currentModel).BeginInit();
            if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex].String, "from"))
            {
                currrntIndex++; // 忽略 from 關鍵字(允許可有可無)
            }
            StringObjectName k123 = StringObjectName.NameTest(tokenScanner, ref currrntIndex);

            if (k123.KeyIndex == 0) // not found
                throw new SqlModelSyntaxException("Not Table Name Found.");

            currentModel.Table = k123.Table;

            if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex].String, "where"))
            {
                currrntIndex++;
                ParserWhereStatement(tokenScanner, currentModel.Where, currrntIndex, tokenScanner.Count - 1);
            }
            if (parameters != null)
            {
                currentModel.Parameters.Fill(parameters);
            }
            ((ICommandInitialize)currentModel).EndInit();

            return currentModel;
        }

        /// <summary>
        /// 指示本物件, 所產生之對應  SqlModel 相對應物件物件類別 如 SelectModel/UpdateModel/InsertModel/DeleteModel
        /// </summary>
        public Type ModelType
        {
            get { return typeof(DeleteModel); }
        }
    }
}