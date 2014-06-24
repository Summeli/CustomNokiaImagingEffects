
using Nokia.Graphics.Imaging;
using System;

namespace NISDKExtendedEffects.ImageEffects
{

    /*Demonstrates a relly simple way to calculate gausian blur effect*/
    public class GausianBlurEffect : CustomEffectBase
    {

        // coefficients of 1D gaussian kernel with sigma = 20
        //http://www.embege.com/gauss/
        protected double[] gausianKernel = { 0.1900801279413907, 0.2048843573061478, 0.21007102950492296, 0.2048843573061478, 0.1900801279413907, };

        public GausianBlurEffect(IImageProvider source)
            : base(source)
        {

        }

        protected override void OnProcess(PixelRegion sourcePixelRegion, PixelRegion targetPixelRegion)
        {
            int height = Convert.ToInt32(sourcePixelRegion.ImageSize.Height);
            int width = Convert.ToInt32(sourcePixelRegion.ImageSize.Width);
            uint[] src = sourcePixelRegion.ImagePixels;
           
            //calculate the 1D symmetric conviolution twice for gausian blur
            uint[] res1 = Filter1DSymmetric(src, height, width);
            uint[] res2 = Filter1DSymmetric(res1, width, height);

            //copy the result into targetPixelRegion
            uint[] target = targetPixelRegion.ImagePixels;
            Buffer.BlockCopy(res2, 0, target, 0, target.Length * sizeof(uint));
            

        }

        public uint[] Filter1DSymmetric(uint[] source, int height, int width)
        {
            uint[] ret = new uint[height * width];


            for (int y = 0; y < height; y++)
            {
                int index = y;
                int ioffset = y * width;
                for (int x = 0; x < width; x++)
                {
                    double r = 0, g = 0, b = 0, a = 0;
                    int moffset = 2;
                    for (int col = -2; col <= 2; col++)
                    {
                        double f = gausianKernel[moffset + col];
                        int ix = x + col;
                        ix = clampEdges(width, ix);
                        uint rgb = source[ioffset + ix];
                        r += f * ((rgb >> 16) & 0xff);
                        g += f * ((rgb >> 8) & 0xff);
                        b += f * (rgb & 0xff);

                    }
                    //convoluted pixel values
                    uint ca = 0;
                    uint cr = clamp((int)(r + 0.5));
                    uint cg = clamp((int)(g + 0.5));
                    uint cb = clamp((int)(b + 0.5));
                    ret[index] = (ca << 24) | (cr << 16) | (cg << 8) | cb;
                    index += height;
                }
            }
            return ret;
        }

        protected static uint clamp(int c)
        {
            if (c < 0)
                return 0;
            if (c > 255)
                return 255;
            return (uint)c;
        }

        protected int clampEdges(int M, int x)
        {
            if (x < 0)
            {
                x = 0;
            }
            if (x >= M)
            {
                x = M - 1;
            }
            return x;
        }

    }    
}