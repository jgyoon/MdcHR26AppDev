using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Models;
using MdcHR26Apps.BlazorServer.Components.Pages.Components.SubAgreement.CommonView;
using MdcHR26Apps.Models.EvaluationAgreement;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.EvaluationSubAgreement;
using MdcHR26Apps.Models.Views.v_ReportTaskListDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;
using System.Web;
using static MdcHR26Apps.BlazorServer.Components.Pages.Components.SubAgreement.CommonView.AgreeItemLists;

namespace MdcHR26Apps.BlazorServer.Components.Pages.SubAgreement.User
{
    public partial class Index : ComponentBase
    {
        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 평가순서관리
        [Inject]
        public IProcessRepository processDbRepository { get; set; } = null!;
        public ProcessDb processDb { get; set; } = new ProcessDb();

        // 직무합의관리
        [Inject]
        public IAgreementRepository agreementDbRepository { get; set; } = null!;
        public List<AgreementDb> agreementDbList { get; set; } = new List<AgreementDb>();

        // 세부직무합의관리
        [Inject]
        public ISubAgreementRepository subAgreementDbRepository { get; set; } = null!;
        public List<SubAgreementDb> model { get; set; } = new List<SubAgreementDb>();

        public List<AgreeItemModel> itemlist { get; set; } = new List<AgreeItemModel>();

        // 평가리스트 작성
        [Inject]
        public IReportRepository reportDbRepository { get; set; } = null!;
        public ReportDb report { get; set; } = new ReportDb();

        [Inject]
        public Iv_ReportTaskListRepository v_ReportTaskListDBRepository { get; set; } = null!;
        public List<v_ReportTaskListDB> v_ReportTaskLists { get; set; } = new List<v_ReportTaskListDB>();

        // 기타
        public bool IsRequest { get; set; } = false;
        public bool IsAgreement { get; set; } = false;
        public bool IsSubRequest { get; set; } = false;
        public bool IsSubAgreement { get; set; } = false;
        public string Sub_Agreement_Comment { get; set; } = string.Empty;
        public bool Is_User_Submission { get; set; } = false;

        public bool IsCreateReportStatus { get; set; } = false;

        // 펼쳐보기
        public bool Collapsed { get; set; } = true;

        // 직무펼쳐보기
        public bool ItemsCollapsed { get; set; } = true;

        // 세부합의내역 펼쳐보기 - 기본값은 비활성화
        public bool SubAgreementCollapsed { get; set; } = false;

        // 세부업무내역 펼쳐보기 - 기본값은 비활성화
        public bool TaskListCollapsed { get; set; } = false;

        // 체크문자
        public string CheckedCode { get; set; } = "\u2713" + " ";


        // 부서장 & 임원 설정여부
        public bool IsTeamLeaderAndDiretorSettings = false;


        // 직무합의 개수
        public int agreementItemCount { get; set; } = 0;

        // 세부직무완료여부
        public bool IsSubAgreementStatus { get; set; } = false;

        // 최대작성 가능 세부직무 개수(최대 5개)
        public int subagreementItemMaxCount { get; set; } = 5;

        // 확인 후 삭제
        public string Agreement_Comment { get; set; } = string.Empty;
        public int sumperoportion { get; set; } = 0;
        public bool IS_SumPeroportionStatus { get; set; } = false;
        public double sumsubperoportion { get; set; } = 0;
        public bool IS_SumSubPeroportionStatus { get; set; } = false;

