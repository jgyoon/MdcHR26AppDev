using MdcHR26Apps.BlazorServer.Models;
using MdcHR26Apps.Models.EvaluationReport;

namespace MdcHR26Apps.BlazorServer.Utils;

public class ScoreUtils
{
    #region + 종합점수백분위환산(100th percentile score)
    /// <summary>
    /// 종합점수 백분위 환산
    /// </summary>
    /// <remarks>메서드명 오타(Scror) 주의: 2025년 코드와 동일하게 유지</remarks>
    public double TotalScroreTo100thpercentile(double Score)
    {
        double resultScore = 0;

        if (Score != 0)
        {
            resultScore = ((Score) / 300) * 100;  // 2025년 기준: 300점 만점 → 100점 만점
            resultScore = Math.Round(resultScore, 2);

            if (resultScore >= 100)
            {
                resultScore = 100;
            }
        }

        return resultScore;
    }
    #endregion

    #region + 일반점수백분위환산(100th percentile score)
    public double ScroreTo100thpercentile(double Score)
    {
        double resultScore = 0;

        if (Score != 0)
        {
            resultScore = Score;
            resultScore = Math.Round(resultScore, 2);

            if (resultScore >= 100)
            {
                resultScore = 100;
            }
        }

        return resultScore;
    }
    #endregion

    #region + [1] 평가대상자 점수 계산

    #region + [1].[1] User_GetTotalScore_1
    /// <summary>
    /// 평가대상자의 종합점수1(일정준수)를 구하는 메서드
    /// </summary>
    /// <param name="lists">평가대상자의 평가리스트</param>
    /// <returns>평가대상자의 종합점수1(double)</returns>
    public double User_GetTotalScore_1(List<ReportDb> lists)
    {
        double totalScore = 0;

        if (lists != null && lists.Count != 0)
        {
            // 평가비중 적용
            foreach (var item in lists)
            {
                totalScore = totalScore +
                    (double)(
                    item.User_Evaluation_1
                    * (double)(item.Report_Item_Proportion / (double)100)
                    * (double)(item.Report_SubItem_Proportion / (double)100)
                    );
            }

            // 소숫점 2자리까지 합산하여 계산
            totalScore = Math.Round(totalScore, 2);

            // 결과값이 100점 이상이면 100으로 수정
            if (totalScore >= 100)
            {
                totalScore = 100;
            }

            return totalScore;
        }
        else
        {
            return totalScore;
        }
    }
    #endregion

    #region + [1].[2] User_GetTotalScore_2
    /// <summary>
    /// 평가대상자의 종합점수2(업무수행도)를 구하는 메서드
    /// </summary>
    /// <param name="lists">평가대상자의 평가리스트</param>
    /// <returns>평가대상자의 종합점수2(double)</returns>
    public double User_GetTotalScore_2(List<ReportDb> lists)
    {
        double totalScore = 0;

        if (lists != null && lists.Count != 0)
        {
            // 평가비중 적용
            foreach (var item in lists)
            {
                totalScore = totalScore +
                    (double)(
                    item.User_Evaluation_2
                    * (double)(item.Report_Item_Proportion / (double)100)
                    * (double)(item.Report_SubItem_Proportion / (double)100)
                    );
            }

            // 소숫점 2자리까지 합산하여 계산
            totalScore = Math.Round(totalScore, 2);

            // 결과값이 100점 이상이면 100으로 수정
            if (totalScore >= 100)
            {
                totalScore = 100;
            }

            return totalScore;
        }
        else
        {
            return totalScore;
        }
    }
    #endregion

    #region + [1].[3] User_GetTotalScore_3
    /// <summary>
    /// 평가대상자의 종합점수3(결과평가(정성))를 구하는 메서드
    /// </summary>
    /// <param name="lists">평가대상자의 평가리스트</param>
    /// <returns>평가대상자의 종합점수3(double)</returns>
    public double User_GetTotalScore_3(List<ReportDb> lists)
    {
        double totalScore = 0;

        if (lists != null && lists.Count != 0)
        {
            // 평가비중 적용
            foreach (var item in lists)
            {
                totalScore = totalScore +
                    (double)(
                    item.User_Evaluation_3
                    * (double)(item.Report_Item_Proportion / (double)100)
                    * (double)(item.Report_SubItem_Proportion / (double)100)
                    );
            }

            // 소숫점 2자리까지 합산하여 계산
            totalScore = Math.Round(totalScore, 2);

            // 결과값이 100점 이상이면 100으로 수정
            if (totalScore >= 100)
            {
                totalScore = 100;
            }

            return totalScore;
        }
        else
        {
            return totalScore;
        }
    }
    #endregion

    #endregion

    #region + GetTotalScoreRankList : 등급 목록 반환
    /// <summary>
    /// 전체 등급 목록 반환 (100점 만점 기준: S, A, B, C, D)
    /// </summary>
    public List<TotalScoreRankModel> GetTotalScoreRankList()
    {
        List<TotalScoreRankModel> lists = new List<TotalScoreRankModel>
        {
            new TotalScoreRankModel(){ TotalScoreNumber = 100, TotalScoreRankName = "S" },
            new TotalScoreRankModel(){ TotalScoreNumber = 90, TotalScoreRankName = "A" },
            new TotalScoreRankModel(){ TotalScoreNumber = 80, TotalScoreRankName = "B" },
            new TotalScoreRankModel(){ TotalScoreNumber = 70, TotalScoreRankName = "C" },
            new TotalScoreRankModel(){ TotalScoreNumber = 60, TotalScoreRankName = "D" }
        };

        return lists;
    }
    #endregion

    #region + GetTotalScore : 점수 → 등급 변환
    /// <summary>
    /// 점수를 등급으로 변환 (100점 만점 기준)
    /// </summary>
    public string GetTotalScore(double score)
    {
        string result = string.Empty;
        if (score == 100)
        {
            return result = "S";
        }
        else if (score == 90)
        {
            return result = "A";
        }
        else if (score == 80)
        {
            return result = "B";
        }
        else if (score == 70)
        {
            return result = "C";
        }
        else if (score == 60)
        {
            return result = "D";
        }
        else
        {
            return result = "-";
        }
    }
    #endregion
}
