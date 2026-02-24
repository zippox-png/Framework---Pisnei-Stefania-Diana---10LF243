using Algorithms.Utilities;
using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace Algorithms.Sections
{
    public class PointwiseOperations
    {

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



    }
}