namespace MdcHR26Apps.BlazorServer.Data;

public class LoginStatusService
{
    public LoginStatus LoginStatus { get; set; } = new();

    public event Action? OnChange;

    public LoginStatus SetLoginStatus(
        bool isLogin,
        Int64 uid,
        string userid,
        string username,
        bool isadmin,
        bool isteamleader,
        bool isdirector,
        bool isdeptobjwriter,
        string department)
    {
        LoginStatus.IsLogin = isLogin;
        LoginStatus.LoginUid = uid;
        LoginStatus.LoginUserId = userid;
        LoginStatus.LoginUserName = username;
        LoginStatus.LoginIsAdministrator = isadmin;
        LoginStatus.LoginIsTeamLeader = isteamleader;
        LoginStatus.LoginIsDirector = isdirector;
        LoginStatus.LoginIsDeptObjectiveWriter = isdeptobjwriter;
        LoginStatus.LoginUserEDepartment = department;

        OnChange?.Invoke();
        return LoginStatus;
    }

    public bool IsloginCheck() => LoginStatus.IsLogin;

    public bool IsloginAndIsAdminCheck() =>
        LoginStatus.IsLogin && LoginStatus.LoginIsAdministrator;

    public bool IsloginAndIsTeamLeaderCheck() =>
        LoginStatus.IsLogin && LoginStatus.LoginIsTeamLeader;

    public bool IsloginAndIsDirectorCheck() =>
        LoginStatus.IsLogin && LoginStatus.LoginIsDirector;
}
