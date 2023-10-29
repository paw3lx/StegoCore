namespace StegoCore.Model;

public class Settings
{
    /// <summary>
    /// Parameter D for Zhao&Koch Algorithm
    /// </summary>
    public int D { get; set; }

    /// <summary>
    /// Key, that will be use for picking place in image while embeding or decoding
    /// </summary>
    public string Key { get; set; }
}
