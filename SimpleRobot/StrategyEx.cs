using System.ComponentModel;
using StockSharp.Algo;
using StockSharp.Algo.Strategies;



namespace SimpleRobot
{
    public class StrategyEx
    {
        /// <summary>
        /// Конструктор для креатора, проперти
        /// </summary>
            public string Name { set; get; }
            public string Description { set; get; }
            public Strategy Strategy { set; get; }
            public INotifyPropertyChanged Properties { set; get; }
            private IStrategyCreator StrategyCreator;
            private StrategyEx() { }
            public StrategyEx(INotifyPropertyChanged properties, IStrategyCreator strategyCreator) {
                Properties = properties;
                StrategyCreator = strategyCreator;
            }
            public override string ToString() {
                return Name;
            }
            public void CreateStrategy(
                INotifyPropertyChanged properties,
                Connector trader) {
                Strategy = StrategyCreator.CreateStrategy(properties, trader);
            }
        }
    }


