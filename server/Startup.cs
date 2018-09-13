using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Documate.BlockChain;
using Documate.Configuration;
using Documate.Crypto;
using Documate.Data;
using Documate.Document;
using Documate.Storage;
using Documate.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Documate {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1);

            var contractsConfig = new ContractsConfig ();
            Configuration.Bind ("Contracts", contractsConfig);

            services.AddCrypto (options => options.UsePrivateKey (Configuration["Crypto:PrivateKey"])
                .AddAdditionalSignatureItem (Address.Of(contractsConfig.Documate.Address)));
            services.AddLocalStorage (options => options.UsePath (Configuration["Storage:Path"]));
            services.AddData (options => options.UseConnection (Configuration.GetConnectionString ("DefaultConnection")));
            services.AddBlockChain (options => options.UseRpcEndpoint (Configuration["BlockChain:RpcEndpoint"])
                .UseDocumentApi (contractsConfig.Document.ABI)
                .UseDocumateApi (contractsConfig.Documate.ABI)
                .UserDocumateAddress (contractsConfig.Documate.Address)
            );
            services.AddScoped<IDocumentService,DocumentService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseHsts ();
                app.UseHttpsRedirection ();
            }

            app.UseMvc ();
        }
    }
}