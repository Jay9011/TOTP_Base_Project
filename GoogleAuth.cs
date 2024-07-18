using System;
using System.Linq;
using System.Security.Cryptography;

namespace OTPProvider
{
    public class GoogleAuth: IOTPProvider
    {
        private const int Step = 30;    // OTP 갱신 주기
        private const int T0 = 0;       // Unix epoch
        private const int Digits = 6;   // OTP 길이
        
        public string GenerateSecretKey()
        {
            byte[] secretKey = new byte[20];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(secretKey);
            }
            return Base32Encoding.ToString(secretKey);
        }

        public string GenerateTOTP(string secretKey)
        {
            // 비밀 키를 Base32로 디코딩
            byte[] key = Base32Encoding.ToBytes(secretKey);
            
            // 현재 Unix 시간 얻기
            long unixTimeStamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            
            // 시간 단계 계산
            long timeStep = (unixTimeStamp - T0) / Step;
            
            // 시간 단계를 바이트 배열로 변환
            byte[] timeStepBytes = BitConverter.GetBytes(timeStep);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timeStepBytes);
            }
            
            // HMAC-SHA1 계산
            byte[] hash;
            using (var hmac = new HMACSHA1(key))
            {
                hash = hmac.ComputeHash(timeStepBytes);
            }
            
            // 동적 자릿수 계산
            int offset = hash[hash.Length - 1] & 0x0F;
            int binaryCode = (hash[offset] & 0x7F) << 24 | 
                             (hash[offset + 1] & 0xFF) << 16 |
                             (hash[offset + 2] & 0xFF) << 8 |
                             (hash[offset + 3] & 0xFF);
            
            int otp = binaryCode % (int)Math.Pow(10, Digits);

            return otp.ToString().PadLeft(Digits, '0');
        }

        public bool ValidateTOTP(string secretKey, string totp)
        {
            // 입력된 TOTP가 숫자인지, 6자리인지 확인
            if (string.IsNullOrEmpty(totp) || totp.Length != Digits || !totp.All(char.IsDigit))
            {
                return false;
            }
            
            // 현재 시간 기준으로 앞뒤 1단계씩 검사 (총 3단계 검사)
            long timeStepNow = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds / Step;

            for (long timeStep = timeStepNow; timeStep <= timeStepNow; timeStep++)
            {
                byte[] key = Base32Encoding.ToBytes(secretKey);
                byte[] timeStepBytes = BitConverter.GetBytes(timeStep);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(timeStepBytes);
                }
                
                byte[] hash;
                using (var hmac = new HMACSHA1(key))
                {
                    hash = hmac.ComputeHash(timeStepBytes);
                }
                
                int offset = hash[hash.Length - 1] & 0x0F;
                int binaryCode = (hash[offset] & 0x7F) << 24 | 
                                 (hash[offset + 1] & 0xFF) << 16 |
                                 (hash[offset + 2] & 0xFF) << 8 |
                                 (hash[offset + 3] & 0xFF);
                
                int generatedTotp = binaryCode % (int)Math.Pow(10, Digits);
                string generatedTotpString = generatedTotp.ToString().PadLeft(Digits, '0');
                
                if (generatedTotpString == totp)
                {
                    return true;
                }
            }

            return false;
        }

        public string GenerateQRCodeURL(string uniqueString, string secretKey, string serviceName, string issuer = "")
        {
            string encodedServiceName = Uri.EscapeDataString(serviceName);
            string encodedIssuer = Uri.EscapeDataString(issuer);
            string encodedUniqueString = Uri.EscapeDataString(uniqueString);
            
            return $"otpauth://totp/{encodedServiceName}:{encodedUniqueString}?secret={secretKey}&issuer={encodedIssuer}";
        }
    }
}