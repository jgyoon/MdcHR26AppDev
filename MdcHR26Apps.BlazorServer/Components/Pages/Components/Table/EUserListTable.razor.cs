using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Views.v_EvaluationUsersList;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Table;

public partial class EUserListTable
{
    [Parameter]
    public List<v_EvaluationUsersList> Users { get; set; } = new();

    [Inject]
    public UrlActions urlActions { get; set; } = null!;

    public int sortNo = 1;

    private int sortNoAdd2(int sort)
    {
        if (Users.Count == sortNo)
        {
            sortNo = 1;
            return sort;
        }
        sortNo = sort + 1;
        return sort;
    }

    private string isuse(bool isuse)
    {
        return isuse ? "평가대상자" : "미평가대상자";
    }

    private void DetailsAction(long uid)
    {
        urlActions.MoveEUserDetailsPage(uid);
    }

    private void EditAction(long uid)
    {
        urlActions.MoveEUsersEditPage(uid);
    }
}
