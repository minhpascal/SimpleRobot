using System.ComponentModel;
using StockSharp.Algo;
using StockSharp.Algo.Strategies;

namespace SimpleRobot
{
        public interface IStrategyCreator
        {
            Strategy CreateStrategy(
                INotifyPropertyChanged properties,
                Connector trader);
        }
}

