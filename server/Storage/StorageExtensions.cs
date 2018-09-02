using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Documate.Storage {

    public static class StorageExtensions {

        public static void AddLocalStorage (this IServiceCollection services, Func<LocalStorageOptionsBuilder, LocalStorageOptionsBuilder> localStorageOptions) {
            var builder =  localStorageOptions(new LocalStorageOptionsBuilder());
            services.AddSingleton<IStorageService> (provider =>
                new LocalStorageService (builder.Path, provider.GetService<ILogger<LocalStorageService>> ())
            );
        }
    }
}