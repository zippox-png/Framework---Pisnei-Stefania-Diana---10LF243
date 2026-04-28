using Emgu.CV;
using Emgu.CV.Structure;

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
    }
}