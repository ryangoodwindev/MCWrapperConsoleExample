using MCWrapper.CLI.Ledger.Clients;
using MCWrapper.Ledger.Entities.ErrorHandling;
using MCWrapperConsole.ServicePipeline;
using System;
using System.Threading.Tasks;

namespace MCWrapperConsole
{
    /// <summary>
    /// Program demonstrates how easy it can be to create a new MultiChain Core blockchain ledger node using the MCWrapper.CLI NuGet package.
    /// 
    /// Disclaimer: I am not associated or affiliated with the MultiChain team. 
    ///             They are absolutely awesome folks (I assume, since their product is rad, never met them personally though).
    ///             I simply enjoy their product and decided to write a package for personal use, then ended up sharing 
    ///             it with the public. Thanks.
    ///             
    ///             MCWrapper.CLI - C# 8.0 / .NET Core 2.1, 2.2, and 3.0 Command Line Interface (multichain-cli.exe) support for the MultiChain Core library.
    ///             https://www.nuget.org/packages/MCWrapper.CLI/
    /// 
    ///             MCWrapper.RPC - C# 8.0 / .NET Core 2.1, 2.2, and 3.0 JSON-RPC support for the MultiChain Core library.
    ///             https://www.nuget.org/packages/MCWrapper.RPC/
    ///             
    ///             MCWrapper - A combining of MCWrapper.CLI and MCWrapper.RPC into a single convenient package. New Extension method and ClientFactory available as well.
    ///             https://www.nuget.org/packages/MCWrapper/
    /// 
    /// Note 1: You must download and install MultiChain Core to your local system before compiling the code below. For the easiest configuration
    ///         ensure that you place the uncompressed files at C:\multichain (Win) or /usr/bin/local (Linux) and the MultiChain Core binaries will be
    ///         automatically detected by the MCWrapper.CLI ForgeClient class.
    ///         
    /// Installing MultiChain Community on Linux (Courtesy of https://www.multichain.com/download-community/)
    ///   su(enter root password)
    ///   cd /tmp
    ///   wget https://www.multichain.com/download/multichain-2.0.3.tar.gz
    ///   tar -xvzf multichain-2.0.3.tar.gz
    ///   cd multichain-2.0.3
    ///   mv multichaind multichain-cli multichain-util /usr/local/bin(to make easily accessible on the command line)
    ///   exit(to return to your regular user)
    ///   
    /// Installing MultiChain Community on Windows
    ///     Download https://www.multichain.com/download/multichain-windows-2.0.3.zip and extract its contents 
    ///     to C:\ or C:\multichain (this way you avoid needing to configure the MCWrapper.CLI CliOptions 'ChainBinaryLocation' property.
    ///     MCWrapper.CLI will automatcially look at either of these default locations for the necessary MultiChain Core exe file(s)) 
    ///     when using any MCWrapper.CLI client
    ///     
    /// 
    /// Note 2: At a minimum a 'blockchainName' parameter must either be passed to all MCWrapper.CLI methods explicitly or when permitted the 'blockchainName'
    ///         parameter can be inferred if stored in the local Environment Variable store as Key: ChainName and Value: Your desired MultiChain blockchain name goes here.
    ///         Furthermore, consumers may populate the 'ChainName' variable to an appsettings.json file which will be inferred from the IConfiguration pipeline, generally.
    /// 
    /// Note 3: The MCWrapper.CLI ForgeClient class methods always require a 'blockchainName' parameter. This is to ensure the value is always present, obviously.
    /// 
    /// Note 4: The MCWrapper.CLI CliClient's all contain explicit and inferred 'blockchainName' methods. An exception is thrown when no 'blockchainName' parameter
    ///         is present due to lack of consumer configuration or is absent due to lack of explicit statement.
    ///         
    /// Note 5: The demo uses a MultiChain blockchain that does not generate native currency since that requires additional params.dat configuration we are not going to 
    ///         cover during this tutorial.
    /// 
    /// That's it for now. 
    ///     Go get started, make some mistakes, learn a few things, and don't forget to have fun.
    ///     
    /// </summary>
    class Program
    {
        /// <summary>
        /// MCWrapper Command Line Interface (Cli) factory serves up all available CliClient services. Included in the package at, MCWrapper.CLI.Ledger.Clients
        /// </summary>
        private static readonly CliClientFactory _factory = ServicePipelineHelper.GetService<CliClientFactory>();

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            // *****************************************************************
            // Blockchain name to be used for this demo.
            //
            const string BLOCKCHAIN_NAME = "ExampleChain";

            // *****************************************************************
            // Attempt to create the example blockchain named 'ExampleChain'.
            //
            // Note: To increase stability a 'blockchainName' parameter is always required by the ForgeClient.CreateBlockchainAsync() method.
            //
            var create = await _factory.ForgeClient.CreateBlockchainAsync(blockchainName: BLOCKCHAIN_NAME);
            if (!create.Success)
                throw new ServiceException($"Unable to create blockchain"); // custom exception included in package at, MCWrapper.Ledger.Entities.ErrorHandling.

            // *****************************************************************
            // Attempt to start the new blockchain.
            //
            // Note: To increase stability a 'blockchainName' parameter is always required by the ForgeClient.StartBlockchainAsync() method.
            //
            var start = await _factory.ForgeClient.StartBlockchainAsync(blockchainName: BLOCKCHAIN_NAME);
            if (!start.Success)
                throw new ServiceException($"Unable to start blockchain"); // custom exception included in package at, MCWrapper.Ledger.Entities.ErrorHandling.

            // *****************************************************************
            // Attempt to use the 'getblockchaininfo' blockchain method.
            // 
            // Additionally, instead of explicitly passing the blockchain name,
            // consumers may also choose to infer the 'blockchainName' through the local
            // Environment Variable store or via the IConfiguration pipeline (appsettings.json).
            // Simply name the Key 'ChainName' and set the Value to whatever you want to or have already named your MultiChain blockchain.
            //
            // Example of an inferred method: var info = await _factory.BlockchainCliClient.GetBlockchainInfoAsync();
            //
            var info = await _factory.BlockchainCliClient.GetBlockchainInfoAsync(blockchainName: BLOCKCHAIN_NAME);
            foreach (var prop in info.Result.GetType().GetProperties()) // reflect each property.
                Console.WriteLine($"{prop.Name}: {prop.GetValue(info.Result)}"); // print each property Name and Value to Console.

            // *****************************************************************
            // Attempt to stop the new blockchain.
            //
            // Note: To increase stability a 'blockchainName' parameter is always required by the ForgeClient.StopBlockchainAsync() method.
            //
            var stop = await _factory.ForgeClient.StopBlockchainAsync(BLOCKCHAIN_NAME);
            if (!stop.Success)
                throw new ServiceException($"Unable to stop blockchain"); // custom exception included in package at, MCWrapper.Ledger.Entities.ErrorHandling.

            // *****************************************************************
            // Super easy, right! In the next tutorial we will create a cold node based on the hote node we just created. Thanks.
            //
            // Questions? Use my contact form at https://ryangoodwin.dev (Hint: it's at the bottom of the page).
        }
    }
}
