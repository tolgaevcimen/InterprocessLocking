using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace InterprocessLocking
{
    public class MutexLock : IDisposable
    {
        Mutex _Mutex { get; set; }
        bool HasHandle { get; set; }

        public MutexLock ( string handle )
        {
            var createdNew = false;
            try
            {
                // avoid race condition for security reasons
                var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
                var securitySettings = new MutexSecurity();
                securitySettings.AddAccessRule(allowEveryoneRule);

                _Mutex = new Mutex(false, "Global:\\" + handle, out createdNew, securitySettings);
                HasHandle = _Mutex.WaitOne();
            }
            catch ( AbandonedMutexException )
            {
                HasHandle = true;
            }
        }

        public void Dispose ()
        {
            if ( _Mutex == null ) return;
            // lock will be disposed
            if ( HasHandle )
                _Mutex.ReleaseMutex();
            _Mutex.Dispose();
            // lock disposed
        }
    }
}
