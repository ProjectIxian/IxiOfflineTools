using IXICore;
using IXICore.Meta;
using IXICore.Network;
using IXICore.Utils;
using IxiOfflineTools.Meta;
using System;

namespace IxiOfflineTools
{
    class Balance
    {
        public byte[] address = null;
        public IxiNumber balance = 0;
        public ulong blockHeight = 0;
        public byte[] blockChecksum = null;
        public bool verified = false;
    }

    class Node : IxianNode
    {
        public Node()
        {
            IxianHandler.init(Config.version, this, NetworkType.main, true, Program.cliOptions.checksumLock);
            init();
        }

        // Perform basic initialization of node
        private void init()
        {
            Logging.consoleOutput = false;

            // Load or Generate the wallet
            if (!initWallet(Program.cliOptions.wallet, Program.cliOptions.walletPassword))
            {
                IxianHandler.forceShutdown = true;
                return;
            }
        }

        private bool initWallet(string walletFile, string walletPassword = "")
        {
            Logging.flush();
            if (Program.cliOptions.noWallet)
            {
                Console.WriteLine("Initializing with no wallet");
                return true;
            }

            WalletStorage walletStorage = new WalletStorage(walletFile);

            if (!walletStorage.walletExists())
            {
                ConsoleHelpers.displayBackupText();

                // Request a password
                string password = walletPassword;
                while (password.Length < 10)
                {
                    Logging.flush();
                    password = ConsoleHelpers.requestNewPassword("Enter a password for your new wallet: ");
                    if (IxianHandler.forceShutdown)
                    {
                        return false;
                    }
                }
                walletStorage.generateWallet(password);
            }
            else if(walletPassword == "")
            {
                ConsoleHelpers.displayBackupText();

                bool success = false;
                while (!success)
                {

                    string password = "";
                    if (password.Length < 10)
                    {
                        Logging.flush();
                        Console.Write("Enter wallet password: ");
                        password = ConsoleHelpers.getPasswordInput();
                    }
                    if (IxianHandler.forceShutdown)
                    {
                        return false;
                    }
                    if (walletStorage.readWallet(password))
                    {
                        success = true;
                    }
                }
            }else
            {
                if (!walletStorage.readWallet(walletPassword))
                {
                    Console.WriteLine("Invalid password");
                    return false;
                }
            }


            if (walletStorage.getPrimaryPublicKey() == null)
            {
                return false;
            }

            // Wait for any pending log messages to be written
            Logging.flush();

            IxianHandler.addWallet(walletStorage);

            return true;
        }

        public void start()
        {
        }

        static public void stop()
        {
            IxianHandler.forceShutdown = true;
        }

        public override ulong getHighestKnownNetworkBlockHeight()
        {
            throw new NotImplementedException();
        }

        public override Block getBlockHeader(ulong blockNum)
        {
            throw new NotImplementedException();
        }

        public override Block getLastBlock()
        {
            throw new NotImplementedException();
        }

        public override ulong getLastBlockHeight()
        {
            throw new NotImplementedException();
        }

        public override int getLastBlockVersion()
        {
            throw new NotImplementedException();
        }

        public override bool addTransaction(Transaction tx, bool force_broadcast)
        {
            throw new NotImplementedException();
        }

        public override bool isAcceptingConnections()
        {
            throw new NotImplementedException();
        }

        public override Wallet getWallet(Address id)
        {
            throw new NotImplementedException();
        }

        public override IxiNumber getWalletBalance(Address id)
        {
            throw new NotImplementedException();
        }

        public override void parseProtocolMessage(ProtocolMessageCode code, byte[] data, RemoteEndpoint endpoint)
        {
            throw new NotImplementedException();
        }

        public override void shutdown()
        {
        }

        public override IxiNumber getMinSignerPowDifficulty(ulong blockNum)
        {
            throw new NotImplementedException();
        }
    }
}
