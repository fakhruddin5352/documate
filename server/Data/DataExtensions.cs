using System;
using Documate.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Documate.Data
{
    public static class DataExtensions
    {
        public static void AddData(this IServiceCollection services, Func<DataOptionsBuilder, DataOptionsBuilder> options) {
            var builder = options(new DataOptionsBuilder());
            services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationDbContext>(op => 
                op.UseNpgsql(builder.Connection));
            services.AddScoped<IDataService,DataService>();


        }
    }
}