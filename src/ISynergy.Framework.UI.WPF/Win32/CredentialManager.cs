using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Models;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Text;

namespace ISynergy.Framework.UI.Win32;

public static class CredentialManager
{
    public static Credential? ReadCredential(string resource)
    {
        var read = CredRead(resource, CredentialTypes.Generic, 0, out IntPtr nCredPtr);
        if (read)
        {
            using (CriticalCredentialHandle critCred = new CriticalCredentialHandle(nCredPtr))
            {
                CREDENTIAL cred = critCred.GetCredential();
                return ReadCredential(cred);
            }
        }

        return null;
    }

    public static Credential? ReadCredential(string resource, string username)
    {
        var read = CredRead(resource, CredentialTypes.Generic, 0, out IntPtr nCredPtr);
        if (read)
        {
            using (CriticalCredentialHandle critCred = new CriticalCredentialHandle(nCredPtr))
            {
                CREDENTIAL cred = critCred.GetCredential();

                if (Marshal.PtrToStringUni(cred.UserName) == username)
                    return ReadCredential(cred);
            }
        }

        return null;
    }

    private static Credential? ReadCredential(CREDENTIAL credential)
    {
        var resource = Marshal.PtrToStringUni(credential.TargetName);
        var username = Marshal.PtrToStringUni(credential.UserName);
        string? password = null;

        if (credential.CredentialBlob != IntPtr.Zero)
            password = Marshal.PtrToStringUni(credential.CredentialBlob, (int)credential.CredentialBlobSize / 2);

        if (!string.IsNullOrEmpty(resource) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            return new Credential(credential.Type, resource, username, password);

        return null;
    }

    public static bool DeleteCredential(string resource) =>
        CredDelete(resource, CredentialTypes.Generic, 0);


    public static int WriteCredential(string resource, string username, string password, CredentialPersistence persistence = CredentialPersistence.LocalMachine)
    {
        var byteArray = Encoding.Unicode.GetBytes(password);

        if (byteArray is not null && byteArray.Length > 512)
            throw new ArgumentOutOfRangeException("password", "The password message has exceeded 512 bytes.");

        var credential = new CREDENTIAL();
        credential.AttributeCount = 0;
        credential.Attributes = IntPtr.Zero;
        credential.Comment = IntPtr.Zero;
        credential.TargetAlias = IntPtr.Zero;
        credential.Type = CredentialTypes.Generic;
        credential.Persist = (UInt32)persistence;
        credential.CredentialBlobSize = (UInt32)Encoding.Unicode.GetBytes(password).Length;
        credential.TargetName = Marshal.StringToCoTaskMemUni(resource);
        credential.CredentialBlob = Marshal.StringToCoTaskMemUni(password);
        credential.UserName = Marshal.StringToCoTaskMemUni(username ?? Environment.UserName);

        var written = CredWrite(ref credential, 0);
        var lastError = Marshal.GetLastWin32Error();

        Marshal.FreeCoTaskMem(credential.TargetName);
        Marshal.FreeCoTaskMem(credential.CredentialBlob);
        Marshal.FreeCoTaskMem(credential.UserName);

        if (written)
            return 0;

        throw new Exception(string.Format("CredWrite failed with the error code {0}.", lastError));
    }

    public static IReadOnlyList<Credential> EnumerateCrendentials(string resource)
    {
        var result = new List<Credential>();
        var count = 0;

        if (CredEnumerate(resource, 0, out count, out IntPtr pCredentials))
        {
            for (int n = 0; n < count; n++)
            {
                var credentialPtr = Marshal.ReadIntPtr(pCredentials, n * Marshal.SizeOf(typeof(IntPtr)));
                var credential = ReadCredential((CREDENTIAL)Marshal.PtrToStructure(credentialPtr, typeof(CREDENTIAL))!);

                if (credential is not null)
                    result.Add(credential);
            }
        }

        return result;
    }

    [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern bool CredRead(string target, CredentialTypes type, int reservedFlag, out IntPtr credentialPtr);

    [DllImport("Advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern bool CredWrite([In] ref CREDENTIAL userCredential, [In] UInt32 flags);

    [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern bool CredEnumerate(string? filter, int flag, out int count, out IntPtr pCredentials);

    [DllImport("Advapi32.dll", EntryPoint = "CredFree", SetLastError = true)]
    static extern bool CredFree([In] IntPtr cred);

    [DllImport("Advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern bool CredDelete(string target, CredentialTypes type, int reservedFlag);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CREDENTIAL
    {
        public UInt32 Flags;
        public CredentialTypes Type;
        public IntPtr TargetName;
        public IntPtr Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public UInt32 CredentialBlobSize;
        public IntPtr CredentialBlob;
        public UInt32 Persist;
        public UInt32 AttributeCount;
        public IntPtr Attributes;
        public IntPtr TargetAlias;
        public IntPtr UserName;
    }

    sealed class CriticalCredentialHandle : CriticalHandleZeroOrMinusOneIsInvalid
    {
        public CriticalCredentialHandle(IntPtr preexistingHandle)
        {
            SetHandle(preexistingHandle);
        }

        public CREDENTIAL GetCredential()
        {
            if (!IsInvalid)
            {
                var credential = (CREDENTIAL)Marshal.PtrToStructure(handle, typeof(CREDENTIAL))!;
                return credential;
            }

            throw new InvalidOperationException("Invalid CriticalHandle!");
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                CredFree(handle);
                SetHandleAsInvalid();
                return true;
            }

            return false;
        }
    }
}
