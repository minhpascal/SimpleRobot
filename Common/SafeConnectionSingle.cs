using System;
using TraderConnection;

namespace CommonRobot
{
    public class SafeConnectionSingle
    {
        private static SafeConnection _instance;
        
        private SafeConnectionSingle() { }

        public static void Init(SafeConnection connection)
        {
            _instance = connection;
        }

        public static SafeConnection Instance
        {
            get
            {if (_instance == null) 
                {
                    throw new NullReferenceException("SafeConnection Instance is null");
                } return _instance; }
            
        }
    }
}
