using System;

namespace Chiats
{
    /// <summary>
    /// �Ѽ����� ArgumentType (None,Scope,Multi,MultiScope)   �Ҧp SQL CTL �w�q : {{$Name:Scope Column,@Param}}XXX
    /// </summary>
    [Flags]
    public enum ArgumentType
    {
        /// <summary>
        /// �D�\�૬�����󦡰Ѽ�
        /// </summary>
        None = 0,

        /// <summary>
        /// �d�򫬪����󦡰Ѽ� �p Column &gt;= @Param#From and Column &lt;= @Param#To
        /// </summary>
        Scope = 0x01,

        /// <summary>
        /// �h�ȫ������󦡰Ѽ� �p Column = @Param#M1 or Column = @Param#M2 or ...
        /// </summary>
        Multi = 0x02,

        /// <summary>
        /// �h��+�d�򫬪����󦡰Ѽ� �p Column = @Parameter#M1 or ( Column &gt;= @Param#M2#From and Column &lt;= @Param#M2#To )
        /// </summary>
        MultiScope = Scope | Multi,

        /// <summary>
        /// �\�૬ Like ���󦡰Ѽ�
        /// </summary>
        Like = 0x04,

        /// <summary>
        /// �\�૬������󦡰Ѽ�
        /// </summary>
        Equal = 0x08,
    }
}