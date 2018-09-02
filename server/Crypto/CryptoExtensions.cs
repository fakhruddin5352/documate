using Documate.Crypto;
using Microsoft.Extensions.DependencyInjection;

namespace Documate.Crypto
{
    public static class CryptoExtensions
    {
        public static void AddCrypto(this IServiceCollection services ) {
            services.AddSingleton<ICryptoService,CryptoService>();
        }
    }
}