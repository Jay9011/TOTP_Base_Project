namespace OTPProvider
{
    public interface IOTPProvider
    {
        /// <summary>
        /// 비밀 키를 생성하는 메서드
        /// </summary>
        /// <returns>비밀 키(검증을 위해 다른 곳에 저장해야 함)</returns>
        string GenerateSecretKey();
        /// <summary>
        /// 비밀 키를 통해 TOTP를 생성하는 메서드
        /// </summary>
        /// <param name="secretKey">생성 할 TOTP의 비밀 키</param>
        /// <returns>생성된 TOTP</returns>
        string GenerateTOTP(string secretKey);
        /// <summary>
        /// TOPT를 검증하는 메서드
        /// </summary>
        /// <param name="secretKey">처음 생성된 비밀 키</param>
        /// <param name="totp">현재 유효한 상태의 TOTP</param>
        /// <returns>검증 결과</returns>
        bool ValidateTOTP(string secretKey, string totp);
        /// <summary>
        /// 사용자 전용 TOTP를 생성하는 메서드
        /// </summary>
        /// <param name="uniqueString">사용자를 구분할 수 있는 고유한 문자열</param>
        /// <param name="secretKey">생성 할 TOTP의 비밀 키</param>
        /// <param name="serviceName">오른쪽에 붙는 추가 정보</param>
        /// <param name="issuer">OTP 발급 조직</param>
        /// <returns></returns>
        string GenerateQRCodeURL(string uniqueString, string secretKey, string serviceName, string issuer = "");
    }
}