using System.Text;

namespace DigitalJournal.Data.Security
{
    internal class DefaultEncryption : ICypher
    {
        static string EncryptDecrypt(string input, object key)
        {

            StringBuilder szInputStringBuild = new StringBuilder(input);
            StringBuilder szOutStringBuild = new StringBuilder(input.Length);
            char Textch;
            for (int iCount = 0; iCount < input.Length; iCount++)
            {
                Textch = szInputStringBuild[iCount];
                Textch = (char)(Textch ^ (int)key);
                szOutStringBuild.Append(Textch);
            }
            return szOutStringBuild.ToString();
        }

        public string Decrypt(string input, object key) =>
            EncryptDecrypt(input, key);

        public string Encrypt(string input, object key) =>
            EncryptDecrypt(input, key);
    }
}