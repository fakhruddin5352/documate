using System;
using System.Linq;
using Documate.Crypto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Documate.Crypto {
    public static class CryptoExtensions {
        public static void AddCrypto (this IServiceCollection services, Action<CryptoOptionsBuilder> options) {
            var builder = new CryptoOptionsBuilder ();
            options (builder);
            var op = builder.Build ();
            services.AddSingleton<ICryptoService> (provider =>
                new CryptoService (provider.GetService<ILogger<CryptoService>>(), op.PrivateKey, op.AdditionalSignatureItems.ToArray ())
            );
        }
    }
}