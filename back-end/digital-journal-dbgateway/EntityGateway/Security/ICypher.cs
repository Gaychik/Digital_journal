namespace DigitalJournal.Data.Security
{
    /// <summary>
    /// Inherited class should be <see langword="static"/> or have a public parameterless constructor  
    /// </summary>
    public interface ICypher
    {
        string Encrypt(string input, object key);
        string Decrypt(string input, object key);
    }
}