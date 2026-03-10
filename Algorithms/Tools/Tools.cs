using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Linq.Expressions;

namespace Algorithms.Tools
{
    public class Tools
    {
        #region Copy
        public static Image<Gray, byte> Copy(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Clone();
            return result;
        }

        public static Image<Bgr, byte> Copy(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            return result;
        }
        #endregion

        #region Invert
        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }
        #endregion

        #region Convert color image to grayscale image
            public static Image<Gray, byte> Convert(Image<Bgr, byte> inputImage)
            {
                Image<Gray, byte> result = inputImage.Convert<Gray, byte>();
                return result;
            }
        #endregion

        #region Binary
        public static Image<Gray, byte> Binary(Image<Gray, byte> inputImage, byte T)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    if (inputImage.Data[y, x, 0] > T)
                        result.Data[y, x, 0] = 255;
                    else
                        result.Data[y, x, 0] = 0;
                }
            }

            return result;
        }

        #endregion

        #region Mirror
        public static Image<Gray, byte> Mirror(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for(int y = 0; y < inputImage.Height; ++y)
            {
                for(int x = 0;x < inputImage.Width; ++x)
                    result.Data[y,inputImage.Width - 1 - x, 0]= inputImage.Data[y,x,0];
            }
            return result;
        }
        public static Image<Bgr, byte> Mirror(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, inputImage.Width - 1 - x, 0] = inputImage.Data[y, x, 0];
                    result.Data[y, inputImage.Width - 1 - x, 1] = inputImage.Data[y, x, 1];
                    result.Data[y, inputImage.Width - 1 - x, 2] = inputImage.Data[y, x, 2];
                }
            }
            return result;
        }
        #endregion
            
        #region Rotate Image Clockwise
        public static Image<Gray, byte> RotateImageClockwise ( Image<Gray,byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for(int y=0;y<inputImage.Height;++y)
                for(int x=0;x<inputImage.Width;++x)
                {
                    result.Data[x, inputImage.Width - 1 - y, 0] = inputImage.Data[y, x, 0];
                }
            return result;
        }
        public static Image<Bgr, byte> RotateImageClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[x, inputImage.Width - 1 - y, 0] = inputImage.Data[y, x, 0];
                    result.Data[x, inputImage.Width - 1 - y, 1] = inputImage.Data[y, x, 1];
                    result.Data[x, inputImage.Width - 1 - y, 2] = inputImage.Data[y, x, 2];
                }
            return result;
        }
        #endregion

        #region Rotate Image Anti-clockwise
        public static Image<Gray, byte> RotateImageAntiClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[inputImage.Height - 1 - x, y, 0] = inputImage.Data[y, x, 0];
                }
            return result;
        }
        public static Image<Bgr, byte> RotateImageAntiClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[inputImage.Height - 1 - x, y, 0] = inputImage.Data[y, x, 0];
                    result.Data[inputImage.Height - 1 - x, y, 1] = inputImage.Data[y, x, 1];
                    result.Data[inputImage.Height - 1 - x, y, 2] = inputImage.Data[y, x, 2];
                }
            return result;
        }
        #endregion

        #region Crop
        public static Image<Gray, byte> Crop(Image<Gray, byte> inputImage, Rectangle roi)
        {
            if (roi.X < 0 || roi.Y < 0 ||
                roi.Right > inputImage.Width ||
                roi.Bottom > inputImage.Height)
                throw new ArgumentException("ROI is outside image bounds.");

            Image<Gray, byte> result = new Image<Gray, byte>(roi.Width, roi.Height);

            for (int y = 0; y < roi.Height; ++y)
            {
                for (int x = 0; x < roi.Width; ++x)
                {
                    result.Data[y, x, 0] =
                        inputImage.Data[roi.Y + y, roi.X + x, 0];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> Crop(Image<Bgr, byte> inputImage, Rectangle roi)
        {
            if (roi.X < 0 || roi.Y < 0 ||
                roi.Right > inputImage.Width ||
                roi.Bottom > inputImage.Height)
                throw new ArgumentException("ROI is outside image bounds.");

            Image<Bgr, byte> result = new Image<Bgr, byte>(roi.Width, roi.Height);

            for (int y = 0; y < roi.Height; ++y)
            {
                for (int x = 0; x < roi.Width; ++x)
                {
                    result.Data[y, x, 0] =
                        inputImage.Data[roi.Y + y, roi.X + x, 0];
                    result.Data[y, x, 1] =
                        inputImage.Data[roi.Y + y, roi.X + x, 1];
                    result.Data[y, x, 2] =
                        inputImage.Data[roi.Y + y, roi.X + x, 2];
                }
            }

            return result;
        }
        public static void ComputeStats(Image<Gray, byte> img,
                                out double mean,
                                out double stdDev)
        {

            MCvScalar m = new MCvScalar();
            MCvScalar s = new MCvScalar();
            CvInvoke.MeanStdDev(img, ref m, ref s);

            mean = m.V0;
            stdDev = s.V0;
        }


        #endregion

    }
}