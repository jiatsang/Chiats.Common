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
    /// �� SqlModel ����إߤ@�� BuildExport �䴩����Ψ�ϥνd��.
    /// </summary>
    /// <remarks>
    /// SqlModel �b���� SQL Command ����, �|�� BuildExport ����@����ڿ�X������(�i�H�����P��Ʈw�k�ƦU��BuildExport ����)
    /// ���O SqlModel �b���� SQL Command , �|�g�Ѥ��P�� Export �Ϭq�զ�. �p Select/Where/From �������P���Ϭq�Ҳզ�.
    /// �p�G�����w�n�Ψ��@�� BuildExport ����. �h�|�H�w�]�� DefaultBuildExport. ���p�G�n���w�ۤv�S�w BuildExport ����άO�ݭn buildFlags
    /// �Ѽƥ[�J�ɫh�i�H�ϥ� IBuildExportSupport �����i��, �Ӥ��P�� Export �Ϭq�N�|�b�@�� BeginBuild �� EndBuild �d�򤺰���.
    /// </remarks>
    public interface IBuildExportSupport
    {
        /// <summary>
        /// BuildExport �䴩���󪺶}�l�I
        /// </summary>
        /// <param name="BuildExport"></param>
        /// <param name="e"></param>
        void BeginBuild(ISqlBuildExport BuildExport, BuildExportSupportEventArgs e);

        /// <summary>
        /// BuildExport �䴩���󪺵����I
        /// </summary>
        void EndBuild();
    }

    /// <summary>
    /// IBuildExportSupport �� BeginBuild �޼ƪ���
    /// </summary>
    public class BuildExportSupportEventArgs : EventArgs
    {
        /// <summary>
        /// IBuildExportSupport �� BeginBuild �޼ƪ���غc�l
        /// </summary>
        /// <param name="DbInformation">���o��Ʈw�����Ѽ�</param>
        /// <param name="BuildOptions">���ܨ� SQL Commnad ����r�ԭz�ﶵ�ѼƳB�z SQL Commnad ����r��X</param>
        public BuildExportSupportEventArgs(IDbInformation DbInformation, CommandBuildOptions BuildOptions)
        {
            this.DbInformation = DbInformation;
            this.BuildOptions = BuildOptions;
        }

        /// <summary>
        /// ���o��Ʈw�����Ѽ�. �� DbInformation �� NULL ��, �h��ܦ]�L�k�ѧO��Ʈw�t�өΪ����ɦӨ������Ʈw�����Ѽ�. �]����Ʈw�����ѼƬO�M��Ʈw�t�Τ��P�Ӧ��Ҥ��P
        /// </summary>
        public readonly IDbInformation DbInformation;

        /// <summary>
        /// ���ܨ� SQL Commnad ����r�ԭz�ﶵ�ѼƳB�z SQL Commnad ����r��X
        /// </summary>
        public readonly CommandBuildOptions BuildOptions;
    }
}