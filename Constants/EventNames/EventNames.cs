using System;
using System.Collections.Generic;
using System.Text;

namespace AzureEventhubProtocol.Constants.EventNames
{
    public static class EventNames
    {
        public const string REGISTER = "register";
        public const string SIGN_OUT = "signOut";
        public const string STATUS_REQUEST = "statusRequest";
        public const string STATUS_RESPONSE = "status";

        // ERC20
        public const string ERC20_TRANSFER_REQUEST = "requestErc20Transfer";
        public const string ERC20_TRANSFER_SAVED = "requestErc20Saved";
        public const string ERC20_TRANSFER_EXECUTED = "requestErc20Executed";
        public const string ERC20_TRANSFER_FAILED = "requestErc20Failed";
    }
}
