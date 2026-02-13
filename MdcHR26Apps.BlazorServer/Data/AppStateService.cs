namespace MdcHR26Apps.BlazorServer.Data;

public class AppStateService(IConfiguration configuration)
{
    /// Primary Constructor 사용 (C# 13)
    private readonly IConfiguration _configuration = configuration;

    public bool evaluationOpen { get; set; } = false;
    public bool editOpen { get; set; } = false;

    // 상태값이 변화됨을 알림
    public event Action? OnChange;
    private void AppStateChanged() => OnChange?.Invoke();

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

    #region + 평가 상태관련(HRSetting)
    public bool GetEvaluationOpen()
    {
        return evaluationOpen;
    }

    public bool GetEditOpen()
    {
        return editOpen;
    }


    public void SetEvaluationOpen(bool isEvaluationOpen)
    {
        evaluationOpen = isEvaluationOpen;
        AppStateChanged();
    }

    public void SetEditOpen(bool isEditOpen)
    {
        editOpen = isEditOpen;
        AppStateChanged();
    }
    #endregion
}
