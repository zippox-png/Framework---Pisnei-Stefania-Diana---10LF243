using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;

namespace Algorithms.Sections
{
    public class MorphologicalOperations
    {
        public static Image<Gray, byte> Binarize(Image<Gray, byte> inputImage, int T)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    if (inputImage.Data[y, x, 0] >= T)
                        result.Data[y, x, 0] = 255;
                    else
                        result.Data[y, x, 0] = 0;
                }
            }
            return result;
        }

        public static Image<Gray, byte> DilateLogic(Image<Gray, byte> binaryImage, int h, int w, int option)
        {
            int h2 = h / 2;
            int w2 = w / 2;

            Image<Gray, byte> borderedImage = new Image<Gray, byte>(binaryImage.Width + 2 * w2, binaryImage.Height + 2 * h2);
            CvInvoke.CopyMakeBorder(binaryImage, borderedImage, h2, h2, w2, w2, Emgu.CV.CvEnum.BorderType.Replicate);

            Image<Gray, byte> result = new Image<Gray, byte>(binaryImage.Size);

            for (int y = 0; y < binaryImage.Height; y++)
            {
                for (int x = 0; x < binaryImage.Width; x++)
                {
                    bool conditionMet = false;
                    for (int i = -h2; i <= h2; i++)
                    {
                        for (int j = -w2; j <= w2; j++)
                        {
                            byte neighbor = borderedImage.Data[y + h2 + i, x + w2 + j, 0];
                            if (option == 1 && neighbor == 255) conditionMet = true;
                            if (option == 0 && neighbor == 0) conditionMet = true;
                        }
                    }
                    if (option == 1) result.Data[y, x, 0] = conditionMet ? (byte)255 : (byte)0;
                    else result.Data[y, x, 0] = conditionMet ? (byte)0 : (byte)255;
                }
            }
            return result;
        }

        public static Image<Gray, byte> ErodeLogic(Image<Gray, byte> binaryImage, int h, int w, int option)
        {
            int h2 = h / 2;
            int w2 = w / 2;

            Image<Gray, byte> borderedImage = new Image<Gray, byte>(binaryImage.Width + 2 * w2, binaryImage.Height + 2 * h2);
            CvInvoke.CopyMakeBorder(binaryImage, borderedImage, h2, h2, w2, w2, Emgu.CV.CvEnum.BorderType.Replicate);

            Image<Gray, byte> result = new Image<Gray, byte>(binaryImage.Size);

            for (int y = 0; y < binaryImage.Height; y++)
            {
                for (int x = 0; x < binaryImage.Width; x++)
                {
                    bool conditionMet = false;
                    for (int i = -h2; i <= h2; i++)
                    {
                        for (int j = -w2; j <= w2; j++)
                        {
                            byte neighbor = borderedImage.Data[y + h2 + i, x + w2 + j, 0];
                            if (option == 1 && neighbor == 0) conditionMet = true;
                            if (option == 0 && neighbor == 255) conditionMet = true;
                        }
                    }
                    if (option == 1) result.Data[y, x, 0] = conditionMet ? (byte)0 : (byte)255;
                    else result.Data[y, x, 0] = conditionMet ? (byte)255 : (byte)0;
                }
            }
            return result;
        }

        public static Image<Gray, byte> MorphDilate(Image<Gray, byte> input, int h, int w, int T, int option)
        {
            var binary = Binarize(input, T);
            return DilateLogic(binary, h, w, option);
        }

        public static Image<Gray, byte> MorphErode(Image<Gray, byte> input, int h, int w, int T, int option)
        {
            var binary = Binarize(input, T);
            return ErodeLogic(binary, h, w, option);
        }

        public static Image<Gray, byte> MorphOpening(Image<Gray, byte> input, int h, int w, int T, int option)
        {
            var binary = Binarize(input, T);
            var eroded = ErodeLogic(binary, h, w, option);
            return DilateLogic(eroded, h, w, option);
        }

        public static Image<Gray, byte> MorphClosing(Image<Gray, byte> input, int h, int w, int T, int option)
        {
            var binary = Binarize(input, T);
            var dilated = DilateLogic(binary, h, w, option);
            return ErodeLogic(dilated, h, w, option);
        }
        public static Image<Bgr, byte> ConnectedComponents(Image<Gray, byte> inputImage, int threshold)
        {
            Image<Gray, byte> binaryImage = Binarize(inputImage, threshold);

            int width = binaryImage.Width;
            int height = binaryImage.Height;

            Image<Bgr, byte> resultImage = new Image<Bgr, byte>(width, height);
            int[,] visited = new int[height, width];
            Random rand = new Random();

            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (binaryImage.Data[y, x, 0] == 255 && visited[y, x] == 0)
                    {
                        byte b = (byte)rand.Next(0, 256);
                        byte g = (byte)rand.Next(0, 256);
                        byte r = (byte)rand.Next(0, 256);

                        Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
                        queue.Enqueue(new Tuple<int, int>(x, y));
                        visited[y, x] = 1;

                        resultImage.Data[y, x, 0] = b;
                        resultImage.Data[y, x, 1] = g;
                        resultImage.Data[y, x, 2] = r;

                        while (queue.Count > 0)
                        {
                            Tuple<int, int> current = queue.Dequeue();
                            int cx = current.Item1;
                            int cy = current.Item2;

                            for (int i = 0; i < 8; i++)
                            {
                                int nx = cx + dx[i];
                                int ny = cy + dy[i];

                                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                                {
                                    if (binaryImage.Data[ny, nx, 0] == 255 && visited[ny, nx] == 0)
                                    {
                                        visited[ny, nx] = 1;
                                        resultImage.Data[ny, nx, 0] = b;
                                        resultImage.Data[ny, nx, 1] = g;
                                        resultImage.Data[ny, nx, 2] = r;
                                        queue.Enqueue(new Tuple<int, int>(nx, ny));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return resultImage;
        }
    }
}