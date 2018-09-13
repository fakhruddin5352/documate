using Documate.ValueObjects;

namespace Documate.Crypto {
    public interface ICryptoService {
        RecoverModel EcRecover (Address sender, Signature signature, Hash hash);
        RecoverModel EcRecover (Address sender, Signature signature, byte[] data);
        Signature Sign (Model model);
    }
}