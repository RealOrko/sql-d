namespace SqlD.Network.Diagnostics;

public class FastPolly
{
    public static FastPollyPolicy Handle<T>()
    {
        return new FastPollyPolicy(typeof(T));
    }
}