using System;
using StockSharp.Algo;
using StockSharp.Messages;

namespace TraderConnection
{
	using MoreLinq;
    public class SafeConnection
    {
        public Connector Trader { get; private set; }
        public event Action<Exception> NewException = delegate { };
        public SafeConnection(Connector trader) {
            Trader = trader;
        }
        protected virtual void OnNewException(Exception exception) {
            NewException(exception);
        }
        public void ConnectSafe() {
            var baseTrader = Trader;
            if (baseTrader != null) { }
            Trader.ConnectionError += OnNewException;
            Trader.OrdersRegisterFailed += fails => fails.ForEach(f => OnNewException(f.Error));
            Trader.Connect();
        }
        public void DisConnectSafe() {
            if (Trader != null && Trader.ConnectionState == ConnectionStates.Connected) { Trader.Disconnect(); }
        }
        public bool IsConnected
        {
            get
            { return Trader != null && Trader.ConnectionState == ConnectionStates.Connected; }
        }
    }
}
