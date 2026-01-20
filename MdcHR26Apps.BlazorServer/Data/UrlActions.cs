using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Data;

public class UrlActions
{
    private readonly NavigationManager _navigationManager;

    public UrlActions(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    public void MoveMainPage() => _navigationManager.NavigateTo("/");
    public void MoveLoginPage() => _navigationManager.NavigateTo("/auth/login");
    public void MoveLogoutPage() => _navigationManager.NavigateTo("/auth/logout");
    public void MoveAdminPage() => _navigationManager.NavigateTo("/admin");
}
