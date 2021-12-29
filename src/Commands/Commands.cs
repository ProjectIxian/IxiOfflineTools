using Fclp;
using IXICore;
using IXICore.Meta;
using IXICore.Utils;
using IxiOfflineTools.API;
using IxiOfflineTools.Meta;
using System;
using System.Collections.Generic;
using System.Threading;

namespace IxiOfflineTools
{
    class Commands
    {
        public Commands()
        {

        }

        public bool preExecute(string[] args)
        {
            Console.WriteLine("IXI Offline Tools {0} ({1})", Config.version, CoreConfig.version);

            string command;
            if (args.Length > 0)
            {
                command = args[0].ToLower();
            }else
            {
                command = "help";
            }
            bool continueExecution = true;
            switch (command)
            {
                case "help":
                    handleHelp();
                    continueExecution = false;
                    break;

                case "version":
                    continueExecution = false;
                    break;
            }

            return continueExecution;
        }

        public void execute(string[] args)
        {
            string command = args[0].ToLower();
            switch(command)
            {
                case "changepass":
                    handleChangePass();
                    break;

                case "start":
                    handleStartWebServer(args);
                    break;

                default:
                    Console.WriteLine("Unknown command");
                    break;
            }
        }

        void handleHelp()
        {
            Console.WriteLine("Commands;");
            Console.WriteLine("  help\t\t- shows this help message");
            Console.WriteLine("  changePass\t- change wallet password");
            Console.WriteLine("  start\t\t- Starts the web service");
            Console.WriteLine("  \t\tParameters:");
            Console.WriteLine("  \t\t--bind\t\t Specify which address to bind to (i.e.: http://localhost:80/)");
            Console.WriteLine("  \t\t--allowIP\t Which IP is allowed access to the web service (i.e.: 192.168.0.16)");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Optional Global Parameters:");
            Console.WriteLine("  --wallet\t\t Wallet file to use (default: wallet.ixi)");
            Console.WriteLine("  --walletPassword\t Decryption password for wallet");
            Console.WriteLine("  --checksumLock\t Checksum lock (default: Ixian)");
            Console.WriteLine("  --consoleOutput\t Outputs log entries to console");
            Console.WriteLine("  --logVerbosity\t Sets log verbosity(0 = none, trace = 1, info = 2, warn = 4, error = 8)");

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("  IxiOfflineTools.exe start --bind http://localhost:80/");
            Console.WriteLine("  IxiOfflineTools.exe start --bind http://localhost:80/,http://localhost:81/ --allowIP 192.168.0.16,192.168.0.17");
            Console.WriteLine("  IxiOfflineTools.exe start --bind http://localhost:80/ --allowIP 192.168.0.16 --wallet myWallet1.ixi");
        }

        void handleStartWebServer(string[] args)
        {
            List<string> binds = null;
            List<string> allowedIps = null;

            var cmdParser = new FluentCommandLineParser();

            cmdParser.Setup<string>("bind").Callback(value => binds = new List<string>(value.Split(','))).Required();
            cmdParser.Setup<string>("allowIP").Callback(value => allowedIps = new List<string>(value.Split(',')));

            cmdParser.Parse(args);

            if (binds == null)
            {
                Console.WriteLine("Error starting web service, bind parameter is missing.");
            }

            Console.WriteLine("Starting Web Service...");

            ApiServer apiServer = new ApiServer();
            apiServer.start(binds, null, allowedIps);

            Console.WriteLine("Web service started on: ");
            foreach (var bind in binds)
            {
                Console.WriteLine(bind);
            }

            Console.WriteLine("");
            if(allowedIps != null)
            {
                Console.WriteLine("Allowed IPs: ");
                foreach (var allowedIp in allowedIps)
                {
                    Console.WriteLine(allowedIp);
                }
            }

            Logging.consoleOutput = true;

            while (!IxianHandler.forceShutdown)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();

                    if (key.Key == ConsoleKey.Escape)
                    {
                        IxianHandler.forceShutdown = true;
                    }

                }
                Thread.Sleep(100);
            }

            apiServer.stop();
        }

        void handleChangePass()
        {
            if (Program.cliOptions.noWallet)
            {
                Console.WriteLine("Cannot change password in no wallet mode.");
                return;
            }

            // Request a new password
            string new_password = "";
            while (new_password.Length < 10)
            {
                new_password = ConsoleHelpers.requestNewPassword("Enter a new password for your wallet: ");
                if (new_password.Length == 0)
                {
                    continue;
                }
            }
            if (IxianHandler.getWalletStorage().writeWallet(new_password))
                Console.WriteLine("Wallet password changed.");
        }
    }
}
