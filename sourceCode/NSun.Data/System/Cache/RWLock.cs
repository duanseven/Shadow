using System.Threading;

namespace NSun.Data.Common
{
    internal static class RWLock
    {
        public const int DEFAULT_RWLOCK_TIMEOUT = 30000;

        #region Public Methods

        public static TResult GetWriteLock<TResult>(ReaderWriterLock lockObj, int timeout, DoWorkFunc<TResult> doWork)
        {
            var status = (lockObj.IsWriterLockHeld ? RWLockStatus.WriteLock : (lockObj.IsReaderLockHeld ? RWLockStatus.ReadLock : RWLockStatus.Unlocked));
            var writeLock = default(LockCookie);
            if( status == RWLockStatus.ReadLock )
                writeLock = lockObj.UpgradeToWriterLock(timeout);
            else if( status == RWLockStatus.Unlocked )
                lockObj.AcquireWriterLock(timeout);
            try
            {
                return doWork();
            }
            finally
            {
                if( status == RWLockStatus.ReadLock )
                    lockObj.DowngradeFromWriterLock(ref writeLock);
                else if( status == RWLockStatus.Unlocked )
                    lockObj.ReleaseWriterLock();
            }
        }

        public static TResult GetReadLock<TResult>(ReaderWriterLock lockObj, int timeout, DoWorkFunc<TResult> doWork)
        {
            var releaseLock = false;
            if( !lockObj.IsWriterLockHeld && !lockObj.IsReaderLockHeld )
            {
                lockObj.AcquireReaderLock(timeout);
                releaseLock = true;
            }
            try
            {
                return doWork();
            }
            finally
            {
                if( releaseLock )
                    lockObj.ReleaseReaderLock();
            }
        }

        #endregion

        #region Nested Classes

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        internal delegate TResult DoWorkFunc<TResult>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        internal enum RWLockStatus
        {
            Unlocked,
            ReadLock,
            WriteLock
        }

        #endregion
    }
}
