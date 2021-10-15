using IXICore.Meta;
using System;
using System.IO;
using System.Reflection;

namespace IxiOfflineTools
{
    class Program
    {
        public static CliOptions cliOptions = null;

        private static Commands commands = null;
        private static Node node = null;

        static void Main(string[] args)
        {
            if (!Logging.start(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)))
            {
                IxianHandler.forceShutdown = true;
                Logging.info("Press ENTER to exit.");
                Console.ReadLine();
                return;
            }

            // Clear the console first
            Console.Clear();

            start(args);
            stop();
        }

        static void start(string[] args)
        {
            cliOptions = CliOptionsParser.readFromCommandLine(args);

            Logging.consoleOutput = cliOptions.verboseOutput;
            Logging.verbosity = cliOptions.logVerbosity;

            commands = new Commands();
            if(commands.preExecute(args))
            {
                // Initialize the node
                node = new Node();

                // Start the node
                node.start();

                if(!IxianHandler.forceShutdown)
                {
                    commands.execute(args);
                }
            }
        }

        static void stop()
        {
            // Stop the DLT
            Node.stop();

            // Stop logging
            Logging.flush();
            Logging.stop();
        }
    }
}
