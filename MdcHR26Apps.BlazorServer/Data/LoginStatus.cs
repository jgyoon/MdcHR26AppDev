namespace MdcHR26Apps.BlazorServer.Data;

public class LoginStatus
{
    public bool IsLogin { get; set; }
    public Int64 LoginUid { get; set; }
    public string? LoginUserId { get; set; }
    public string? LoginUserName { get; set; }
    public bool LoginIsAdministrator { get; set; }
    public bool LoginIsTeamLeader { get; set; }
    public bool LoginIsDirector { get; set; }
    public bool LoginIsDeptObjectiveWriter { get; set; }
    public string? LoginUserEDepartment { get; set; }
}
