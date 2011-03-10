using System;

namespace AweFsm.EventArgs
{
    public class FsmExceptionEventArgs : System.EventArgs
    {
        public string Message { get; set; }
        public object[] Data { get; set; }
        public Exception Exception { get; set; }
        public bool Handled { get; set; }

        public FsmExceptionEventArgs(string message, object[] data, Exception exception)
        {
            Message = message;
            Data = data;
            Exception = exception;
            Handled = false;
        }
    }
}