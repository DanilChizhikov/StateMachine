namespace MbsCore.StateMachine.Tests
{
    internal sealed class StringData : IStateData
    {
        public string Data { get; set; }

        public override string ToString()
        {
            return Data;
        }
    }
}