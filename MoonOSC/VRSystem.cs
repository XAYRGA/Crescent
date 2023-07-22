using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OVRSharp;

using Valve.VR;
using System.Numerics;

namespace MoonOSC
{
    static internal class VRSystem
    {

        public static Application VRApp;
        public static Valve.VR.CVRSystem VRSys;
        private static TrackedDevicePose_t[] PosesLastFrame = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];

        private static ETrackedPropertyError lastError;

        public static void Start()
        {
            VRApp = new Application(Application.ApplicationType.Background);
            VRSys = VRApp.OVRSystem;
        }



        public static void Update()
        {
            VRSys.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseRawAndUncalibrated, 0, PosesLastFrame);
        }


        internal static void QuaternionFromMatrix(HmdMatrix34_t m, ref HmdQuaternion_t q)
        {
            q.w = Math.Sqrt(1f + m.m0 + m.m5 + m.m10) / 2.0f; // Scalar
            q.x = (m.m9 - m.m6) / (4 * q.w);
            q.y = (m.m2 - m.m8) / (4 * q.w);
            q.z = (m.m4 - m.m1) / (4 * q.w);
        }

        public static Vector3 RotationMatrixToYPR(HmdMatrix34_t m)
        {
            var v = new Vector3(0);
            HmdQuaternion_t q = new HmdQuaternion_t();
            QuaternionFromMatrix(m, ref q);

            double test = q.x * q.y + q.z * q.w;
            if (test > 0.499f)
            { // singularity at north pole
                v.X = (float)(2 * Math.Atan2(q.x, q.w)); // heading
                v.Y = (float)(Math.PI / 2f); // attitude
                v.Z = 0; // bank
                return v;
            }
            if (test < -0.499f)
            { // singularity at south pole
                v.X = (float)(-2f * Math.Atan2(q.x, q.w)); // headingq
                v.Y = -(float)(Math.PI / 2f); // attitude
                v.Z = 0; // bank
                return v;
            }
            double sqx = q.x * q.x;
            double sqy = q.y * q.y;
            double sqz = q.z * q.z;
            v.X = (float)Math.Atan2(2f * q.y * q.w - 2f * q.x * q.z, 1f - 2f * sqy - 2f * sqz); // heading
            v.Y = (float)Math.Asin(2f * test); // attitude
            v.Z = (float)Math.Atan2(2f * q.x * q.w - 2f * q.y * q.z, 1f - 2f * sqx - 2f * sqz); // bank

            return v;
        }

        private static float RAD2DEG(float rad)
        {
            var deg = (float)(rad / Math.PI) * 180f;
            if (deg < 0)
                return deg + 360;
            return deg;
        }


        public static Vector3 GetTrackerVelocity(int tracker)
        {
            var rtn = new Vector3(0f);
            if (tracker > OpenVR.k_unMaxTrackedDeviceCount)
                return rtn;

            var hmdTrkData = PosesLastFrame[tracker];
            if (hmdTrkData.bPoseIsValid && hmdTrkData.bDeviceIsConnected)
            {
                var hmdVelData = hmdTrkData.vVelocity;
                rtn.X = hmdVelData.v0;
                rtn.Y = hmdVelData.v1;
                rtn.Z = hmdVelData.v2;
            }
            return rtn;
        }

        public static Vector3 GetTrackerPosition(int tracker)
        {
            var rtn = new Vector3(0f);
            if (tracker > OpenVR.k_unMaxTrackedDeviceCount)
                return rtn;

            var hmdTrkData = PosesLastFrame[tracker];
            if (hmdTrkData.bPoseIsValid && hmdTrkData.bDeviceIsConnected)
            {
                var hmdVelData = hmdTrkData.mDeviceToAbsoluteTracking;

                rtn.X = hmdVelData.m3;
                rtn.Y = hmdVelData.m7;
                rtn.Z = hmdVelData.m11;
            }
            return rtn;
        }

        public static Vector3 GetTrackerRotation(int tracker)
        {
            var rtn = new Vector3(0f);
            if (tracker > OpenVR.k_unMaxTrackedDeviceCount)
                return rtn;

            var hmdTrkData = PosesLastFrame[tracker];

            if (hmdTrkData.bPoseIsValid && hmdTrkData.bDeviceIsConnected)
            {
                var hmdMtxData = hmdTrkData.mDeviceToAbsoluteTracking;
                var hmdRot = RotationMatrixToYPR(hmdMtxData);
                rtn.X = RAD2DEG(hmdRot.X);
                rtn.Y = RAD2DEG(hmdRot.Y);
                rtn.Z = RAD2DEG(hmdRot.Z);
                rtn = hmdRot;
            }
            return rtn;
        }

        public static string GetTrackerSerialNumber(uint tracker)
        {
            string rtn = null;
            if (tracker > OpenVR.k_unMaxTrackedDeviceCount)
                return rtn;
            StringBuilder data = new StringBuilder();
            VRSys.GetStringTrackedDeviceProperty(tracker, ETrackedDeviceProperty.Prop_SerialNumber_String, data, 32, ref lastError);
            rtn = data.ToString();
            return rtn;
        }


        private static uint GetControllerByRole(ETrackedControllerRole ctrlRole)
        {
            for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                var devClass = VRSys.GetTrackedDeviceClass(i);
                if (devClass != ETrackedDeviceClass.Controller)
                    continue;
                var role = VRSys.GetControllerRoleForTrackedDeviceIndex(i);
                if (role != ctrlRole)
                    continue;
                return i;
            }
            return 0xFFFFFFFF;
        }

        public static uint GetHMD()
        {
            return 0;
        }

        public static uint GetLeftHand()
        {
            return GetControllerByRole(ETrackedControllerRole.LeftHand);
        }


        public static uint GetRightHand()
        {
            return GetControllerByRole(ETrackedControllerRole.RightHand);
        }



    }
}