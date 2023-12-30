using System;
using System.Data;
using System.Data.Common;

namespace Chiats.SQL
{
    /// <summary>
    ///
    /// </summary>
    public class CommandParameter : DbParameter, IVariantName
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public CommandParameter(string Name, object Value)
        {
            this.ParameterName = Name;
            this.Value = Value;
            switch(Value)
            {
                case int number:
                    this.DbType = DbType.Int32;
                    break;
                case long number2:
                    this.DbType = DbType.Int64;
                    break;
                case uint number:
                    this.DbType = DbType.UInt32;
                    break;
                case ulong number2:
                    this.DbType = DbType.UInt64;
                    break;
                case DateTime datetime :
                    this.DbType = DbType.DateTime2;
                    break;
                case bool _val:
                    this.DbType = DbType.Boolean;
                    break;
                //default:
                //    this.DbType = DbType.String;
                //    break;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <param name="dbtype"></param>
        public CommandParameter(string Name, object Value, DbType dbtype)
        {
            this.ParameterName = Name;
            this.Value = Value;
            this.DbType = dbtype;
        }

        private DbType _DbType = DbType.String;

        public override DbType DbType
        {
            get
            {
                return _DbType;
            }
            set
            {
                _DbType = value;
            }
        }

        private ParameterDirection _direction = ParameterDirection.Input;

        public override ParameterDirection Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }

        private bool _IsNullable = true;

        /// <summary>
        ///
        /// </summary>
        public override bool IsNullable
        {
            get
            {
                return _IsNullable;
            }
            set
            {
                _IsNullable = value;
            }
        }

        private string _parameterName;

        /// <summary>
        ///
        /// </summary>
        public override string ParameterName
        {
            get
            {
                return _parameterName;
            }
            set
            {
                _parameterName = value;
            }
        }

        public string Name { get { return _parameterName; } }

        /// <summary>
        ///
        /// </summary>
        public override void ResetDbType()
        {
        }

        private int _size = 0;

        /// <summary>
        ///
        /// </summary>
        public override int Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }

        private string _sourceColumn = null;

        /// <summary>
        ///
        /// </summary>
        public override string SourceColumn
        {
            get
            {
                return _sourceColumn;
            }
            set
            {
                _sourceColumn = value;
            }
        }

        private bool _sourceColumnNullMapping = false;

        /// <summary>
        ///
        /// </summary>
        public override bool SourceColumnNullMapping
        {
            get
            {
                return _sourceColumnNullMapping;
            }
            set
            {
                _sourceColumnNullMapping = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public override DataRowVersion SourceVersion
        {
            get
            {
                return DataRowVersion.Current;
            }
            set
            {
            }
        }

        private object _value = null;

        public override object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
    }
}