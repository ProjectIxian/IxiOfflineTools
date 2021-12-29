using IXICore;
using System;
using System.Collections.Generic;
using System.Net;

namespace IxiOfflineTools.API
{
    class ApiServer : GenericAPIServer
    {
        protected override bool processRequest(HttpListenerContext context, string methodName, Dictionary<string, object> parameters)
        {
            JsonResponse response = null;

            if (methodName.Equals("verifyminingsolution", StringComparison.OrdinalIgnoreCase))
            {
                response = onVerifyMiningSolution(parameters);
            }

            switch (methodName.ToLower())
            {
                case "shutdown":
                case "decoderawtransaction":
                case "calculatetransactionfee":
                case "verify":
                case "validateaddress":
                    // allow
                    return false;

                case "mywallet":
                case "mypubkey":
                case "generatenewaddress":
                case "getwalletbackup":
                case "getviewingwallet":
                case "sign":
                case "listwallets":
                case "signrawtransaction":
                    {
                        // deny if no wallet mode
                        if (Program.cliOptions.noWallet)
                        {
                            JsonResponse errorResponse = new JsonResponse() { error = new JsonError() { code = (int)RPCErrorCode.RPC_WALLET_ERROR, message = "No wallet mode active" } }; ;
                            sendResponse(context.Response, errorResponse);
                            context.Response.Close();
                            return true;
                        }
                        // allow
                        return false;
                    }
            }

            // deny everything else
            if(response == null)
                response = new JsonResponse() { error = new JsonError() { code = (int)RPCErrorCode.RPC_METHOD_NOT_FOUND, message = "Unknown API request '" + methodName + "'" } }; ;
            sendResponse(context.Response, response);
            context.Response.Close();
            return true;
        }

        // Verifies a mining solution based on the block's difficulty
        // It does not submit it to the network.
        public JsonResponse onVerifyMiningSolution(Dictionary<string, object> parameters)
        {
            // Check that all the required query parameters are sent
            if (!parameters.ContainsKey("nonce"))
            {
                JsonError error = new JsonError { code = (int)RPCErrorCode.RPC_INVALID_PARAMETER, message = "Parameter 'nonce' is missing" };
                return new JsonResponse { result = null, error = error };
            }

            if (!parameters.ContainsKey("blockchecksum"))
            {
                JsonError error = new JsonError { code = (int)RPCErrorCode.RPC_INVALID_PARAMETER, message = "Parameter 'blockchecksum' is missing" };
                return new JsonResponse { result = null, error = error };
            }

            if (!parameters.ContainsKey("diff"))
            {
                JsonError error = new JsonError { code = (int)RPCErrorCode.RPC_INVALID_PARAMETER, message = "Parameter 'diff' is missing" };
                return new JsonResponse { result = null, error = error };
            }

            if (!parameters.ContainsKey("solver"))
            {
                JsonError error = new JsonError { code = (int)RPCErrorCode.RPC_INVALID_PARAMETER, message = "Parameter 'solver' is missing" };
                return new JsonResponse { result = null, error = error };
            }

            string nonce = (string)parameters["nonce"];
            if (nonce.Length < 1 || nonce.Length > 128)
            {
                return new JsonResponse { result = null, error = new JsonError() { code = (int)RPCErrorCode.RPC_INVALID_PARAMS, message = "Invalid nonce was specified" } };
            }

            ulong blockdiff = ulong.Parse((string)parameters["diff"]);

            byte[] solver_address = Crypto.stringToHash((string)parameters["solver"]);
            byte[] block_checksum = Crypto.stringToHash((string)parameters["blockchecksum"]);

            bool verify_result = Miner.verifyNonce_v3(nonce, block_checksum, solver_address, blockdiff);

            if (verify_result)
            {
                Console.WriteLine("Received verify share: {0} - PASSED with diff {1}", nonce, blockdiff);
            }
            else
            {
                Console.WriteLine("Received verify share: {0} - REJECTED with diff {1}", nonce, blockdiff);
            }

            return new JsonResponse { result = verify_result, error = null };
        }

    }
}
