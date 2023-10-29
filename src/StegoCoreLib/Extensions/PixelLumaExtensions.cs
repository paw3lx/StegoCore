using StegoCore.Model;

namespace StegoCore.Extensions;

internal static class PixelLumaExtensions
{
    internal static float[][] GetY(this PixelLuma[][] luma)
    {
        float[][] y = new float[luma.Length][];
        int i, j;
        for (i = 0; i < luma.Length; i++)
        {
            y[i] = new float[luma[i].Length];
            for (j = 0; j < y[i].Length; j++)
            {
                y[i][j] = luma[i][j].Y;
            }
        }
        return y;
    }

    internal static PixelLuma[][] SetY(this PixelLuma[][] luma, float[][] yMatrix)
    {
        int i, j;
        for (i = 0; i < luma.Length; i++)
        {
            for (j = 0; j < luma[i].Length; j++)
            {
                var oldLuma = luma[i][j];
                luma[i][j] = new PixelLuma(yMatrix[i][j], oldLuma.Cb, oldLuma.Cr);
            }
        }
        return luma;
    }
}
