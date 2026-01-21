using System.Security.Cryptography;
using System.Text;

namespace MdcHR26Apps.BlazorServer.Utils;

public static class PasswordHasher
{
    /// <summary>
    /// 새로운 Salt 생성 (16 bytes)
    /// </summary>
    public static byte[] GenerateSalt()
    {
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    /// <summary>
    /// 비밀번호 해시 생성 (SHA-256)
    /// </summary>
    /// <param name="password">평문 비밀번호</param>
    /// <param name="salt">Salt 값</param>
    /// <returns>32 bytes 해시</returns>
    public static byte[] HashPassword(string password, byte[] salt)
    {
        using var sha256 = SHA256.Create();

        // 비밀번호를 UTF-8 바이트로 변환
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        // Salt + Password 결합
        var combined = new byte[salt.Length + passwordBytes.Length];
        Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
        Buffer.BlockCopy(passwordBytes, 0, combined, salt.Length, passwordBytes.Length);

        // SHA-256 해시
        return sha256.ComputeHash(combined);
    }

    /// <summary>
    /// 비밀번호 검증
    /// </summary>
    /// <param name="inputPassword">입력된 평문 비밀번호</param>
    /// <param name="storedHash">DB에 저장된 해시</param>
    /// <param name="storedSalt">DB에 저장된 Salt</param>
    /// <returns>일치 여부</returns>
    public static bool VerifyPassword(string inputPassword, byte[] storedHash, byte[] storedSalt)
    {
        // 입력된 비밀번호를 같은 Salt로 해시
        var inputHash = HashPassword(inputPassword, storedSalt);

        // 바이트 배열 비교
        return CompareByteArrays(inputHash, storedHash);
    }

    /// <summary>
    /// 바이트 배열 비교 (타이밍 공격 방지)
    /// </summary>
    private static bool CompareByteArrays(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;

        int diff = 0;
        for (int i = 0; i < a.Length; i++)
        {
            diff |= a[i] ^ b[i];
        }
        return diff == 0;
    }

    /// <summary>
    /// 비밀번호 강도 검증
    /// </summary>
    /// <param name="password">비밀번호</param>
    /// <returns>오류 메시지 (null이면 통과)</returns>
    public static string? ValidatePasswordStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return "비밀번호를 입력해주세요.";

        if (password.Length < 4)
            return "비밀번호는 최소 4자 이상이어야 합니다.";

        if (password.Length > 50)
            return "비밀번호는 최대 50자까지 가능합니다.";

        return null; // 통과
    }
}
