namespace Documate.Crypto {
    public interface ICryptoService {
        RecoverModel EcRecover (string sender, string signature, string hash);
        RecoverModel EcRecover (string sender, string signature, byte[] data);
        string Sign (Model model);
    }
}