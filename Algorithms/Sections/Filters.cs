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
        public static readonly double[,] Laplace1 =
        {
            { 0, -1,  0 },
            { -1, 4, -1 },
            { 0, -1,  0 }
        };

        public static readonly double[,] Laplace2 =
        {
            { -1, -1, -1 },
            { -1,  8, -1 },
            { -1, -1, -1 }
        };

        public static int[,] ApplyFilterGraySigned(Image<Gray, byte> inputImage, double[,] filterMask)
        {
            int width = inputImage.Width;
            int height = inputImage.Height;

            int[,] result = new int[height, width];

            int h = filterMask.GetLength(0);
            int w = filterMask.GetLength(1);
            int half_h = h / 2;
            int half_w = w / 2;

            var borderImage = new Image<Gray, byte>(
                width + 2 * half_w,
                height + 2 * half_h);

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
                    double sum = 0.0;

                    for (int i = -half_h; i <= half_h; i++)
                    {
                        for (int j = -half_w; j <= half_w; j++)
                        {
                            double val = borderImage.Data[y + i, x + j, 0];
                            sum += filterMask[i + half_h, j + half_w] * val;
                        }
                    }

                    result[y - half_h, x - half_w] = (int)Math.Round(sum);
                }
            }

            return result;
        }


        public static Image<Gray, byte> HighlightLaplaceValues(Image<Gray, byte> inputImage, int laplaceVariant, double sigma = 1.0)
        {
            Image<Gray, byte> smoothed = GaussFiltering(inputImage, sigma, sigma);
            double[,] laplaceMask = laplaceVariant == 1 ? Laplace1 : Laplace2;

            int[,] signedResult = ApplyFilterGraySigned(smoothed, laplaceMask);

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    int value = signedResult[y, x] + 128;
                    value = Max(0, Min(255, value));
                    result.Data[y, x, 0] = (byte)value;
                }
            }

            return result;
        }

        public static Image<Gray, byte> LaplaceZeroCrossing(Image<Gray, byte> inputImage, int laplaceVariant, int threshold, double sigma = 1.0)
        {
            Image<Gray, byte> smoothed = GaussFiltering(inputImage, sigma, sigma);
            double[,] laplaceMask = laplaceVariant == 1 ? Laplace1 : Laplace2;

            int[,] laplaceValues = ApplyFilterGraySigned(smoothed, laplaceMask);

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            result.SetZero();

            for (int y = 1; y < inputImage.Height - 1; y++)
            {
                for (int x = 1; x < inputImage.Width - 1; x++)
                {
                    if (IsZeroCrossing(laplaceValues, x, y, threshold))
                        result.Data[y, x, 0] = 255;
                    else
                        result.Data[y, x, 0] = 0;
                }
            }

            return result;
        }

        private static bool IsZeroCrossing(int[,] img, int x, int y, int T)
        {
            if (CheckTriplet(
                img[y, x - 1],    
                img[y - 1, x - 1],  
                img[y - 1, x],      
                img[y + 1, x],      
                img[y + 1, x + 1],  
                img[y, x + 1],      
                T))
                return true;

            
            if (CheckTriplet(
                img[y + 1, x],      
                img[y + 1, x + 1],  
                img[y, x + 1],      
                img[y, x - 1],      
                img[y - 1, x - 1],  
                img[y - 1, x],      
                T))
                return true;

           
            if (CheckTriplet(
                img[y - 1, x],      
                img[y - 1, x + 1],  
                img[y, x + 1],      
                img[y, x - 1],      
                img[y + 1, x - 1],  
                img[y + 1, x],      
                T))
                return true;

            if (CheckTriplet(
                img[y, x - 1],      
                img[y + 1, x - 1],  
                img[y + 1, x],      
                img[y - 1, x],      
                img[y - 1, x + 1],  
                img[y, x + 1],      
                T))
                return true;

            return false;
        }

        private static bool CheckTriplet(int A1, int A2, int A3, int B1, int B2, int B3, int T)
        {
            bool p1 = (A1 >= T && B1 <= -T) || (A1 <= -T && B1 >= T);
            bool p2 = (A2 >= T && B2 <= -T) || (A2 <= -T && B2 >= T);
            bool p3 = (A3 >= T && B3 <= -T) || (A3 <= -T && B3 >= T);

            return p1 || p2 || p3;
        }
    }
}
