namespace MdcHR26Apps.BlazorServer.Data;

public class AppStateService
{
    private readonly IConfiguration _configuration;

    public AppStateService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public event Action? OnChange;

    public bool GetIsOpen()
    {
        var isOpen = _configuration.GetValue<int>("AppSettings:IsOpen");
        return isOpen == 1;
    }

    public string TruncateText(string? text, int maxLength)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text.Length <= maxLength ? text : $"{text.Substring(0, maxLength)}...";
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
