// ------------------------------------------------------------------------
// Chiats Common&Data Library V4.1.21 (2021/08)
// Chiats@Studio(http://www.chiats.com/Common)
// Design&Coding By Chia Tsang Tsai
// Copyright(C) 2005-2022 Chiats@Studio All 
// ------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Chiats
{
    /// <summary>
    /// 執行時間標記元件.
    /// </summary>
    public class ThreadTimeMark : IDisposable
    {
        /// <summary>
        /// 標記時間
        /// </summary>
        public struct ThreadElapsedTime
        {
            /// <summary>
            /// 時間建構子
            /// </summary>
            /// <param name="ElapsedTime">實際時間量</param>
            /// <param name="UserTime">取得執行緒在作業系統核心外部的使用者模式中執行所花的時間量</param>
            /// <param name="PrivilegedTime">取得執行緒在作業系統核心中執行程式碼所耗用的時間量</param>
            public ThreadElapsedTime(TimeSpan ElapsedTime, TimeSpan UserTime, TimeSpan PrivilegedTime)
            {
                this.ElapsedTime = ElapsedTime;
                this.UserTime = UserTime;
                this.PrivilegedTime = PrivilegedTime;
            }

            /// <summary>
            /// 實際時間. 從 ThreadTimeMark 建立(Rese 後) 至 Mark 的實際時間,
            /// </summary>
            public TimeSpan ElapsedTime;

            /// <summary>
            /// 取得執行緒在作業系統核心外部的使用者模式中執行所花的時間量
            /// </summary>
            public TimeSpan UserTime;

            /// <summary>
            /// 取得執行緒在作業系統核心中執行程式碼所耗用的時間量
            /// </summary>
            public TimeSpan PrivilegedTime;
        }

        /// <summary>
        /// 標記的開始時間
        /// </summary>
        public DateTime StartTime { get; private set; }

        private TimeSpan StartUserTime;
        private TimeSpan StartPrivilegedTime;
        private ProcessThread ThreadObject = null;

        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        /// <summary>
        /// 標記時間建構子
        /// </summary>
        public ThreadTimeMark()
        {
            StartTime = DateTime.Now;

            Thread.BeginThreadAffinity();
            int CurrentThreadID = GetCurrentThreadId();
            if (this.ThreadObject != null)
            {
                StartUserTime = this.ThreadObject.UserProcessorTime;
                StartPrivilegedTime = this.ThreadObject.PrivilegedProcessorTime;
            }
        }

        /// <summary>
        /// 重置標記時間
        /// </summary>
        public void Reset()
        {
            StartTime = DateTime.Now;
            if (this.ThreadObject != null)
            {
                StartUserTime = this.ThreadObject.UserProcessorTime;
                StartPrivilegedTime = this.ThreadObject.PrivilegedProcessorTime;
            }
        }

        /// <summary>
        /// 標記時間
        /// </summary>
        /// <returns></returns>
        public ThreadElapsedTime Mark()
        {
            TimeSpan UserTime = TimeSpan.Zero;
            TimeSpan PrivilegedTime = TimeSpan.Zero;
            TimeSpan ElapsedTime;
            if (this.ThreadObject != null)
            {
                UserTime = ThreadObject.UserProcessorTime - StartUserTime;
                PrivilegedTime = ThreadObject.PrivilegedProcessorTime - StartPrivilegedTime;
            }
            ElapsedTime = DateTime.Now - StartTime;
            return new ThreadElapsedTime(ElapsedTime, UserTime, PrivilegedTime);
        }

        private bool disposed;

        /// <summary>
        ///
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed ressources
                }
                Thread.EndThreadAffinity();
            }
            //dispose unmanaged ressources
            disposed = true;
        }

        /// <summary>
        /// 釋放物件及其所擁有的資源.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///
        /// </summary>
        ~ThreadTimeMark()
        {
            Dispose(false);
        }
    }
}