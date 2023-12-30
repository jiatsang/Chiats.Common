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
    /// SQL �ѪR�r��R�O�@�Τ���. �̦r��R�O���� DeleteModel ����
    /// </summary>
    internal class DeleteParser : BaseParser, ISqlParserObject
    {
        // delete from table where ...
        public static DeleteParser Default = new DeleteParser();

        /// <summary>
        /// �ѪR�r�� (TokenList) �è̦r��R�O���� DeleteModel
        /// </summary>
        /// <param name="tokenScanner">�� CommandText �g Token Scan �Უ�ͤ� TokenList.</param>
        /// <param name="SqlModel"></param>
        /// <returns> DeleteModel </returns>
        public SqlModel PaserCommand(SQLTokenScanner tokenScanner, SqlModel SqlModel, object parameters = null)
        {
            DeleteModel currentModel = ConvertCommonModel<DeleteModel>(SqlModel);

            int currrntIndex = 1; // ���������Ĥ@������r Select/Insert/Update/Delete

            ((ICommandInitialize)currentModel).BeginInit();
            if (StringComparer.OrdinalIgnoreCase.Equals(tokenScanner[currrntIndex].String, "from"))
            {
                currrntIndex++; // ���� from ����r(���\�i���i�L)
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
        /// ���ܥ�����, �Ҳ��ͤ�����  SqlModel �۹������󪫥����O �p SelectModel/UpdateModel/InsertModel/DeleteModel
        /// </summary>
        public Type ModelType
        {
            get { return typeof(DeleteModel); }
        }
    }
}