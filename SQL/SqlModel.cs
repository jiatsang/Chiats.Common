// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Chiats.SQL
{

    /// <summary>
    /// SqlModel ����¦���� CommonModel �������O. ��h�W�O�H�䴩�з� SQL �y�k����¦���O.<br/>
    /// SelectSqlModel ���]�p�O�H�з� SQL �y�k,���D�n�ت�, ������{�ꪺ�Ҷq. �ȥH
    /// SQL Server 2005/2008 �����Ҷi����թM�B�@. �䴩���y�k�����h�H���`�Ϊ��y�k������.
    /// ���|�Ҷq���Ӫ��X�i�ʤΤ��P��Ʈw���~���i���, ���L�k�O�Ҥ@�w�i�H�ۮe.
    /// </summary>
    /// <remarks>
    /// SQLModlel �䴩�H�{���y�����B�z��k, �Ӳ��ͦX�k�� SQL �y�k , �p Select/Insert/Update/Delete<br/>
    /// �p�U�H�d�ҩҥ�.
    /// <code>
    /// SelectSqlModel obj_model = new SelectSqlModel();
    ///
    /// obj_model.Column.Add("*");
    /// obj_model.Table.PrimaryTableName = "MyTable";
    /// Assert.AreEqual(obj_model.CommandText, "SELECT * FROM MyTable");
    /// </code>
    /// SqlModel ��¦�W�O�N�{�����H�r�ꪺ�զ� SQL �y�k�覡�ഫ���t�@�ؤ�k.
    /// <code>
    /// sql = string.Format("Select * From {0}",MyTableName);
    /// </code>
    /// �Ӧ��ؤ覡�ണ�Ѥ�r�ꪺ�զ���h���u�I. <br/>
    /// 1. �{�����iŪ��, �åB���e���� SQL �y�k�X��. �Ҧp��������r. <br/>
    /// 2. SQL �R�O���i���ҩ�. �Ҧp�ڭ̥i�H��������X 'SELECT * FROM MyTable' �������W��, �O�W����<br/>
    /// ���F���W��, ���ڭ̤]�N�i�H���o��Ʈw�Ӫ�檺�Ҧ�����T, �Ʀܯ��޸�T����.���]�N��ܧڭ̨��i�H�@��
    /// SQL �y�k������.
    /// <code>
    /// TableName = obj_model.Table.PrimaryTableName;
    /// AlisaTableName = obj_model.Table.AlisaTableName;
    /// </code>
    /// �ثe�p�����䴩���� Select/Insert/Update/Delete �� StoreProcedure ���I�s���� �����`�λy�k.<br/>
    /// ���T���䴩���� Select Into �y�k. <br/>
    /// StoreProcedure ���I�s�����]�|���䭭��. �䤤�� StoreProcedure �ǤJ�ѼƬO����S�����D.
    /// �����X�����G. �p�G�b�L�k���ҤU, �h�i��|�y���@�Ǩt�Ϊ���í�w���ӷ�.
    /// </remarks>
    public abstract class SqlModel :
        ISqlBuilder,
        IPartSqlModel,
        ICommandInitialize,
        ICommandBuilder,
        IParameterCommandBuilder,
        IBuildExportSupport,
        IDisposable
    {

        internal SqlModel Parent = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _init = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _changed = 0;


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ParameterMode parameterMode = ParameterMode.Parameter;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ISqlBuildExport currentBuildExport = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static ISqlBuildExport defaultExport = DefaultBuildExport.SQLBuildExport;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static ISqlBuildExport defaultCTLExport = DefaultBuildExport.SQLCTLBuildExport;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SqlOptions options;

        /// <summary>
        /// ���ܫإ� SQL �R�O����r�ԭz����X�榡�覡,
        /// </summary>
        public CommandBuildType BuildType { get; private set; } = CommandBuildType.SQL;

        /// <summary>
        ///  �j���ഫ���p�g�W��. ���W��/���W��
        /// </summary>
        public SqlOptions Options
        {
            get
            {
                var TopModel = GetTopModel();
                if (TopModel == this)
                    return options;
                return TopModel.Options;
            }

            set
            {
                var TopModel = GetTopModel();
                if (TopModel == this)
                    options = value;
                else
                    TopModel.Options = value;
            }
        }

        /// <summary>
        /// ���o�ثe�� BuildExport ����.
        /// </summary>
        protected ISqlBuildExport GetBuildExport(CommandBuildType buildType, CommandFormatOptions formatFlags, ISqlBuildExport buildExport)
        {
            ISqlBuildExport BuildExport = null;
            if (buildType == CommandBuildType.SQLCTL)
                BuildExport = defaultCTLExport;
            else if (buildExport != null)
                // ���w BuildExport ����O�� IBuildExportSupport.BuildBegin �� BuildEnd �d��.
                BuildExport = currentBuildExport = buildExport;
            else if (currentBuildExport != null)
                BuildExport = currentBuildExport;
            else
                BuildExport = defaultExport; // �����w�h�@�ΦP�@�� BuildExport ����.

            BuildExport.FormatOptions = formatFlags;
            BuildExport.Options = Options;
            return BuildExport;
        }


        /// <summary>
        /// �ѪR SQL �r��R�O�@�Τ���. �̦r��R�O���ͬ۹����� SelectModel/UpdateModel/InsertModel/DeleteModel ����
        /// </summary>
        /// <typeparam name="T">CommonModel �p SelectModel/UpdateModel/InsertModel/DeleteModel </typeparam>
        /// <param name="CTLSQL">SQL �r��R�O</param>
        /// <returns></returns>
        public static T Parse<T>(string CTLSQL, object parameters = null) where T : SqlModel
        {

            if (typeof(T) == typeof(CommandModel))
            {
                SqlModel rs = new CommandModel(CTLSQL, parameters);
                return (T)rs;
            }

            T result = null;
            SQLTokenScanner list = new SQLTokenScanner(CTLSQL);
            ISqlParserObject ParserObject = null;

            if (list.Count > 0)
            {
                string first_token = list[0].String;
                if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "select"))
                    ParserObject = SelectParser.Default;
                else if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "update"))
                    ParserObject = UpdateParser.Default;
                else if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "delete"))
                    ParserObject = DeleteParser.Default;
                else if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "insert"))
                    ParserObject = InsertParser.Default;

                if (ParserObject != null &&
                    (ParserObject.ModelType == typeof(T) ||
                    ParserObject.ModelType.IsSubclassOf(typeof(T))))
                {
                    result = (T)ParserObject.PaserCommand(list, null, parameters);
                }
            }
            return result; // �L�k�ѪR�θѪR���~�ɦ^�� NULL
        }

        /// <summary>
        /// �ѪR SQL �r��R�O�@�Τ���. �̦r��R�O���ͬ۹����� SelectModel/UpdateModel/InsertModel/DeleteModel ����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CTLSQL"></param>
        /// <param name="SqlModel"></param>
        /// <returns></returns>
        internal static T Parse<T>(string CTLSQL, T SqlModel, object parameters = null) where T : SqlModel
        {
            SQLTokenScanner list = new SQLTokenScanner(CTLSQL);
            ISqlParserObject ParserObject = null;
            T result = null;

            if (list.Count > 0)
            {
                string first_token = list[0].String;
                string second_token = (list.Count > 1) ? list[1].String : null;

                // �H�r��r���M�w�ϥΨ��@�� ParserObject
                if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "select"))
                    ParserObject = SelectParser.Default;
                else if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "update"))
                    ParserObject = UpdateParser.Default;
                else if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "delete"))
                    ParserObject = DeleteParser.Default;
                else if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "insert"))
                    ParserObject = InsertParser.Default;
                else if (StringComparer.OrdinalIgnoreCase.Equals(first_token, "create") && StringComparer.OrdinalIgnoreCase.Equals(second_token, "table"))
                    ParserObject = CreateTableParser.Default;

                if (ParserObject != null &&
                    (ParserObject.ModelType == typeof(T) || ParserObject.ModelType.IsSubclassOf(typeof(T)))
                )
                {
                    result = (T)ParserObject.PaserCommand(list, SqlModel, parameters);
                }
            }
            return result; // �ѪR�L�k�ѪR�θѪR�ɦ^�� NULL
        }

        /// <summary>
        /// �ѪR SQL �r��R�O�@�Τ���. �̦r��R�O���ͬ۹����� SelectModel/UpdateModel/InsertModel/DeleteModel ����
        /// </summary>
        /// <param name="commandText">SQL �r��R�O</param>
        /// <returns></returns>
        public static SqlModel Parse(string commandText, object parameters = null)
        {
            return Parse<SqlModel>(commandText, parameters);
        }

        /// <summary>
        ///  SqlModel �򥻫غc�l
        /// </summary>
        public SqlModel()
        {
            OnConstructorInitiailize();
            OnConstructorFinish();
        }

        /// <summary>
        ///  SqlModel  �غc�e����l�ƨƥ�. ��l�ƥ��n�����������.
        /// </summary>
        protected internal virtual void OnConstructorInitiailize()
        { }

        /// <summary>
        /// SqlModel  �غc��e���ƥ�. �@������إ߫�ݭn�B�z���u�@.
        /// </summary>
        protected internal virtual void OnConstructorFinish()
        { }

        /// <summary>
        ///  ISupportInitialize.BeginInit
        /// </summary>
        protected void BeginInit()
        {
            lock (this) { _init++; }
        }

        void ICommandInitialize.BeginInit()
        {
            BeginInit();
        }

        /// <summary>
        ///  ISupportInitialize.EndInit
        /// </summary>
        protected void EndInit()
        {
            lock (this) { _init--; }
        }

        void ICommandInitialize.EndInit()
        {
            EndInit();
        }

        /// <summary>
        /// �� BuildType ���s�إ� SQL �R�O����r�ԭz.
        /// </summary>
        /// <param name="BuildType">���ܫإ� SQL �R�O����r�ԭz���覡</param>
        /// <param name="buildFlags"></param>
        /// <returns>SQL �R�O����r�ԭz</returns>
        protected abstract string RebuildCommand(CommandBuildType BuildType, CommandFormatOptions buildFlags = CommandFormatOptions.None, ISqlBuildExport buildExport = null);

        /// <summary>
        /// ��l�Ƨ����|����.
        /// </summary>
        protected virtual void OnInitialized() { }

        /// <summary>
        /// SQL Command ��Ƥ��e�ܧ󪺨ƥ�q��
        /// </summary>
        public event EventHandler CommandChanged;

        /// <summary>
        ///
        /// </summary>
        protected virtual void OnCommandChanged() { }

        /// <summary>
        /// �q�����󤺮e�v�ܧ� , SqlModel
        /// </summary>
        protected internal void Changed()
        {
            lock (this)
            {
                _changed++; // �q�� COMMAND ���e�����ʩβ���SQL COMMAND �����ܧ�
                OnCommandChanged();
                if (CommandChanged != null) CommandChanged(this, EventArgs.Empty);
            }
        }
        protected internal int ChangeCount()
        {
            if (this.Parent != null)
                return _changed + Parent.ChangeCount();  // �ܧ�P�ɥ]�t Parent SqlModel 
            return _changed;
        }
        /// <summary>
        /// �Ǧ^�̤W�@�h�� CommonModel ��������
        /// </summary>
        /// <returns>�̤W�@�h�� CommonModel ��������</returns>
        public SqlModel GetTopModel()
        {
            if (Parent == null)
                return this;
            return Parent.GetTopModel();
        }

        /// <summary>
        /// ���ܳB�z Parameter �Ѽƪ���k.  ���@�ѼƬ��@�� TopModel ����.
        /// </summary>
        public ParameterMode ParameterMode
        {
            get
            {
                if (GetTopModel() == this)
                    return parameterMode;
                return GetTopModel().ParameterMode;
            }

            set
            {
                if (GetTopModel() == this)
                    parameterMode = value;
                else
                    GetTopModel().ParameterMode = value;
                this.Changed();
            }
        }

        #region ISQLCommandBuilder ����

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string command_text = null;

        /// <summary>
        /// ���s�إ� SQL �R�O����r�ԭz.
        /// </summary>
        public string CommandText
        {
            get
            {
                lock (this)
                {
                    // _change <> 0 ��� COMMAND ���e�����ʩβ���SQL COMMAND �����ܧ�
                    if (command_text == null || ChangeCount() != 0)
                        command_text = RebuildCommand(BuildType);
                    _changed = 0;
                    return command_text;
                }
            }
        }

        /// <summary>
        /// �� BuildType ���s�إ� SQL �R�O����r�ԭz.
        /// </summary>
        /// <param name="buildType">���ܫإ� SQL �R�O����r�ԭz���覡</param>
        /// <returns>SQL �R�O����r�ԭz</returns>
        public string BuildCommand(CommandBuildType buildType)
        {
            return RebuildCommand(buildType, CommandFormatOptions.None);
        }

        public string BuildCommand(ISqlBuildExport buildExport, CommandFormatOptions buildFlags = CommandFormatOptions.None)
        {
            return RebuildCommand(buildExport.BuildType, buildFlags, buildExport);
        }

        /// <summary>
        ///  BuildType ���s�إ� SQL �R�O����r�ԭz.
        /// </summary>
        /// <param name="buildType"></param>
        /// <param name="buildFlags"></param>
        /// <returns></returns>
        public string BuildCommand(CommandBuildType buildType, CommandFormatOptions buildFlags = CommandFormatOptions.None)
        {
            return RebuildCommand(buildType, buildFlags);
        }

        #endregion ISQLCommandBuilder ����

        /// <summary>
        /// String�A��ܥثe�� Object�C
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return RebuildCommand(CommandBuildType.SQLCTL);
        }

        /// <summary>
        /// ���o�Ҧ������s�� Parameter ����. �B�@�� CoverLinker �N���P���X�����W�٬ۦP�����s���_��.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal CoverLinker<Parameter> CoverParameters = new CoverLinker<Parameter>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CommonModelParameters list = null;

        /// <summary>
        /// �Ǧ^���t���Ҧ��Ѽƪ���.
        /// </summary>
        public CommonModelParameters Parameters
        {
            get
            {
                if (list == null)
                {
                    list = new CommonModelParameters(CoverParameters);
                    list.Changed += Parameters_Changed;
                }

                return list;
            }
        }
        /// <summary>
        /// �]�w���w�Ѽ�, �p�ѼƤ��s�b, �h�^�� false
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool SetParameterValue(string Name, object Value)
        {

            foreach(var cp in CoverParameters)
            {
                Debug.Print($"CoverParameter [{Name}] {cp.Name}/{cp.Object.Name}");
                //if(cp.Name == Name)
                //{

                //}
            }
            var __params = CoverParameters.FirstOrDefault(f => f.Object.Name == Name);
            if (__params != null)
            {
                __params.Object.Value = Value;
                return true;
            }
            return false;
        }
        public bool ParameterExist(string Name)
        {
            return CoverParameters.Count(f => f.Name == Name) != 0;
        }

        private void Parameters_Changed(object sender, EventArgs e)
        {
            this.Changed();
        }

        #region ICommandBuilder Members

        string ICommandBuilder.CommandText
        {
            get { return CommandText; }
        }

        /// <summary>
        ///
        /// </summary>
        public System.Data.CommandType CommandType
        {
            get { return System.Data.CommandType.Text; }
        }

        #endregion ICommandBuilder Members

        #region IParameterCommandBuilder Members

        /// <summary>
        /// ���o�ѼƼҦ��O�_�ҥ�
        /// </summary>
        public bool ParameterEnabled
        {
            get { return ParameterMode == ParameterMode.Parameter; }
        }

        Parameter[] IParameterCommandBuilder.Parameters
        {
            get
            {
                List<Parameter> list = new List<Parameter>();
                foreach (Parameter CoverObject in Parameters)
                {
                    if (CoverObject != null)
                    {
                        list.Add(CoverObject);
                    }
                }
                return list.ToArray();
            }
        }

        #endregion IParameterCommandBuilder Members

        #region IBuildExportSupport Members

        void IBuildExportSupport.BeginBuild(ISqlBuildExport buildExport, BuildExportSupportEventArgs e)
        {
            if (this.currentBuildExport != buildExport)
            {
                this.currentBuildExport = buildExport;
                this.currentBuildExport.DbInformation = e.DbInformation;
                this.currentBuildExport.BuildOptions = e.BuildOptions;
                _changed++; // �q�� COMMAND ���e�����ʩβ���SQL COMMAND �����ܧ�
            }
        }

        void IBuildExportSupport.EndBuild()
        {
            if (this.currentBuildExport != null)
            {
                this.currentBuildExport.DbInformation = null;
                this.currentBuildExport.BuildOptions = CommandBuildOptions.None;
                this.currentBuildExport = null;
                _changed++;// �q�� COMMAND ���e�����ʩβ���SQL COMMAND �����ܧ�
            }
        }

        #endregion IBuildExportSupport Members

        /// <summary>
        ///
        /// </summary>
        public virtual void Dispose() { }
    }
}