using System;
using System.Security.Cryptography;
using System.Text;

namespace OTPProvider
{
    /// <summary>
    /// Base32 인코딩을 진행하는 클래스
    /// </summary>
    public static class Base32Encoding
    {
        private const string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        /// <summary>
        /// Base32 인코딩을 진행하는 메서드
        /// </summary>
        /// <param name="input">인코딩할 바이트 배열</param>
        /// <returns>Bae32 인코딩된 문자열</returns>
        public static string ToString(byte[] input)
        {
            if (input == null || input.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();
            int bufferSize = 0;
            int buffer = 0;

            foreach (byte b in input)
            {
                buffer = (buffer << 8) | b;
                bufferSize += 8;
                while (bufferSize >= 5)
                {
                    bufferSize -= 5;
                    result.Append(Base32Chars[(buffer >> bufferSize) & 31]);
                }
            }
            
            if (bufferSize > 0)
            {
                result.Append(Base32Chars[buffer << (5 - bufferSize) & 31]);
            }

            return result.ToString();
        }
        /// <summary>
        /// Base32 디코딩을 진행하는 메서드
        /// </summary>
        /// <param name="input">디코딩할 문자열</param>
        /// <returns>Bae32 디코딩된 바이트 배열</returns>
        public static byte[] ToBytes(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new byte[0];
            }
            
            input = input.TrimEnd('=').ToUpper();
            int byteCount = input.Length * 5 / 8;
            byte[] returnArray = new byte[byteCount];
            
            byte curByte = 0, bitsRemaining = 8;
            int mask = 0, arrayIndex = 0;

            foreach (char c in input)
            {
                int cValue = Base32Chars.IndexOf(c);
                if (bitsRemaining > 5)
                {
                    mask = cValue << (bitsRemaining - 5);
                    curByte = (byte)(curByte | mask);
                    bitsRemaining -= 5;
                }
                else
                {
                    mask = cValue >> (5 - bitsRemaining);
                    curByte = (byte)(curByte | mask);
                    returnArray[arrayIndex++] = curByte;
                    curByte = (byte)(cValue << (3 + bitsRemaining));
                    bitsRemaining += 3;
                }
            }

            if (arrayIndex != byteCount)
            {
                returnArray[arrayIndex] = curByte;
            }

            return returnArray;
        }

        public static string RandomKeyGenerate(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentException("키 크기는 0보다 커야 합니다.", nameof(size));
            }

            int byteSize = (int)Math.Ceiling(size * 5 / 8.0);
            byte[] randomBytes = new byte[byteSize];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            
            return ToString(randomBytes).Substring(0, size);
        }
    }
}