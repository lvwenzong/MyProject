using Sgmall.Core;
using System;

namespace Sgmall.Web.Framework
{
    public class SgmallApiException : SgmallException
    {
        private Enum _ErrorCode { get; set; }
        public Enum ErrorCode
        {
            get
            {
                return _ErrorCode;
            }
        }

        private string _Message { get; set; }
        public override string Message
        {
            get
            {
                string result = base.Message;
                if (!string.IsNullOrWhiteSpace(_Message))
                {
                    result = _Message;
                }
                else
                {
                    _ErrorCode = ApiErrorCode.System_Error;
                }
                return result;
            }
        }

        /// <summary>
        /// Api统一异常
        /// </summary>
        public SgmallApiException() { }
        /// <summary>
        /// Api统一异常
        /// </summary>
        public SgmallApiException(Enum errorcode, string message) : base(errorcode.ToString()+":"+message)
        {
            this._ErrorCode = errorcode;
            this._Message = message;
        }

        /// <summary>
        /// Api统一异常
        /// </summary>
        public SgmallApiException(string message) : base(message) { }

        /// <summary>
        /// Api统一异常
        /// </summary>
        public SgmallApiException(string message, Exception inner) : base(message, inner) { }
    }
}
