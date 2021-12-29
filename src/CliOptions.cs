using Fclp;
using System.Text;

namespace IxiOfflineTools
{
    class CliOptions
    {
        public string wallet = "wallet.ixi";
        public string walletPassword = "";
        public byte[] checksumLock = null;
        public bool verboseOutput = false;
        public int logVerbosity = 14;
        public bool noWallet = false;
    }

    class CliOptionsParser
    {
        public static CliOptions readFromCommandLine(string[] args)
        {
            CliOptions options = new CliOptions();

            var cmdParser = new FluentCommandLineParser();

            cmdParser.Setup<string>('w', "wallet").Callback(value => options.wallet = value);
            cmdParser.Setup<string>("walletPassword").Callback(value => options.walletPassword = value);
            cmdParser.Setup<string>("checksumLock").Callback(value => options.checksumLock = Encoding.UTF8.GetBytes(value));
            cmdParser.Setup<bool>("verboseOutput").Callback(value => options.verboseOutput = false);
            cmdParser.Setup<int>("logVerbosity").Callback(value => options.logVerbosity = value);
            cmdParser.Setup<bool>("noWallet").Callback(value => options.noWallet = true);

            cmdParser.Parse(args);

            return options;
        }
    }
}
