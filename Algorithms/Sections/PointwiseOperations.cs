using Algorithms.Utilities;
using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace Algorithms.Sections
{
    public class PointwiseOperations
    {

        #region Gama
        public static byte[] CreateLinearOpLUT(float alpha, float beta)
        {
            byte[] table = new byte[256];

            for (int r = 0; r < 256; r++)
            {
                table[r] = Utils.Clamp(alpha * r + beta);
            }

            return table;
        }

        public static byte[] CreateGammaLUT(float gamma)
        {   
            byte[] table = new byte[256];

            for (int r = 0; r < 256; r++)
            {
                float normalized = r / 255f;
                float corrected = (float)Math.Pow(normalized, gamma);
                float value = 255f * corrected;

                table[r] = Utils.Clamp(value);
            }

            return table;
        }

        public static Image<Gray, byte> ApplyLUTGray(Image<Gray, byte> inputImage, byte[] lut)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = lut[inputImage.Data[y, x, 0]];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> ApplyLUTColor(Image<Bgr, byte> inputImage, byte[] lut)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        result.Data[y, x, c] = lut[inputImage.Data[y, x, c]];
                    }
                }
            }

            return result;
        }
        #endregion

        #region Piecewise Linear Contrast
        public static Image<Gray, byte> PiecewiseLinear(Image<Gray, byte> inputImage, int r1, int r2, int s1, int s2)
        {
            if (!(0 <= r1 && r1 < r2 && r2 <= 255 && 0 <= s1 && s1 < s2 && s2 <= 255))
                throw new ArgumentException("Invalid r1,r2,s1,s2 values.");

            byte[] LUT = new byte[256];

            double d1 = r1 != 0 ? (double)s1 / r1 : 0;
            double d2 = (double)(s2 - s1) / (r2 - r1);
            double d3 = (r2 != 255) ? (double)(255 - s2) / (255 - r2) : 0;

            for (int i = 0; i <= 255; i++)
            {
                if (i < r1)
                    LUT[i] = (byte)Math.Round(d1 * i);
                else if (i <= r2)
                    LUT[i] = (byte)Math.Round(d2 * (i - r1) + s1);
                else
                    LUT[i] = (byte)Math.Round(d3 * (i - r2) + s2);
            }

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
                for (int x = 0; x < inputImage.Width; x++)
                    result.Data[y, x, 0] = LUT[inputImage.Data[y, x, 0]];

            return result;
        }

        public static Image<Bgr, byte> PiecewiseLinear(Image<Bgr, byte> inputImage, int r1, int r2, int s1, int s2)
        {
            if (!(0 <= r1 && r1 < r2 && r2 <= 255 && 0 <= s1 && s1 < s2 && s2 <= 255))
                throw new ArgumentException("Invalid r1,r2,s1,s2 values.");

            byte[] LUT = new byte[256];
            double d1 = r1 != 0 ? (double)s1 / r1 : 0;
            double d2 = (double)(s2 - s1) / (r2 - r1);
            double d3 = (r2 != 255) ? (double)(255 - s2) / (255 - r2) : 0;

            for (int i = 0; i <= 255; i++)
            {
                if (i < r1)
                    LUT[i] = (byte)Math.Round(d1 * i);
                else if (i <= r2)
                    LUT[i] = (byte)Math.Round(d2 * (i - r1) + s1);
                else
                    LUT[i] = (byte)Math.Round(d3 * (i - r2) + s2);
            }

            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = LUT[inputImage.Data[y, x, 0]];
                    result.Data[y, x, 1] = LUT[inputImage.Data[y, x, 1]];
                    result.Data[y, x, 2] = LUT[inputImage.Data[y, x, 2]];
                }

            return result;
        }
        #endregion

    }
}