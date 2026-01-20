using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.EvaluationUsers;

/// <summary>
/// EvaluationUsers Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class EvaluationUsersRepository(string connectionString, ILoggerFactory loggerFactory) : IEvaluationUsersRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<EvaluationUsersRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 평가 참여자 추가
    /// </summary>
    public async Task<Int64> AddAsync(EvaluationUsers model)
    {
        const string sql = """
            INSERT INTO EvaluationUsers(
                Uid, Is_Evaluation, TeamLeaderId, DirectorId, Is_TeamLeader)
            VALUES(
                @Uid, @Is_Evaluation, @TeamLeaderId, @DirectorId, @Is_TeamLeader);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 참여자 목록 조회
    /// </summary>
    public async Task<IEnumerable<EvaluationUsers>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM EvaluationUsers ORDER BY EUid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<EvaluationUsers>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 참여자 조회
    /// </summary>
    public async Task<EvaluationUsers?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM EvaluationUsers WHERE EUid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<EvaluationUsers>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 참여자 정보 수정
    /// </summary>
    public async Task<int> UpdateAsync(EvaluationUsers model)
    {
        const string sql = """
            UPDATE EvaluationUsers
            SET
                Uid = @Uid,
                Is_Evaluation = @Is_Evaluation,
                TeamLeaderId = @TeamLeaderId,
                DirectorId = @DirectorId,
                Is_TeamLeader = @Is_TeamLeader
            WHERE EUid = @EUid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 참여자 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM EvaluationUsers WHERE EUid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] Uid 존재 확인: CheckUidAsync
    /// <summary>
    /// Uid 존재 여부 확인
    /// </summary>
    public async Task<bool> CheckUidAsync(Int64 uid)
    {
        const string sql = "SELECT COUNT(*) FROM EvaluationUsers WHERE Uid = @uid";

        using var connection = new SqlConnection(dbContext);
        int count = await connection.ExecuteScalarAsync<int>(sql, new { uid });
        return count > 0;
    }
    #endregion

    #region + [7] Uid로 조회: GetByUidAsync
    /// <summary>
    /// Uid로 참여자 조회
    /// </summary>
    public async Task<EvaluationUsers?> GetByUidAsync(Int64 uid)
    {
        const string sql = "SELECT * FROM EvaluationUsers WHERE Uid = @uid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<EvaluationUsers>(sql, new { uid });
    }
    #endregion

    #region + [8] 팀원 목록: GetByTeamLeaderIdAsync
    /// <summary>
    /// 부서장의 팀원 목록 조회
    /// </summary>
    public async Task<IEnumerable<EvaluationUsers>> GetByTeamLeaderIdAsync(Int64 teamLeaderId)
    {
        const string sql = """
            SELECT * FROM EvaluationUsers
            WHERE TeamLeaderId = @teamLeaderId
            ORDER BY EUid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<EvaluationUsers>(sql, new { teamLeaderId });
    }
    #endregion

    #region + [9] 이름 검색: SearchByNameAsync
    /// <summary>
    /// 사용자 이름으로 검색 (부분 일치)
    /// 참고: 실제로는 View (v_MemberListDB 등)를 통한 조회 권장
    /// </summary>
    public async Task<IEnumerable<EvaluationUsers>> SearchByNameAsync(string userName)
    {
        // 이 메서드는 UserDb와 JOIN 필요
        // View를 통한 조회 권장
        const string sql = """
            SELECT EU.*
            FROM EvaluationUsers EU
            INNER JOIN UserDb U ON EU.Uid = U.Uid
            WHERE U.UserName LIKE @userName + '%'
            ORDER BY EU.EUid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<EvaluationUsers>(sql, new { userName });
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
