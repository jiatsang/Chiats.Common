using System;

namespace Chiats
{
    /// <summary>
    /// 把计摸 ArgumentType (None,Scope,Multi,MultiScope)   ㄒ SQL CTL ﹚竡 : {{$Name:Scope Column,@Param}}XXX
    /// </summary>
    [Flags]
    public enum ArgumentType
    {
        /// <summary>
        /// 獶兵ンΑ把计
        /// </summary>
        None = 0,

        /// <summary>
        /// 絛瞅兵ンΑ把计  Column &gt;= @Param#From and Column &lt;= @Param#To
        /// </summary>
        Scope = 0x01,

        /// <summary>
        /// 兵ンΑ把计  Column = @Param#M1 or Column = @Param#M2 or ...
        /// </summary>
        Multi = 0x02,

        /// <summary>
        /// +絛瞅兵ンΑ把计  Column = @Parameter#M1 or ( Column &gt;= @Param#M2#From and Column &lt;= @Param#M2#To )
        /// </summary>
        MultiScope = Scope | Multi,

        /// <summary>
        ///  Like 兵ンΑ把计
        /// </summary>
        Like = 0x04,

        /// <summary>
        /// 单兵ンΑ把计
        /// </summary>
        Equal = 0x08,
    }
}