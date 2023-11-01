namespace StegoCore.Model;

internal class PixelLuma
{
    public PixelLuma(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
        Calculate();
    }

    public PixelLuma(float y, float cb, float cr)
    {
        Y = y;
        Cb = cb;
        Cr = cr;
        Decalculate();
    }

    public float Y { get; private set; }

    public float Cb { get; private set; }

    public float Cr { get; private set; }

    public byte R { get; private set; }

    public byte G { get; private set; }

    public byte B { get; private set; }

    private void Calculate()
    {
        Y = (float)(0.299 * R + 0.587 * G + 0.114 * B);
        Cb = (float)(-0.168736 * R - 0.331264 * G + 0.5 * B + 128);
        Cr = (float)(0.5 * R - 0.418688 * G - 0.081312 * B + 128);
    }

    private void Decalculate()
    {
        R = (byte)(Y + 1.402 * (Cr - 128));
        G = (byte)(Y - 0.34414 * (Cb - 128) - 0.71414 * (Cr - 128));
        B = (byte)(Y + 1.772 * (Cb - 128));
    }
}
