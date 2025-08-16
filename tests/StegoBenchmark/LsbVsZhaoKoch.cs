using System.Text;
using BenchmarkDotNet.Attributes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StegoCore;
using StegoCore.Algorithms;
using StegoCore.Core;

namespace StegoBenchmark;

[MemoryDiagnoser]
public class LsbVsZhaoKoch
{
    private readonly Lsb _stegoLsb;
    private readonly ZhaoKoch _stegoZhaoKoch;
    private readonly SecretData _secretData;
    private readonly Image<Rgba32> _image;

    public LsbVsZhaoKoch()
    {
        _secretData = new SecretData(Encoding.UTF8.GetBytes("Hello World!"));
        _image = Image.Load<Rgba32>("lena30.jpg");
        _stegoLsb = new Lsb();
        _stegoZhaoKoch = new ZhaoKoch();
    }

    [Benchmark]
    public Image<Rgba32> Lsb() => _stegoLsb.Embed(_image, _secretData);

    [Benchmark]
    public Image<Rgba32> ZhaoKoch() => _stegoZhaoKoch.Embed(_image, _secretData);
}