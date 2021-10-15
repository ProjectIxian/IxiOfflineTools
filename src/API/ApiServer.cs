using IXICore;
using System.Collections.Generic;
using System.Net;

namespace IxiOfflineTools.API
{
    class ApiServer : GenericAPIServer
    {
        protected override bool processRequest(HttpListenerContext context, string methodName, Dictionary<string, object> parameters)
        {
            switch(methodName.ToLower())
            {
                case "shutdown":
                case "decoderawtransaction":
                case "signrawtransaction":
                case "calculatetransactionfee":
                case "mywallet":
                case "mypubkey":
                case "generatenewaddress":
                case "getwalletbackup":
                case "getviewingwallet":
                case "sign":
                case "verify":
                case "listwallets":
                case "validateaddress":
                    // allow
                    return false;
            }

            // deny everything else
            JsonResponse unknownRequest = new JsonResponse() { error = new JsonError() { code = (int)RPCErrorCode.RPC_METHOD_NOT_FOUND, message = "Unknown API request '" + methodName + "'" } }; ;
            sendResponse(context.Response, unknownRequest);
            context.Response.Close();
            return true;
        }
    }
}
