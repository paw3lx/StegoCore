using System.Text;
using BenchmarkDotNet.Attributes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StegoCore;
using StegoCore.Algorithms;
using StegoCore.Core;

namespace StegoBenchmark;

[MemoryDiagnoser]
public class LsbPerformance
{
    private readonly Lsb _stegoLsb;
    private readonly SecretData _secretData;
    private readonly Image<Rgba32> _image;
    private readonly byte[] _data = Encoding.UTF8.GetBytes(SecretData);

    private const string SecretData = "Hello World! Testing longer secret data. Blah blah blah.";

    public LsbPerformance()
    {
        _secretData = new SecretData(_data);
        _image = Image.Load<Rgba32>("lena30.jpg");
        _stegoLsb = new Lsb();
    }

    [Benchmark]
    public void Lsb()
    {
        var image = _stegoLsb.Embed(_image, _secretData);
        var secret = _stegoLsb.Decode(image);
    } 

}