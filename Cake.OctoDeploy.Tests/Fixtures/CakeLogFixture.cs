using System.Collections.Generic;
using Cake.Core.Diagnostics;

namespace Cake.OctoDeploy.Tests.Fixtures
{
    public class CakeLogFixture : ICakeLog
    {
        #region Constructor

        public CakeLogFixture()
        {
            Messages = new List<MessageWrapper>();
        }

        #endregion

        #region Properties

        public List<MessageWrapper> Messages { get; }

        public Verbosity Verbosity { get; set; }

        #endregion

        #region Public Methods

        public void Write(Verbosity verbosity, LogLevel level, string format, params object[] args)
        {
            Messages.Add(new MessageWrapper(verbosity, level, format, args));
        }

        #endregion

        public class MessageWrapper
        {
            #region Constructor

            public MessageWrapper(Verbosity verbosity, LogLevel logLevel, string format, object[] arguments)
            {
                Verbosity = verbosity;
                LogLevel = logLevel;
                Format = format;
                Arguments = arguments;
            }

            #endregion

            #region Properties

            public object[] Arguments { get; }
            public string Format { get; }
            public LogLevel LogLevel { get; }

            public Verbosity Verbosity { get; }

            #endregion
        }
    }
}