namespace AweFsm.EventArgs
{
    public class FsmUnknownEventArgs : System.EventArgs
    {
        public FsmUnknownEventArgs(string message, object[] data)
        {
            Message = message;
            Data = data;
        }

        public string Message { get; set; }
        public object[] Data { get; set; }
    }
}