namespace Documate.Crypto
{
    public interface ICryptoService
    {
         string EcRecover(string signature, byte[] data);
         string Sign(Model model, string privateKey );
    }
}