using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Views.v_MemberListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Table
{
    public partial class UserListTable
    {
        [Parameter]
        public List<v_MemberListDB> Users { get; set; } = new List<v_MemberListDB>();
        // 페이지 이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        public List<v_MemberListDB> BackupUsers { get; set; } = new List<v_MemberListDB>();

        public int sortNo = 1;

        // 테이블 CSS Style
        public string table_style_1 = "white-space: nowrap; overflow: hidden; text-overflow: ellipsis; text-align: center; vertical-align: middle;";
        private int sortNoAdd(int sort)
        {
            sortNo = sort + 1;
            return sort;
        }

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

        private string isadmin(bool isadmin)
        {
            return isadmin ? "관리자" : "일반사용자";
        }

        private string isuse(bool isuse)
        {
            return isuse ? "사용중" : "미사용";
        }

        private void UserDetailsAction(Int64 Uid)
        {
            urlActions.MoveUserDetailsPage(Uid);
        }
    }
}
