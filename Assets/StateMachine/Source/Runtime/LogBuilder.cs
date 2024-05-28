using System.Text;
using UnityEngine;

namespace MbsCore.StateMachine
{
    public sealed class LogBuilder
    {
        private const string LogTemplate = "<color={0}>[{1}]</color> {2}";
        private const string LogStingTemplate = "[{0}] {1}";
        private const string LogColor = "white";
        private const string WarnColor = "yellow";
        private const string ErrorColor = "red";
        
        private readonly string _name;
        private readonly StringBuilder _stringBuilder;

        public LogBuilder(string name)
        {
            _name = name;
            _stringBuilder = new StringBuilder();
        }
        
        public LogBuilder Append(string msg)
        {
            _stringBuilder.Append(msg);
            return this;
        }
        
        public LogBuilder AppendLine(string msg)
        {
            _stringBuilder.AppendLine(msg);
            return this;
        }
        
        public LogBuilder AppendFormat(string msg, params object[] args)
        {
            _stringBuilder.AppendFormat(msg, args);
            return this;
        }

        public void Log()
        {
            Debug.LogFormat(LogTemplate, LogColor, _name, _stringBuilder);
            Clear();
        }

        public void Warn()
        {
            Debug.LogWarningFormat(LogTemplate, WarnColor, _name, _stringBuilder);
            Clear();
        }
        
        public void Error()
        {
            Debug.LogErrorFormat(LogTemplate, ErrorColor, _name, _stringBuilder);
            Clear();
        }
        
        public override string ToString()
        {
            return string.Format(LogStingTemplate, _name, _stringBuilder);
        }
        
        public void Clear()
        {
            _stringBuilder.Clear();
        }
    }
}