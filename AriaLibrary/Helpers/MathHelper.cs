using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Helpers
{
    public static class MathHelper
    {
        public static Vector3 QuaternionToEulerAngles(float w, float x, float y, float z)
        {
            // Roll (x-axis rotation)
            float sinr_cosp = 2 * (w * x + y * z);
            float cosr_cosp = 1 - 2 * (x * x + y * y);
            float roll = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // Pitch (y-axis rotation)
            float sinp = 2 * (w * y - z * x);
            float pitch;
            if (Math.Abs(sinp) >= 1)
                pitch = (float)Math.CopySign(Math.PI / 2, sinp); // Use 90 degrees if out of range
            else
                pitch = (float)Math.Asin(sinp);

            // Yaw (z-axis rotation)
            float siny_cosp = 2 * (w * z + x * y);
            float cosy_cosp = 1 - 2 * (y * y + z * z);
            float yaw = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return new Vector3(roll, pitch, yaw);
        }
    }
}
