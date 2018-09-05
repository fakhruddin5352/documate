using System;
using Microsoft.Extensions.DependencyInjection;

namespace Documate.BlockChain
{
    public static class BlockChainExtensions
    {
        public static void AddBlockChain(this IServiceCollection services, Action<BlockChainOptionsBuilder> options) {

            var builder = new BlockChainOptionsBuilder();
            options(builder);
            var op = builder.Build();

            services.AddSingleton<IBlockChainService>(provider => new BlockChainService(op.RpcEndpoint,op.DocumateAddress, op.DocumateAbi));

        }
    }
}