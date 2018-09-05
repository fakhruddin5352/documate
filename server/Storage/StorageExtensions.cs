using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Documate.Storage {

    public static class StorageExtensions {

        public static void AddLocalStorage (this IServiceCollection services, Func<LocalStorageOptionsBuilder, LocalStorageOptionsBuilder> localStorageOptions) {
            var options  = localStorageOptions(new LocalStorageOptionsBuilder()).Build();
            services.AddSingleton<IStorageService> (provider =>
                new LocalStorageService (options.Path, provider.GetService<ILogger<LocalStorageService>> ())
            );
        }
    }
}