        // 기타
        public string resultText { get; set; } = String.Empty;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await CheckLogined();
                await SetData();
                StateHasChanged();
            }
            else
            {
                await SetData();
                StateHasChanged();
            }
        }

        #region + [0].[1] CheckLogined : IsloginCheck()
        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginCheck())
            {
                StateHasChanged();
                urlActions.MoveMainPage();
            }
        }
        #endregion

        private async Task SetData()
        {
            long sessionUid = loginStatusService.LoginStatus.LoginUid;
            if (sessionUid > 0)
            {
                processDb = await processDbRepository.GetByUidAsync(sessionUid);
                // 직무평가합의
                IsRequest = processDb.Is_Request;
                // 직무평가합의 여부
                IsAgreement = processDb.Is_Agreement;
                // 세부직무평가 합의
                IsSubRequest = processDb.Is_SubRequest;
                // 직무평가합의 여부
                IsSubAgreement = processDb.Is_SubAgreement;
                // 세부직무평가 합의 Comment
                Sub_Agreement_Comment = processDb.SubAgreement_Comment != null ? processDb.SubAgreement_Comment : string.Empty;
                // 평가제출 여부
                Is_User_Submission = processDb.Is_User_Submission;
                // 부서장 설정여부
                IsTeamLeaderAndDiretorSettings = GetTeamLeaderAndDiretorSettings(processDb);

                agreementDbList = await agreementDbRepository.GetByUidAllAsync(sessionUid);

                agreementItemCount = agreementDbList.Count;

                model = await subAgreementDbRepository.GetByUidAllAsync(sessionUid);

                itemlist = GetItems(agreementDbList, model);

                // 세부직무완료여부
                IsSubAgreementStatus = GetSubAgreementStatus(itemlist);

                // 세부업무리스트
                await GetReportTaskList();

                //subagreementItemMaxCount = agreementItemCount * 5;
            }
        }

        /// <summary>
        /// 부서장 & 임원 설정여부를 확인하는 메서드
        /// </summary>
        /// <param name="processDb"></param>
        /// <returns>설정(O) true, 설정(X) false</returns>
        private bool GetTeamLeaderAndDiretorSettings(ProcessDb processDb)
        {
            if (processDb.TeamLeaderId > 0 && processDb.DirectorId > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region + 직무별 세부직무 총합을 구하기(1)
        /// <summary>
        /// 직무별 세부직무 총합을 구하는 메서드
        /// </summary>
        /// <param name="agreementDbList">agreementDbList</param>
        /// <param name="model">subAgreementDbList</param>
        /// <returns>직무별 세부직무 총합</returns>
        private List<AgreeItemModel> GetItems(List<AgreementDb> agreementDbList, List<SubAgreementDb> model)
        {
            List<AgreeItemModel> resultlist = new List<AgreeItemModel>();

            if (agreementDbList.Count != 0)
            {
                foreach (var item in agreementDbList)
                {
                    resultlist.Add(new AgreeItemModel
                    {
                        ItemName = item.Report_Item_Name_1 + "-" + item.Report_Item_Name_2,
                        ItemPeroportion = item.Report_Item_Proportion,
                        ItemSubPeroportion = GetSubPeroportion(model, item.Report_Item_Name_1, item.Report_Item_Name_2),
                        ItemCompleteStatus = GetItemCompleteStatus(GetSubPeroportion(model, item.Report_Item_Name_1, item.Report_Item_Name_2))
                    });
                }
            }

            return resultlist;
        }
        #endregion

        #region + 직무별 세부직무 총합을 구하기(2)
        /// <summary>
        /// 직무별 세부직무 총합을 구하기 메서드(2)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ItemName_1"></param>
        /// <param name="ItemName_2"></param>
        /// <returns></returns>
        private int GetSubPeroportion(List<SubAgreementDb> model, string ItemName_1, string ItemName_2)
        {
            int resultVaule = 0;

            if (model.Count != 0)
            {
                List<SubAgreementDb> lists = (List<SubAgreementDb>)model.Where(e => e.Report_Item_Name_1.Equals(ItemName_1) && e.Report_Item_Name_2.Equals(ItemName_2)).ToList();
                if (lists.Count != 0)
                {
                    foreach (var item in lists)
                    {
                        resultVaule = resultVaule + item.Report_SubItem_Proportion;
                    }
                }
            }
            return resultVaule;
        }
        #endregion

        #region + 직무별 세부직무 총합을 확인하고 완료여부 체크
        /// <summary>
        /// 직무별 세부직무 총합을 확인하고 완료여부 체크하는 메서드
        /// </summary>
        /// <param name="maxValue">직무별 세부직무 총합</param>
        /// <returns>세부직무 완료여부</returns>
        private bool GetItemCompleteStatus(int maxValue)
        {
            return maxValue == 100 ? true : false;
        }
        #endregion

        #region + 모든 직무별 세부직무 완료여부 체크하기
        /// <summary>
        /// 모든 직무별 세부직무 완료여부 체크하는 메서드
        /// </summary>
        /// <param name="items">List<Items> items</param>
        /// <returns>모든 직무별 세부직무 완료여부 체크여부</returns>
        private bool GetSubAgreementStatus(List<AgreeItemModel> items)
        {
            bool result = false;
            if (items != null && items.Count != 0)
            {
                List<AgreeItemModel> lists = new List<AgreeItemModel>();
                lists = items.Where(e => e.ItemCompleteStatus.Equals(false)).ToList();

                result = lists.Count == 0 ? true : false;
            }

            return result;
        }
        #endregion

        #region + [9].[1] MoveUserSubAgreementMainPage : 세부직무작성 메인페이지 이동
        public void MoveUserSubAgreementMainPage()
        {
            urlActions.MoveUserSubAgreementMainPage();
        }
        #endregion

        #region + [9].[2] MoveUserSubAgreementCreatePage : 세부직무작성 생성페이지 이동
        public void MoveUserSubAgreementCreatePage()
        {
            urlActions.MoveUserSubAgreementCreatePage();
        }
        #endregion

        #region + [4].[1] MoveReportMainPage : 본인평가페이지 이동
        public void MoveReportMainPage()
        {
            urlActions.MoveReportMainPage();
        }
        #endregion

        #region + 직무합의요청
        protected async Task SetSubRequst()
        {
            processDb.Is_SubRequest = true;

            int status = await processDbRepository.UpdateAsync(processDb);

            if (status > 0)
            {
                IsSubRequest = processDb.Is_SubRequest;
                StateHasChanged();
                //urlActions.MoveUserSubAgreementMainPageReload();
            }
            else
            {
                resultText = "합의요청 실패!-1";
            }
        }
        #endregion

        #region + 직무합의요청취소
        protected async Task SetSubRequstCancel()
        {
            processDb.Is_SubRequest = false;

            int status = await processDbRepository.UpdateAsync(processDb);

            if (status > 0)
            {
                IsSubRequest = processDb.Is_SubRequest;
                StateHasChanged();
                //urlActions.MoveUserSubAgreementMainPageReload();
            }
            else
            {
                resultText = "합의요청 실패!-2";
            }
        }
        #endregion

        #region + 합의여부확인
        private string result()
        {
            return IsSubAgreement ? "합의(O)" : "합의(X)";
        }
        #endregion

        #region + 문자열 변경
        /// <summary>
        /// 문자열을 웹형식에 맞추어서 변환하는 메서드
        /// </summary>
        /// <param name="contenct">string</param>
        /// <returns>웹형식의 문자열</returns>
        private string replaceString(string contenct)
        {
            return Regex.Replace(HttpUtility.HtmlEncode(contenct), "\r?\n|\r", "<br />");
        }
        #endregion

        #region + 평가표 생성 : CreateReport
        private async Task CreateReport()
        {
            await Task.Delay(1);

            if (model != null && model.Count > 0)
            {
                foreach (var item in model)
                {
                    ReportDb reports = new ReportDb();

                    #region + [0] 평가기본정보
                    // [02] 사용자 계정
                    reports.Uid = item.Uid;
                    // [04] Report_Item_Number
                    reports.Report_Item_Number = item.Report_Item_Number;
                    // [05] Report_Item_Name_1(지표분류명)
                    reports.Report_Item_Name_1 = item.Report_Item_Name_1;
                    // [06] Report_Item_Name_2(직무분류명)
                    reports.Report_Item_Name_2 = item.Report_Item_Name_2;
                    // [07] Report_Item_Proportion(직무 %)
                    reports.Report_Item_Proportion = item.Report_Item_Proportion;
                    // [08] Report_SubItem_Name(세부직무명)
                    reports.Report_SubItem_Name = item.Report_SubItem_Name;
                    // [09] Report_Item_Proportion(세부직무 %)
                    reports.Report_SubItem_Proportion = item.Report_SubItem_Proportion;
                    #endregion

                    #region + 평가대상자 평가
                    // [10] User_Evaluation_1(일정준수)
                    reports.User_Evaluation_1 = 0;
                    // [11] User_Evaluation_2(업무수행도)
                    reports.User_Evaluation_2 = 0;
                    // [12] User_Evaluation_3(결과물)
                    reports.User_Evaluation_3 = 0;
                    // [13] User_Evaluation_4(comment)
                    reports.User_Evaluation_4 = String.Empty;
                    #endregion

                    #region + [2] 부서장(팀장) 평가
                    // [14] TeamLeader_Evaluation_1(일정준수)
                    reports.TeamLeader_Evaluation_1 = 0;
                    // [15] TeamLeader_Evaluation_2(업무수행도)
                    reports.TeamLeader_Evaluation_2 = 0;
                    // [16] TeamLeader_Evaluation_3(결과물)
                    reports.TeamLeader_Evaluation_3 = 0;
                    // [17] TeamLeader_Evaluation_4(comment)
                    reports.TeamLeader_Evaluation_4 = String.Empty;
                    #endregion

                    #region  + [3] 임원 평가
                    // [18] Director_Evaluation_1(일정준수)
                    reports.Director_Evaluation_1 = 0;
                    // [19] Director_Evaluation_2(업무수행도)
                    reports.Director_Evaluation_2 = 0;
                    // [20] Director_Evaluation_3(결과물)
                    reports.Director_Evaluation_3 = 0;
                    // [21] Director_Evaluation_4(comment)
                    reports.Director_Evaluation_4 = String.Empty;
                    #endregion

                    // [23] Total_Score(종합점수)
                    reports.Total_Score = 0;
                    var rid = await reportDbRepository.AddAsync(reports);
                    reports.Rid = rid;
                }
                IsCreateReportStatus = true;
            }

            //await GetReportTaskList();
        }
        #endregion

        #region + 세부업무리스트 출력
        private async Task GetReportTaskList()
        {
            await Task.Delay(1);
            long sessionUid = loginStatusService.LoginStatus.LoginUid;
            if (sessionUid > 0 && IsSubRequest && IsSubAgreement)
            {
                v_ReportTaskLists =
                    await v_ReportTaskListDBRepository.GetByUidAllAsync(sessionUid);
            }
        }
        #endregion

        /// <summary>
        /// Toggle 이벤트(펼쳐보기)
        /// </summary>
        private void Toggle()
        {
            Collapsed = !Collapsed;
        }

        private void ReportToggle()
        {
            ItemsCollapsed = !ItemsCollapsed;
        }

        /// <summary>
        /// SubAgreementToggle 이벤트
        /// </summary>
        private void SubAgreementToggle()
        {
            SubAgreementCollapsed = !SubAgreementCollapsed;
        }

        /// <summary>
        /// TaskListToggle 이벤트
        /// </summary>
        private void TaskListToggle()
        {
            TaskListCollapsed = !TaskListCollapsed;
        }

        /// <summary>
        /// 세부직무 상세 페이지로 이동
        /// </summary>
        private void MoveDetailsPage(long sid)
        {
            urlActions.MoveUserSubAgreementDetailsPage(sid);
        }
    }
}
