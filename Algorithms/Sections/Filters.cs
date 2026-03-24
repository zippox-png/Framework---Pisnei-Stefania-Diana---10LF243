using Emgu.CV;
using Emgu.CV.Structure;
using System;
using static System.Math;

namespace Algorithms.Sections
{
    public class Filters
    {
        public static Image<TColor, TDepth> ApplyFilter<TColor, TDepth>(Image<TColor, TDepth> inputImage, double[,] filterMask)
            where TColor : struct, IColor
            where TDepth : new()
        {
            bool isByte = typeof(TDepth) == typeof(byte);

            Image<TColor, TDepth> result = new Image<TColor, TDepth>(inputImage.Size);

            int h = filterMask.GetLength(0);
            int w = filterMask.GetLength(1);
            int half_h = h / 2;
            int half_w = w / 2;

            var borderImage = new Image<TColor, TDepth>(
                inputImage.Width + 2 * half_w,
                inputImage.Height + 2 * half_h);

            CvInvoke.CopyMakeBorder(
                inputImage,
                borderImage,
                half_h,
                half_h,
                half_w,
                half_w,
                Emgu.CV.CvEnum.BorderType.Replicate);

            for (int y = half_h; y < borderImage.Height - half_h; y++)
            {
                for (int x = half_w; x < borderImage.Width - half_w; x++)
                {
                    for (int channel = 0; channel < borderImage.NumberOfChannels; channel++)
                    {
                        double sum = 0.0;

                        for (int i = -half_h; i <= half_h; i++)
                        {
                            for (int j = -half_w; j <= half_w; j++)
                            {
                                double val = Convert.ToDouble(borderImage.Data[y + i, x + j, channel]);
                                sum += filterMask[i + half_h, j + half_w] * val;
                            }
                        }

                        if (isByte)
                        {
                            byte clampedSum = (byte)Max(0, Min(255, sum + 0.5));
                            result.Data[y - half_h, x - half_w, channel] =
                                (TDepth)Convert.ChangeType(clampedSum, typeof(TDepth));
                        }
                        else
                        {
                            result.Data[y - half_h, x - half_w, channel] =
                                (TDepth)Convert.ChangeType(sum, typeof(TDepth));
                        }
                    }
                }
            }

            return result;
        }

        public static Image<Gray, byte> ApplyFilterGray(Image<Gray, byte> inputImage, double[,] filterMask)
        {
            return ApplyFilter<Gray, byte>(inputImage, filterMask);
        }

        public static Image<Bgr, byte> ApplyFilterColor(Image<Bgr, byte> inputImage, double[,] filterMask)
        {
            return ApplyFilter<Bgr, byte>(inputImage, filterMask);
        }

        public static double[,] GaussMask(double q)
        {
            if (q <= 0)
            {
                return new double[,] { { 1.0 } };
            }

            int l = (int)Math.Ceiling(4 * q);

            if (l % 2 == 0)
                l++;

            double[,] mask = new double[1, l];
            int half = l / 2;
            double sum = 0.0;

            for (int z = -half; z <= half; z++)
            {
                double value = (1.0 / (q * Math.Sqrt(2.0 * Math.PI))) *
                               Math.Exp(-(z * z) / (2.0 * q * q));

                mask[0, z + half] = value;
                sum += value;
            }

            for (int i = 0; i < l; i++)
            {
                mask[0, i] /= sum;
            }

            return mask;
        }

        public static double[,] TransposeMask(double[,] mask)
        {
            int h = mask.GetLength(0);
            int w = mask.GetLength(1);

            double[,] transposed = new double[w, h];

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    transposed[j, i] = mask[i, j];
                }
            }

            return transposed;
        }

        public static Image<Gray, byte> GaussFiltering(Image<Gray, byte> inputImage, double qx, double qy)
        {
            double[,] maskX = GaussMask(qx);
            Image<Gray, byte> tempImage = ApplyFilter(inputImage, maskX);

            double[,] maskY = TransposeMask(GaussMask(qy));
            Image<Gray, byte> resultImage = ApplyFilter(tempImage, maskY);

            return resultImage;
        }

        public static Image<Bgr, byte> GaussFiltering(Image<Bgr, byte> inputImage, double qx, double qy)
        {
            double[,] maskX = GaussMask(qx);
            Image<Bgr, byte> tempImage = ApplyFilter(inputImage, maskX);

            double[,] maskY = TransposeMask(GaussMask(qy));
            Image<Bgr, byte> resultImage = ApplyFilter(tempImage, maskY);

            return resultImage;
        }
    }
}