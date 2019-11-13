using MCWrapper.CLI.Extensions;
using MCWrapper.Ledger.Entities.ErrorHandling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MCWrapperConsole.ServicePipeline
{
    /// <summary>
    /// ServiceHelper object creates a psuedo Dependency Injection pipeline.
    /// </summary>
    internal static class ServicePipelineHelper
    {
        /// <summary>
        /// Encapsulate service provider and services collection.
        /// </summary>
        private static IServiceProvider Provider;
        private static IServiceCollection Services;

        /// <summary>
        /// Locate and return service type.
        /// Automatically initiliaze the IServiceProvider on firt use.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            // null detection attemtps to reinitialize the service provider.
            if (Provider == null)
                InitializeServiceProvider();

            // lookup service by Type.
            var service = Provider.GetService<T>();

            // null detection throws on no service found.
            if (service == null)
                throw new ServiceException("Service type unavailable");

            // service located successfully; return to subscriber.
            return service;
        }

        /// <summary>
        /// Build service collection and provider.
        /// </summary>
        private static void InitializeServiceProvider()
        {
            // map configuration builder to local copy of appsettings.json file.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            // fetch key/value pairs from appsettings.json file.
            IConfiguration configuration = builder.Build();

            // good place to instantiate our services collection.
            Services = new ServiceCollection();

            // Add MCWrapper MultiChain services to the services collection 
            // dependency pipeline. WARNING! If this step is not completed
            // MCWrapper will not function properly MCWrapper is heavily
            // dependent on some sort of DI Pipeline.
            Services.AddMultiChainCoreCliServices(configuration: configuration);

            // build the local service provider.
            Provider = Services.BuildServiceProvider();
        }
    }
}