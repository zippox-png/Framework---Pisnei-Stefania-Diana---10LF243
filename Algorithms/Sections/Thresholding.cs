using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace Algorithms.Sections
{
    public class Thresholding
    {
        public static byte OtsuThreshold(Image<Gray, byte> inputImage)
        {
            int[] histogram = new int[256];

            int width = inputImage.Width;
            int height = inputImage.Height;
            int n = width * height;


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte value = inputImage.Data[y, x, 0];
                    histogram[value]++;
                }
            }
            
            
            double[] p = new double[256];

            for (int k = 0; k < 256; k++)
            {
                p[k] = (double)histogram[k] / n;
            }

            byte T = 0;
            double maxInterVariance = 0.0;

            for (int t = 1; t <= 254; t++)
            {
                double P1 = 0.0, P2 = 0.0;
                double sum1 = 0.0, sum2 = 0.0;

                for (int k = 0; k <= t; k++)
                {
                    P1 += p[k];
                    sum1 += k * p[k];
                }

                P2 = 1.0 - P1;

                for (int k = t + 1; k <= 255; k++)
                {
                    sum2 += k * p[k];
                }

                if (P1 == 0 || P2 == 0)
                    continue;

                double mu1 = sum1 / P1;
                double mu2 = sum2 / P2;

                double interVariance = P1 * P2 * (mu1 - mu2) * (mu1 - mu2);

                if (interVariance > maxInterVariance)
                {
                    maxInterVariance = interVariance;
                    T = (byte)t;
                }
            }

            return T;
        }
    }
}