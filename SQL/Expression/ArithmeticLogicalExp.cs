// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;

namespace Chiats.SQL.Expression
{

    public class RankAndRowNumberExp : NonTerminalExp
    {
        public string Expression { get; private set; }
        public RankAndRowNumberExp(string Expression) 
        {
            this.Expression = Expression;
        }
        public override void Export(CommandBuilder CommandBuilder, CommandBuildType BuildType, ParameterMode ParameterMode, ExportParameter Exporter, bool ForceLowerName)
        {
            // throw new System.NotImplementedException();
        }
    }
    /// <summary>
    ///  ��N�޿�B�⦡  ." Example :   A + B
    ///  �]�t�D�׺ݹB�⤸
    ///       �|�h�B�⤸    +-*/  (�h���B��l)
    ///  �޿�B�⦡�����]�P�ɥi�H�]�t�⦡�B�⦡   Example :    A >= B+C
    ///  ArithmeticLogicalExp �t�d�ѪR �⦡���޿�B�⤧���Y. ���]�t�i�p�ⶶ�ǩM��k.
    /// </summary>
    public class ArithmeticLogicalExp : NonTerminalExp, IEnumerable<ArithmeticLogicalExp.ArithmeticLogicalLinker>
    {
        private List<ArithmeticLogicalLinker> linker_list = new List<ArithmeticLogicalLinker>();

        #region class ArithmeticLogicalLinker

        /// <summary>
        ///  ��N�޿�B�⦡����@�`�I ,�t�B�⦡�M�s���l (LogicalLinkOperator,ArithmeticLinkOperator)
        ///  ��N�޿�B�⦡���̫�@�Ӹ`�I�s���l�T�w�� ArithmeticLogicalLink.Empty
        /// </summary>
        public class ArithmeticLogicalLinker
        {
            /// <summary>
            /// �s�W�@�Ӻ�N�޿�B�⦡
            /// </summary>
            /// <param name="Exp"></param>
            public ArithmeticLogicalLinker(BaseExp Exp) { this.exp = Exp; this.linker = ArithmeticLogicalLink.Empty; }

            /// <summary>
            ///
            /// </summary>
            /// <param name="Exp"></param>
            /// <param name="Linker"></param>
            public ArithmeticLogicalLinker(BaseExp Exp, ArithmeticLogicalLink Linker) { this.exp = Exp; this.linker = Linker; }

            private BaseExp exp;
            private ArithmeticLogicalLink linker;

            /// <summary>
            ///  �O�_���̫�@�Ӹ`�I�s���l
            /// </summary>
            public bool IsTerminalLinker { get { return linker == ArithmeticLogicalLink.Empty; } }

            /// <summary>
            ///  �O�_���޿�B��l.  False �ɬ���N�B�⦡�l
            /// </summary>
            public bool IsLogicalLinker { get { return linker >= ArithmeticLogicalLink.LogicalLink; } }

            /// <summary>
            /// �^�ǹB�⦡����.
            /// </summary>
            public BaseExp Expression { get { return exp; } }

            /// <summary>
            ///  �^�ǹB�⦡����.
            /// </summary>
            /// <typeparam name="T">�B�⦡���󫬧O</typeparam>
            /// <returns></returns>
            public T Exp<T>() where T : BaseExp
            {
                return (T)(exp as T);
            }

            /// <summary>
            /// �^�Ǻ⦡�l.(��N���޿�B�⦡)
            /// </summary>
            public ArithmeticLogicalLink Linker { get { return linker; } internal set { linker = value; } }
        }

        #endregion class ArithmeticLogicalLinker

        /// <summary>
        ///  ��N�޿�B�⦡,
        /// </summary>
        public ArithmeticLogicalExp(BaseExp Exp)
        {
            linker_list.Add(new ArithmeticLogicalLinker(Exp, ArithmeticLogicalLink.Empty));
        }

        /// <summary>
        /// �^�Ǻ⦡���Ӽ�
        /// </summary>
        public int Count { get { return linker_list.Count; } }

        /// <summary>
        ///  �^�ǫ��w �⦡���޿�B�⦡
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ArithmeticLogicalLinker this[int index] { get { return linker_list[index]; } }

        /// <summary>
        /// �s�W�@�� ��N�޿�B�⦡ .
        /// </summary>
        /// <param name="Exp"></param>
        /// <param name="ArithmeticLink"></param>
        internal void Add(ArithmeticLogicalLink ArithmeticLink, BaseExp Exp)
        {
            if (ArithmeticLink != ArithmeticLogicalLink.Empty &&
                ArithmeticLink != ArithmeticLogicalLink.LogicalLink &&
                ArithmeticLink != ArithmeticLogicalLink.ArithmeticLink)   // Empty,LogicalLink , ArithmeticLink ���L�Ī��s���l
            {
                ArithmeticLogicalLinker LastLinker = linker_list[linker_list.Count - 1];
                LastLinker.Linker = ArithmeticLink;
                if (Exp is ArithmeticLogicalExp)
                {
                    // �X�� �h�� ArithmeticLogicalExp ����@��N�޿�B�⦡ ,
                    Debug.Print("Message : �X�� �h�� ArithmeticLogicalExp ����@��N�޿�B�⦡  ");
                    ArithmeticLogicalExp ALExp = (ArithmeticLogicalExp)Exp;
                    foreach (var linker in ALExp)
                    {
                        linker_list.Add(linker);
                    }
                }
                else
                {
                    linker_list.Add(new ArithmeticLogicalLinker(Exp, ArithmeticLogicalLink.Empty));
                }
            }
            else
            {
                Debug.Print("ERROR : Empty,LogicalLink , ArithmeticLink ���L�Ī��s���l  ");
            }
        }

        /// <summary>
        /// ��X�B�⦡���r���ܦ�
        /// </summary>
        /// <param name="CommandBuilder">�B�⦡���r���ܦ����;�</param>
        /// <param name="BuildType"></param>
        /// <param name="ParameterMode"></param>
        /// <param name="Exporter"></param>
        public override void Export(CommandBuilder CommandBuilder, CommandBuildType BuildType, ParameterMode ParameterMode, ExportParameter Exporter , bool ForceLowerName)
        {
            foreach (ArithmeticLogicalExp.ArithmeticLogicalLinker linker in linker_list)
            {
                linker.Expression.Export(CommandBuilder, BuildType, ParameterMode, Exporter, ForceLowerName);
                if (!linker.IsTerminalLinker)
                {
                    CommandBuilder.AppendToken(linker.Linker.EnumConvert<ArithmeticLogicalLink>());
                }
            }
        }

        /// <summary>
        /// �Ǧ^�|�v�@�d�ݶ��X���C�|�{���C
        /// </summary>
        /// <returns>���O�GSystem.Collections.IEnumerator  ����A�Ω�v�@�d�ݶ��X�C</returns>
        public IEnumerator<ArithmeticLogicalExp.ArithmeticLogicalLinker> GetEnumerator()
        {
            return linker_list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return linker_list.GetEnumerator();
        }
    }
}