using System.Security.Cryptography;

namespace CarBooking.Helpers
{
    public static class CustomTotpHelper
    {
        public static string GenerateBase32Secret(int length = 20)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static byte[] Base32Decode(string base32)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var output = new List<byte>();

            int buffer = 0, bitsLeft = 0;

            foreach (char c in base32.TrimEnd('='))
            {
                int val = alphabet.IndexOf(c);
                if (val < 0) throw new ArgumentException("Invalid Base32");

                buffer = (buffer << 5) | val;
                bitsLeft += 5;

                if (bitsLeft >= 8)
                {
                    output.Add((byte)(buffer >> (bitsLeft - 8)));
                    bitsLeft -= 8;
                }
            }
            return output.ToArray();
        }

        public static string GenerateTotp6(string base32Secret, long offset = 0)
        {
            byte[] key = Base32Decode(base32Secret);

            long timestep =
                (DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30) + offset;

            byte[] timestepBytes = BitConverter.GetBytes(timestep);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(timestepBytes);

            using var hmac = new HMACSHA1(key);
            byte[] hash = hmac.ComputeHash(timestepBytes);

            int offsetBits = hash[^1] & 0x0F;

            int binary =
                ((hash[offsetBits] & 0x7F) << 24) |
                ((hash[offsetBits + 1] & 0xFF) << 16) |
                ((hash[offsetBits + 2] & 0xFF) << 8) |
                (hash[offsetBits + 3] & 0xFF);

            int otp = binary % 1_000_000; 
            return otp.ToString("D6");
        }

        public static bool VerifyTotp(string secret, string userOtp)
        {
            return
                GenerateTotp6(secret) == userOtp ||
                GenerateTotp6(secret, -1) == userOtp ||
                GenerateTotp6(secret, 1) == userOtp;
        }
    }
}
