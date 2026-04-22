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
        public static Vector3 QuaternionToEulerAngles(float x, float y, float z, float w)
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
        public static Vector4 EulerAnglesToQuaternion(float x, float y, float z)
        {
            float x_r = x * (float)Math.PI / 180.0f;
            float y_r = y * (float)Math.PI / 180.0f;
            float z_r = z * (float)Math.PI / 180.0f;

            float qx = (float)(Math.Sin(x_r / 2) * Math.Cos(y_r / 2) * Math.Cos(z_r / 2) -
                 Math.Cos(x_r / 2) * Math.Sin(y_r / 2) * Math.Sin(z_r / 2));
            float qy = (float)(Math.Cos(x_r / 2) * Math.Sin(y_r / 2) * Math.Cos(z_r / 2) +
                 Math.Sin(x_r / 2) * Math.Cos(y_r / 2) * Math.Sin(z_r / 2));
            float qz = (float)(Math.Cos(x_r / 2) * Math.Cos(y_r / 2) * Math.Sin(z_r / 2) -
                 Math.Sin(x_r / 2) * Math.Sin(y_r / 2) * Math.Cos(z_r / 2));
            float qw = (float)(Math.Cos(x_r / 2) * Math.Cos(y_r / 2) * Math.Cos(z_r / 2) +
                 Math.Sin(x_r / 2) * Math.Sin(y_r / 2) * Math.Sin(z_r / 2));


            return new Vector4(qx, qy, qz, qw);
        }
    }
}
