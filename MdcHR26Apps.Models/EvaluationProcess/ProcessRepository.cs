using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.EvaluationProcess;

/// <summary>
/// ProcessDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class ProcessRepository(string connectionString, ILoggerFactory loggerFactory) : IProcessRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<ProcessRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 평가 프로세스 추가
    /// </summary>
    public async Task<Int64> AddAsync(ProcessDb model)
    {
        const string sql = """
            INSERT INTO ProcessDb(
                Uid, TeamLeaderId, DirectorId,
                Is_Request, Is_Agreement, Agreement_Comment,
                Is_SubRequest, Is_SubAgreement, SubAgreement_Comment,
                Is_User_Submission, Is_Teamleader_Submission, Is_Director_Submission,
                FeedBackStatus, FeedBack_Submission)
            VALUES(
                @Uid, @TeamLeaderId, @DirectorId,
                @Is_Request, @Is_Agreement, @Agreement_Comment,
                @Is_SubRequest, @Is_SubAgreement, @SubAgreement_Comment,
                @Is_User_Submission, @Is_Teamleader_Submission, @Is_Director_Submission,
                @FeedBackStatus, @FeedBack_Submission);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 프로세스 목록 조회
    /// </summary>
    public async Task<IEnumerable<ProcessDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM ProcessDb ORDER BY Pid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ProcessDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 프로세스 조회
    /// </summary>
    public async Task<ProcessDb?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM ProcessDb WHERE Pid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<ProcessDb>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 프로세스 정보 수정
    /// </summary>
    public async Task<int> UpdateAsync(ProcessDb model)
    {
        const string sql = """
            UPDATE ProcessDb
            SET
                Uid = @Uid,
                TeamLeaderId = @TeamLeaderId,
                DirectorId = @DirectorId,
                Is_Request = @Is_Request,
                Is_Agreement = @Is_Agreement,
                Agreement_Comment = @Agreement_Comment,
                Is_SubRequest = @Is_SubRequest,
                Is_SubAgreement = @Is_SubAgreement,
                SubAgreement_Comment = @SubAgreement_Comment,
                Is_User_Submission = @Is_User_Submission,
                Is_Teamleader_Submission = @Is_Teamleader_Submission,
                Is_Director_Submission = @Is_Director_Submission,
                FeedBackStatus = @FeedBackStatus,
                FeedBack_Submission = @FeedBack_Submission
            WHERE Pid = @Pid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 프로세스 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM ProcessDb WHERE Pid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 사용자별 조회: GetByUidAsync
    /// <summary>
    /// 사용자 ID로 프로세스 조회
    /// </summary>
    public async Task<ProcessDb?> GetByUidAsync(Int64 uid)
    {
        const string sql = "SELECT * FROM ProcessDb WHERE Uid = @uid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<ProcessDb>(sql, new { uid });
    }
    #endregion

    #region + [7] 합의 요청 확인: IsRequestCheckAsync
    /// <summary>
    /// 합의 요청 여부 확인
    /// 개선: Boolean 비교 수정 ('true' → 1)
    /// </summary>
    public async Task<bool> IsRequestCheckAsync(Int64 uid)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM ProcessDb
            WHERE Uid = @uid AND Is_Request = 1
            """;

        using var connection = new SqlConnection(dbContext);
        int count = await connection.ExecuteScalarAsync<int>(sql, new { uid });
        return count > 0;
    }
    #endregion

    #region + [8] 합의 완료 확인: IsAgreementCheckAsync
    /// <summary>
    /// 합의 완료 여부 확인
    /// 개선: Boolean 비교 수정 ('true' → 1)
    /// </summary>
    public async Task<bool> IsAgreementCheckAsync(Int64 uid)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM ProcessDb
            WHERE Uid = @uid AND Is_Agreement = 1
            """;

        using var connection = new SqlConnection(dbContext);
        int count = await connection.ExecuteScalarAsync<int>(sql, new { uid });
        return count > 0;
    }
    #endregion

    #region + [9] 평가 제출 확인: IsReportSubmissionCheckAsync
    /// <summary>
    /// 평가 제출 여부 확인
    /// 개선: Boolean 비교 수정 ('true' → 1)
    /// </summary>
    public async Task<bool> IsReportSubmissionCheckAsync(Int64 uid)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM ProcessDb
            WHERE Uid = @uid AND Is_User_Submission = 1
            """;

        using var connection = new SqlConnection(dbContext);
        int count = await connection.ExecuteScalarAsync<int>(sql, new { uid });
        return count > 0;
    }
    #endregion

    #region + [10] 사용자 존재 확인: CheckUidAsync
    /// <summary>
    /// 사용자 존재 여부 확인
    /// </summary>
    public async Task<bool> CheckUidAsync(Int64 uid)
    {
        const string sql = "SELECT COUNT(*) FROM ProcessDb WHERE Uid = @uid";

        using var connection = new SqlConnection(dbContext);
        int count = await connection.ExecuteScalarAsync<int>(sql, new { uid });
        return count > 0;
    }
    #endregion

    #region + [11] 부서장 팀원 목록: GetByTeamLeaderIdAsync
    /// <summary>
    /// 부서장 관할 팀원 목록 조회
    /// </summary>
    public async Task<IEnumerable<ProcessDb>> GetByTeamLeaderIdAsync(Int64 teamLeaderId)
    {
        const string sql = """
            SELECT * FROM ProcessDb
            WHERE TeamLeaderId = @teamLeaderId
            ORDER BY Pid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ProcessDb>(sql, new { teamLeaderId });
    }
    #endregion

    #region + [12] 부서장 팀원 중 제출 완료: GetByTeamLeaderIdWithUserSubmissionAsync
    /// <summary>
    /// 부서장 관할 중 사용자 제출 완료 목록
    /// </summary>
    public async Task<IEnumerable<ProcessDb>> GetByTeamLeaderIdWithUserSubmissionAsync(Int64 teamLeaderId)
    {
        const string sql = """
            SELECT * FROM ProcessDb
            WHERE TeamLeaderId = @teamLeaderId
              AND Is_User_Submission = 1
            ORDER BY Pid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ProcessDb>(sql, new { teamLeaderId });
    }
    #endregion

    #region + [13] 임원 관할 목록: GetByDirectorIdAsync
    /// <summary>
    /// 임원 관할 팀원 목록 조회
    /// </summary>
    public async Task<IEnumerable<ProcessDb>> GetByDirectorIdAsync(Int64 directorId)
    {
        const string sql = """
            SELECT * FROM ProcessDb
            WHERE DirectorId = @directorId
            ORDER BY Pid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ProcessDb>(sql, new { directorId });
    }
    #endregion

    #region + [14] 임원 관할 중 제출 완료: GetByDirectorIdWithTeamleaderSubmissionAsync
    /// <summary>
    /// 임원 관할 중 부서장 제출 완료 목록
    /// </summary>
    public async Task<IEnumerable<ProcessDb>> GetByDirectorIdWithTeamleaderSubmissionAsync(Int64 directorId)
    {
        const string sql = """
            SELECT * FROM ProcessDb
            WHERE DirectorId = @directorId
              AND Is_Teamleader_Submission = 1
            ORDER BY Pid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ProcessDb>(sql, new { directorId });
    }
    #endregion

    #region + [15] 부서장 제출 완료 목록: GetByTeamLeaderIdWithTeamLeaderSubmissionAsync
    /// <summary>
    /// 부서장 평가 제출 완료 목록
    /// </summary>
    public async Task<IEnumerable<ProcessDb>> GetByTeamLeaderIdWithTeamLeaderSubmissionAsync(Int64 teamLeaderId)
    {
        const string sql = """
            SELECT * FROM ProcessDb
            WHERE TeamLeaderId = @teamLeaderId
              AND Is_Teamleader_Submission = 1
            ORDER BY Pid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ProcessDb>(sql, new { teamLeaderId });
    }
    #endregion

    #region + [#] Dispose
    /// <summary>
    /// 리소스 해제
    /// </summary>
    public void Dispose()
    {
        db?.Dispose();
        GC.SuppressFinalize(this);
    }
    #endregion
}
