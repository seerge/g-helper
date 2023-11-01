using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HidLibrary;

namespace GHelper.USB
{
    // Reference : thanks to https://github.com/RomanYazvinsky/ for initial discovery of XGM payloads
    public static class XGM
    {
        private static int Set(byte[] msg)
        {

            //Logger.WriteLine("XGM Payload :" + BitConverter.ToString(msg));

            var payload = new byte[300];
            Array.Copy(msg, payload, msg.Length);

            foreach (HidDevice device in Device.GetHidDevices(new int[] { 0x1970 }, 300))
            {
                device.OpenDevice();
                Logger.WriteLine("XGM " + device.Attributes.ProductHexId + "|" + device.Capabilities.FeatureReportByteLength + ":" + BitConverter.ToString(msg));
                device.WriteFeatureData(payload);
                device.CloseDevice();
                //return 1;
            }

            return 0;
        }
        public static void Init()
        {
            byte[] ASUS_INIT = Encoding.ASCII.GetBytes("^ASUS Tech.Inc.");

            Set(ASUS_INIT);

            /*
            SetXGM(new byte[] { 0x5e, 0xd0, 0x02 });
            SetXGM(new byte[] { 0x5e, 0xd0, 0x03 });
            SetXGM(ASUS_INIT);
            SetXGM(new byte[] { 0x5e, 0xd1, 0x02 }); // reset fan
            SetXGM(ASUS_INIT);
            SetXGM(new byte[] { 0x5e, 0xce, 0x03 }); 
            SetXGM(new byte[] { 0x5e, 0xd0, 0x04 });
            SetXGM(new byte[] { 0x5e, 0xd0, 0x01 });
            */
        }

        public static void ApplyLight(bool status)
        {
            Set(new byte[] { 0x5e, 0xc5, status ? (byte)0x50 : (byte)0 });
        }


        public static int Reset()
        {
            return Set(new byte[] { 0x5e, 0xd1, 0x02 });
        }

        public static int SetFan(byte[] curve)
        {

            if (AsusACPI.IsInvalidCurve(curve)) return -1;

            //InitXGM();

            byte[] msg = new byte[19];
            Array.Copy(new byte[] { 0x5e, 0xd1, 0x01 }, msg, 3);
            Array.Copy(curve, 0, msg, 3, curve.Length);

            return Set(msg);
        }
    }
}
