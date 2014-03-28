using System;
//using System.Linq;
//using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace VAIPHO
{
    /// <summary>
    /// Identifies the phone number type specified.
    /// </summary>
    public enum AddressType
    {
        /// <summary>Unknown phone number type.</summary>
        Unknown,
        /// <summary>International phone number.</summary>
        International,
        /// <summary>National phone number.</summary>
        National,
        /// <summary>Network-specific phone number.</summary>
        NetworkSpecific,
        /// <summary>Subscriber phone number.</summary>
        Subscriber,
        /// <summary>Alphanumeric phone number.</summary>
        Alphanumeric,
        /// <summary>Abbreviated phone number.</summary>
        Abbreviated
    }

    /// <summary>
    /// Information about the phone number.
    /// </summary>
    public struct PhoneAddress
    {
        /// <summary>The address type.</summary>
        public AddressType AddressType;
        /// <summary>The phone number in string format.</summary>
        public String Address;
    }

    /// <summary>
    /// Phone control.
    /// </summary>
    public class Phone
    {
       // private static long PMCF_DEFAULT = 0x00000001;
      //  private static long PMCF_PROMPTBEFORECALLING = 0x00000002;

      /*  private struct PhoneMakeCallInfo
        {
            public IntPtr cbSize;
            public IntPtr dwFlags;
            public IntPtr pszDestAddress;
            public IntPtr pszAppName;
            public IntPtr pszCalledParty;
            public IntPtr pszComment;
        }

        [DllImport("phone.dll")]
        private static extern IntPtr PhoneMakeCall(ref PhoneMakeCallInfo ppmci);*/

        /// <summary>
        /// Dials the specified phone number.
        /// </summary>
        /// <param name="PhoneNumber">Phone number to dial.</param>
     /*   public static void MakeCall(string PhoneNumber)
        {
            MakeCall(PhoneNumber, false);
        }*/

        /// <summary>
        /// Dials the specified phone number.
        /// </summary>
        /// <param name="PhoneNumber">Phone number to dial.</param>
        /// <param name="PromptBeforeCall">Prompts the user before the call is placed.</param>
      /*  unsafe public static void MakeCall(string PhoneNumber, bool PromptBeforeCall)
        {
            IntPtr res;

            PhoneNumber += '\0';
            char[] cPhoneNumber = PhoneNumber.ToCharArray();

            fixed (char* pAddr = cPhoneNumber)
            {
                PhoneMakeCallInfo info = new PhoneMakeCallInfo();
                info.cbSize = (IntPtr)Marshal.SizeOf(info);
                info.pszDestAddress = (IntPtr)pAddr;

                if (PromptBeforeCall)
                {
                    info.dwFlags = (IntPtr)PMCF_PROMPTBEFORECALLING;
                }
                else
                {
                    info.dwFlags = (IntPtr)PMCF_DEFAULT;
                }

                res = PhoneMakeCall(ref info);
            }
        }*/
    }

    /// <summary>
    /// Reads information from the Subscriber Identity Module (SIM)
    /// </summary>
    public class Sim
    {
        private static long SERVICE_PROVIDER = 0x00006F46;

        [StructLayout(LayoutKind.Sequential)]
        private struct SimRecord
        {
            public IntPtr cbSize;
            public IntPtr dwParams;
            public IntPtr dwRecordType;
            public IntPtr dwItemCount;
            public IntPtr dwSize;
        }

        [DllImport("sms.dll")]
        private static extern IntPtr SmsGetPhoneNumber(IntPtr psmsaAddress);

        [DllImport("cellcore.dll")]
        private static extern IntPtr SimInitialize(IntPtr dwFlags, IntPtr lpfnCallBack, IntPtr dwParam, out IntPtr lphSim);

        [DllImport("cellcore.dll")]
        private static extern IntPtr SimGetRecordInfo(IntPtr hSim, IntPtr dwAddress, ref SimRecord lpSimRecordInfo);

        [DllImport("cellcore.dll")]
        private static extern IntPtr SimReadRecord(IntPtr hSim, IntPtr dwAddress, IntPtr dwRecordType, IntPtr dwIndex, byte[] lpData, IntPtr dwBufferSize, ref IntPtr lpdwBytesRead);

        [DllImport("cellcore.dll")]
        private static extern IntPtr SimDeinitialize(IntPtr hSim);

        /// <summary>
        /// Gets the phone number from the SIM.
        /// </summary>
        /// <returns>PhoneAddress structure with phone number.</returns>
        unsafe public static PhoneAddress GetPhoneNumber()
        {
            PhoneAddress phoneaddr = new PhoneAddress();

            Byte[] buffer = new Byte[516];
            fixed (byte* pAddr = buffer)
            {
                IntPtr res = SmsGetPhoneNumber((IntPtr)pAddr);
                if (res != IntPtr.Zero)
                    throw new Exception("Could not get phone number from SIM");

                byte* pCurrent = pAddr;
                phoneaddr.AddressType = (AddressType)Marshal.ReadInt32((IntPtr)pCurrent);
                pCurrent += Marshal.SizeOf(phoneaddr.AddressType);
                phoneaddr.Address = Marshal.PtrToStringUni((IntPtr)pCurrent);
            }

            return phoneaddr;
        }

        /// <summary>
        /// Gets the current wireless carriers network name from the SIM.
        /// </summary>
        /// <returns>The carrier description.</returns>
        public static string GetServiceProvider()
        {
            IntPtr hSim, res;

            res = SimInitialize(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, out hSim);
            if (res != IntPtr.Zero)
                throw new Exception("Could not initialize SIM");

            SimRecord rec = new SimRecord();
            rec.cbSize = (IntPtr)Marshal.SizeOf(rec);

            res = SimGetRecordInfo(hSim, (IntPtr)SERVICE_PROVIDER, ref rec);
            if (res != IntPtr.Zero)
                throw new Exception("Could not read the service provider information from the SIM");

            byte[] bData = new byte[(int)rec.dwSize + 1];
            IntPtr dwBytesRead = IntPtr.Zero;

            res = SimReadRecord(hSim, (IntPtr)SERVICE_PROVIDER, rec.dwRecordType, IntPtr.Zero, bData, (IntPtr)bData.Length, ref dwBytesRead);
            if (res != IntPtr.Zero)
                throw new Exception("Could not read the service provider from the SIM");

            byte[] bScrubbed = new byte[(int)dwBytesRead];
            int nPos = 0;

            // Scrub the non-ascii characters
            for (int i = 0; i < (int)dwBytesRead; i++)
            {
                if (((int)bData[i] > 19) && ((int)bData[i] < 125))
                {
                    bScrubbed[nPos] = bData[i];
                    nPos++;
                }
            }

            SimDeinitialize(hSim);

            return Encoding.ASCII.GetString(bScrubbed, 0, bScrubbed.Length);
        }
    }
}
