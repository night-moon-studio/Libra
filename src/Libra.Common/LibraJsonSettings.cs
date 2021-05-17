using System.Text.Json;


public static class LibraJsonSettings
{
    public static readonly JsonSerializerOptions Options;

    static LibraJsonSettings()
    {
        Options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
#if NET5_0_OR_GREATER
        Options.IncludeFields = true;
#endif

    }
}
