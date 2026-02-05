namespace MdcHR26Apps.BlazorServer.Data;

public class AppStateService(IConfiguration configuration)
{
    /// Primary Constructor 사용 (C# 13)
    private readonly IConfiguration _configuration = configuration;

    // 상태값이 변화됨을 알림
    public event Action? OnChange;
    private void AppStateChanged() => OnChange?.Invoke();

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

    #region + 개발환경관련
    public bool IsProduction { get; private set; } = false;

    public void SetIsProduction(int no)
    {
        IsProduction = no == 1 ? true : false;
        AppStateChanged();
    }

    public bool GetIsProduction()
    {
        return IsProduction;
    }
    #endregion

    #region + 모달 관련
    public bool IsDeleteModalShow { get; set; } = false;

    public void SetDeleteAction()
    {
        IsDeleteModalShow = true;
        AppStateChanged();
    }

    public void AppStateInital()
    {
        IsDeleteModalShow = false;
        AppStateChanged();
    }
    #endregion
}
