using System.Security.Cryptography;
using System.Text;

namespace SignalRChatService {
    public class SHA1Hash {

        /// <summary>
        /// Generates an SHA1 Hash for the specified text
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string Generate(string text) {
            using (var sha1 = new SHA1Managed()) {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(text));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (var b in hash) {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}
