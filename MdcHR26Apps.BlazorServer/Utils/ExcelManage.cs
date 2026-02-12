using ClosedXML.Excel;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using MdcHR26Apps.Models.Views.v_ReportTaskListDB;

namespace MdcHR26Apps.BlazorServer.Utils;

public class ExcelManage(IWebHostEnvironment environment)
{
    private readonly IWebHostEnvironment _environment = environment;
    private readonly string _folderPath = Path.Combine(environment.WebRootPath, "files");
    private readonly ScoreUtils _scoreUtils = new();

    #region + [1] CreateAdminExcelFile : 관리자용 평가 리스트 엑셀 생성
    /// <summary>
    /// 관리자용 전체 평가 현황 엑셀 파일 생성
    /// </summary>
    public bool CreateAdminExcelFile(List<v_ProcessTRListDB> data, string fileFolderPath, string fileName)
    {
        string exportFile = Path.Combine(_folderPath, fileFolderPath, fileName);

        if (File.Exists(exportFile))
        {
            File.Delete(exportFile);
        }

        if (!File.Exists(exportFile))
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("평가현황");

                // 헤더 설정
                worksheet.Cells("A1").Value = "No";
                worksheet.Cells("B1").Value = "이름";
                worksheet.Cells("C1").Value = "평가제출";
                worksheet.Cells("D1").Value = "평가자점수";
                worksheet.Cells("E1").Value = "부서장(팀장)이름";
                worksheet.Cells("F1").Value = "평가완료(팀장)";
                worksheet.Cells("G1").Value = "부서장(팀장)점수";
                worksheet.Cells("H1").Value = "2026년 종합평가";
                worksheet.Cells("I1").Value = "미래성장방안";
                worksheet.Cells("J1").Value = "임원이름";
                worksheet.Cells("K1").Value = "평가완료(임원)";
                worksheet.Cells("L1").Value = "최종점수(임원)";
                worksheet.Cells("M1").Value = "종합평가";
                worksheet.Cells("N1").Value = "종합등급";

                int recordIndex = 2;

                foreach (var item in data)
                {
                    worksheet.Cell(recordIndex, 1).Value = recordIndex - 1;
                    worksheet.Cell(recordIndex, 2).Value = item.UserName;
                    worksheet.Cell(recordIndex, 3).Value = IsSubmission(item.Is_User_Submission);
                    worksheet.Cell(recordIndex, 4).Value = IsScore(item.User_Evaluation_1, item.User_Evaluation_2, item.User_Evaluation_3);
                    worksheet.Cell(recordIndex, 5).Value = item.TeamLeader_Name ?? "-";
                    worksheet.Cell(recordIndex, 6).Value = IsSubmission(item.Is_Teamleader_Submission);
                    worksheet.Cell(recordIndex, 7).Value = IsScore(item.TeamLeader_Evaluation_1, item.TeamLeader_Evaluation_2, item.TeamLeader_Evaluation_3);
                    worksheet.Cell(recordIndex, 8).Value = item.TeamLeader_Comment ?? string.Empty;
                    worksheet.Cell(recordIndex, 9).Value = item.Feedback_Comment ?? string.Empty;
                    worksheet.Cell(recordIndex, 10).Value = item.Director_Name ?? "-";
                    worksheet.Cell(recordIndex, 11).Value = IsSubmission(item.Is_Director_Submission);
                    worksheet.Cell(recordIndex, 12).Value = item.Director_Score;
                    worksheet.Cell(recordIndex, 13).Value = item.Director_Comment ?? string.Empty;
                    worksheet.Cell(recordIndex, 14).Value = _scoreUtils.GetTotalScore(item.Total_Score);

                    recordIndex++;
                }

                workbook.SaveAs(exportFile);
            }
        }

        return File.Exists(exportFile);
    }
    #endregion

    #region + [1].[1] TeamLeader용 excel 생성
    public bool CreateTeamLeaderExcelFile(List<v_ProcessTRListDB> data, string filefolderPath, string fileName)
    {
        string exportFile = Path.Combine(_folderPath, filefolderPath, fileName);
        if (File.Exists(exportFile))
        {
            File.Delete(exportFile);
        }
        if (!File.Exists(exportFile))
        {
            using (var excelPackage = new XLWorkbook())
            {
                //create a new Worksheet
                var worksheet = excelPackage.Worksheets.Add("Lists");
                #region + TaskLists
                worksheet.Cells("A1").Value = "No";
                worksheet.Cells("B1").Value = "이름";
                worksheet.Cells("C1").Value = "평가제출";
                worksheet.Cells("D1").Value = "평가자점수";
                worksheet.Cells("E1").Value = "평가완료";
                worksheet.Cells("F1").Value = "부서장(팀장)점수";
                int recordIndex = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(recordIndex, 1).Value = recordIndex - 1;
                    worksheet.Cell(recordIndex, 2).Value = item.UserName;
                    worksheet.Cell(recordIndex, 3).Value = this.IsSubmission(item.Is_User_Submission);
                    worksheet.Cell(recordIndex, 4).Value = this.IsScore(item.User_Evaluation_1, item.User_Evaluation_2, item.User_Evaluation_3);
                    worksheet.Cell(recordIndex, 5).Value = this.IsSubmission(item.Is_Teamleader_Submission);
                    worksheet.Cell(recordIndex, 6).Value = item.TeamLeader_Score;
                    recordIndex++;
                }
                #endregion
                excelPackage.SaveAs(exportFile);
            }
        }
        return File.Exists(exportFile) ? true : false;
    }
    #endregion
    
    #region + [1].[2] Director용 excel 생성
    public bool CreateDirectorExcelFile(List<v_ProcessTRListDB> data, string filefolderPath, string fileName)
    {
        string exportFile = Path.Combine(_folderPath, filefolderPath, fileName);
        if (File.Exists(exportFile))
        {
            File.Delete(exportFile);
        }
        if (!File.Exists(exportFile))
        {
            using (var excelPackage = new XLWorkbook())
            {
                //create a new Worksheet
                var worksheet = excelPackage.Worksheets.Add("Lists");
                #region + TaskLists
                worksheet.Cells("A1").Value = "No";
                worksheet.Cells("B1").Value = "이름";
                worksheet.Cells("C1").Value = "평가제출";
                worksheet.Cells("D1").Value = "평가자점수";
                worksheet.Cells("E1").Value = "평가완료(팀장)";
                worksheet.Cells("F1").Value = "부서장(팀장)점수";
                worksheet.Cells("G1").Value = "평가완료(임원)";
                worksheet.Cells("H1").Value = "최종점수(임원)";
                int recordIndex = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(recordIndex, 1).Value = recordIndex - 1;
                    worksheet.Cell(recordIndex, 2).Value = item.UserName;
                    worksheet.Cell(recordIndex, 3).Value = this.IsSubmission(item.Is_User_Submission);
                    worksheet.Cell(recordIndex, 4).Value = this.IsScore(item.User_Evaluation_1, item.User_Evaluation_2, item.User_Evaluation_3);
                    worksheet.Cell(recordIndex, 5).Value = this.IsSubmission(item.Is_Teamleader_Submission);
                    worksheet.Cell(recordIndex, 6).Value = item.TeamLeader_Score;
                    worksheet.Cell(recordIndex, 7).Value = this.IsSubmission(item.Is_Director_Submission);
                    worksheet.Cell(recordIndex, 8).Value = item.Director_Score;
                    recordIndex++;
                }
                #endregion
                excelPackage.SaveAs(exportFile);
            }
        }
        return File.Exists(exportFile) ? true : false;
    }
    #endregion    

    #region + [2] CreateExcelFile : 세부 업무 리스트 엑셀 생성
    /// <summary>
    /// 세부 업무 리스트 엑셀 파일 생성 (2026년 구조)
    /// </summary>
    public bool CreateExcelFile(List<v_ReportTaskListDB> data, string fileFolderPath, string fileName)
    {
        string exportFile = Path.Combine(_folderPath, fileFolderPath, fileName);

        if (File.Exists(exportFile))
        {
            File.Delete(exportFile);
        }

        if (!File.Exists(exportFile))
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("세부업무");

                // 헤더 설정 (2026년 구조)
                worksheet.Cells("A1").Value = "No";
                worksheet.Cells("B1").Value = "평가자ID";
                worksheet.Cells("C1").Value = "평가자명";
                worksheet.Cells("D1").Value = "세부협약제목";
                worksheet.Cells("E1").Value = "비중(%)";
                worksheet.Cells("F1").Value = "업무제목";
                worksheet.Cells("G1").Value = "업무설명";
                worksheet.Cells("H1").Value = "시작일";
                worksheet.Cells("I1").Value = "종료일";
                worksheet.Cells("J1").Value = "달성률(%)";
                worksheet.Cells("K1").Value = "업무상태";

                int recordIndex = 2;

                foreach (var item in data)
                {
                    worksheet.Cell(recordIndex, 1).Value = recordIndex - 1;
                    worksheet.Cell(recordIndex, 2).Value = item.UserId;
                    worksheet.Cell(recordIndex, 3).Value = item.UserName;
                    worksheet.Cell(recordIndex, 4).Value = item.Report_SubItem_Name;
                    worksheet.Cell(recordIndex, 5).Value = item.Report_SubItem_Proportion;
                    worksheet.Cell(recordIndex, 6).Value = item.TaskName;
                    worksheet.Cell(recordIndex, 7).Value = item.TaskObjective;
                    worksheet.Cell(recordIndex, 8).Value = item.TargetDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(recordIndex, 9).Value = item.ResultDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(recordIndex, 10).Value = item.ResultProportion;
                    worksheet.Cell(recordIndex, 11).Value = TaskStatusToName(item.TaskStatus);

                    recordIndex++;
                }

                workbook.SaveAs(exportFile);
            }
        }

        return File.Exists(exportFile);
    }
    #endregion

    #region + [3] DeleteExcelFile : 엑셀 파일 삭제
    /// <summary>
    /// 엑셀 파일 삭제
    /// </summary>
    public void DeleteExcelFile(string fileFolderPath, string fileName)
    {
        string deleteFile = Path.Combine(_folderPath, fileFolderPath, fileName);
        if (File.Exists(deleteFile))
        {
            File.Delete(deleteFile);
        }
    }
    #endregion

    #region + 유틸리티 메서드
    /// <summary>
    /// 업무 상태 숫자 → 한글 변환 (2026년)
    /// </summary>
    private string TaskStatusToName(int status)
    {
        return status switch
        {
            0 => "대기",
            1 => "진행중",
            2 => "완료",
            3 => "보류",
            _ => "-"
        };
    }

    /// <summary>
    /// 점수 계산 (3개 점수 합산 → 100점 만점)
    /// </summary>
    private double IsScore(double? score1, double? score2, double? score3)
    {
        double score = (score1 ?? 0) + (score2 ?? 0) + (score3 ?? 0);
        return _scoreUtils.TotalScroreTo100thpercentile(score);
    }

    /// <summary>
    /// 제출 여부 → Y/N 변환
    /// </summary>
    private string IsSubmission(bool status) => status ? "Y" : "N";
    #endregion
}
