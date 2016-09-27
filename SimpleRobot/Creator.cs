using System;
using System.ComponentModel;
using System.Linq;
using StockSharp.Algo;
using StockSharp.Algo.Strategies;
using SimpleRobot;

namespace SimpleRobot
{
    public class Creator: IStrategyCreator 
    {
            private Strategy CreateStrategy(
                StrategyParam properties,
                Connector trader) {
                SabjStrategy strategy;

                var security1 = trader.Securities.FirstOrDefault(s => s.Id == properties.SecFuture);
                var security2 = trader.Securities.FirstOrDefault(s => s.Id == properties.SecStock);

                if (security1 == null || security2 == null) {
                    throw new InvalidOperationException(string.Format("Security {0} or {1} is not found", properties.SecFutureVolume, properties.SecStockVolume));
                }
                trader.RegisterMarketDepth(security1);
                trader.RegisterMarketDepth(security2);

                var portfolio1 = trader.Portfolios.FirstOrDefault(p => p.Name == properties.PortfolioForFuture);
                var portfolio2 = trader.Portfolios.FirstOrDefault(p => p.Name == properties.PortfolioForStock);

                if (portfolio1 == null|| portfolio2 == null) throw new InvalidOperationException(string.Format("Portfolio {0} is not found", properties.PortfolioForStock, properties.PortfolioForFuture));

                trader.RegisterPortfolio(portfolio1);
                trader.RegisterPortfolio(portfolio2);
                
            
            strategy = new SabjStrategy()
            {
                Volume = properties.SecFutureVolume,
                Connector = trader,
                Portfolio = portfolio1,
                Security = security1,
                
                //SpreadToGenerateSignal = properties.Spread,
            };
            strategy.ArbitrageSecuritiesVolume.Add(security1, properties.SecFutureVolume);
            strategy.ArbitrageSecuritiesVolume.Add(security2, properties.SecStockVolume);
            return strategy;
            }
            public Strategy CreateStrategy(INotifyPropertyChanged properties, Connector trader) {
                var propertiesEx = properties as StrategyParam;
                if (propertiesEx == null)
                    return null;

                return CreateStrategy(propertiesEx, trader);
            }

        }
    }

