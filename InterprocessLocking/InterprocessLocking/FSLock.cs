using System;
using System.IO;
using System.Threading;

namespace InterprocessLocking
{
    public class FSLock : IDisposable
    {
        public int TryoutPeriod = 30;

        FileStream LockedStream { get; set; }

        public FSLock ( string handle )
        {
            while ( LockedStream == null )
            {
                try
                {
                    //lock will be attempted to be acquired
                    LockedStream = new FileStream(handle, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                    //lock acquired
                }
                catch ( IOException )
                {
                    // could not acquire the lock, wait
                    Thread.Sleep(TryoutPeriod);
                }
                catch
                {
                    throw;
                }
            }
        }

        public void Dispose ()
        {
            // lock will be disposed
            LockedStream.Close();
            // lock disposed
        }
    }
}
