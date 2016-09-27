using System;
using System.Collections.Generic;
using System.Linq;
using Ecng.Common;
using StockSharp.Algo;
using StockSharp.Algo.Strategies;
using StockSharp.BusinessEntities;
using StockSharp.Logging;
using MoreLinq;
using Ecng.ComponentModel;
using StockSharp.Algo.Strategies.Quoting;


namespace SimpleRobot
{
    using StockSharp.Messages;
    public class SabjStrategy : Strategy {

        public StrategyParam Params { set; get; }
        private Security secFuture;
        private Security secStock;
        private Portfolio portSt;
        private Portfolio portFt;
        private decimal _lastTradePrice;

        private LimitQuotingStrategy _bidOrder;
        private LimitQuotingStrategy _askOrder;
        private LimitQuotingStrategy _NewOrder;
        private readonly double _empiricvalue;
        private MarketRule<IConnector, IConnector> _stopRule;
        public bool IsClosePositionsOnStop { set; get; }


        //Словарь <Имя инструмента, Объем>
        private readonly Dictionary<Security, decimal> _arbitrageSecuritiesVolume = new Dictionary<Security, decimal>();

        public Dictionary<Security, decimal> ArbitrageSecuritiesVolume
        {
            get { return _arbitrageSecuritiesVolume; }
        }
        //Коэффициент для ценной бумаги (акции), чтобы уровнять отношения цены акций и фючерсов
        public decimal StockMultiplicator { get; set; }
        public decimal LastTradePrice
        {
            get { return _lastTradePrice; }
            set
            {
                if (_lastTradePrice != value) {
                    _lastTradePrice = value;
                    this.Notify("LastTradePrice");
                }
            }
        }
        protected override void OnStarted() {
            Security.WhenNewTrades(Connector).Do(() =>
            {
                if (Security.LastTrade != null) {
                    _lastTradePrice = Security.LastTrade.Price;

                }
            })
            .Apply(this);
            Security.WhenMarketDepthChanged(Connector)
            .Do(OnMarketDepthChanged)
            .Apply(this);

            //Volume = Params.;

            //Для всех указанных торговых инструментов подписываемся на правило изменения стакана
            foreach (var arbSec in _arbitrageSecuritiesVolume) {
                arbSec.Key.WhenMarketDepthChanged(Connector).Do(OnMarketDepthChanged).Apply(this);
            }
            base.OnStarted();
        }
        
        private void OnMarketDepthChanged() {
            if (Security == null || Security.BestAsk == null || Security.BestBid == null) {
                return;
            }
            var SecurityStock = Params.SecStock;
            var SecurityFuture = Params.SecFuture;

            var ask = Security.BestAsk.Price;
            var bid = Security.BestBid.Price;
            //var div = 1.97;
           // var t = 17;
            var offset = Params.Offset * Security.PriceStep;
            var div = Params.DIRV;
            var t = Params.TimeExp;
            var lot = 10;

            lock (this) {
                if (_bidOrder == null && _askOrder == null) {
                    var spread = (bid / ((ask - (decimal)div) * lot) - 1) * (365 / t);
                    if (spread >= Params.Spread) {

                        var bidPrice = bid - offset;
                        var askPrice = ask + offset;

                        secFuture = Connector.Securities.FirstOrDefault(s => s.Id == "SRZ6@FORTS");
                        secStock = Connector.Securities.FirstOrDefault(s => s.Id == "SBER@TQBR");
                        portSt = Connector.Portfolios.FirstOrDefault(p => p.Name == "100620");
                        portFt = Connector.Portfolios.FirstOrDefault(p => p.Name == "4110OFR");

                        Connector.RegisterMarketDepth(secFuture);
                        Connector.RegisterMarketDepth(secStock);

                        Connector.RegisterSecurity(secFuture);
                        Connector.RegisterSecurity(secStock);

                        this.AddWarningLog("Сигнал на вход. Bid {0}, Ask {1}", bid, ask);
                        
                        _bidOrder = new LimitQuotingStrategy(Sides.Buy, Volume, bidPrice.Value);
                        _askOrder = new LimitQuotingStrategy(Sides.Sell, Volume, askPrice.Value);

                        var newOrder = new Order();
                        {
                            newOrder.Security = secStock; //SecurityFuture;
                            newOrder.Portfolio = portFt;
                            newOrder.Price = secFuture.BestAsk.Price;
                            newOrder.Volume = 10;
                            newOrder.Direction = Sides.Buy;

                            Connector.RegisterOrder(newOrder);
                        }
                    
                        var NewOrder2 = new Order();
                        {
                            NewOrder2.Portfolio = portSt;
                            NewOrder2.Security = secFuture;
                            NewOrder2.Price = secStock.BestBid.Price;
                            NewOrder2.Volume = 1; //количество контрактов в заявке
                            NewOrder2.Direction = Sides.Sell;
                            Connector.RegisterOrder(NewOrder2);
                      
                        }
                


                _bidOrder.WhenStopped().Do(() =>
                        {
                            if (_bidOrder.LeftVolume == 0) {
                                if (_askOrder.ProcessState == ProcessStates.Stopped && _askOrder.LeftVolume == 0) {
                                    this.AddWarningLog("Trade complete.");
                                    _bidOrder = _askOrder = null;

                                    if (_stopRule != null) {
                                        _stopRule.Dispose();
                                        _stopRule = null;
                                    }
                                } else {
                                    CreateStopRule();
                                }
                            }
                        }).Apply(this);

                        _askOrder.WhenStopped().Do(() =>
                        {
                            if (_askOrder.LeftVolume == 0) {
                                if (_bidOrder.ProcessState == ProcessStates.Stopped && _bidOrder.LeftVolume == 0) {
                                    this.AddWarningLog("Trade complete.");
                                    _bidOrder = _askOrder = null;

                                    if (_stopRule != null) {
                                        _stopRule.Dispose();
                                        _stopRule = null;
                                    }
                                } else {
                                    CreateStopRule();
                                }
                            }
                        }).Apply(this);
                    }
                }
            }
        }
        //сколько держать позицию
        private void CreateStopRule() {
            _stopRule = Connector
                .WhenIntervalElapsed(TimeSpan.FromSeconds(((StrategyParam)Params).Stop))
                .Do(() =>
                {
                    _bidOrder.Stop();
                    _askOrder.Stop();

                    if (Position != 0) {
                        this.AddWarningLog("Stop time elapsed, closing position {0}", Position);

                        var direction = Position > 0 ? Sides.Sell : Sides.Buy;

                        var quoting = new MarketQuotingStrategy(direction, Position.Abs());

                        quoting.WhenStopped().Do(() =>
                        {
                            this.AddWarningLog("Trade complete by stop.");
                            _bidOrder = _askOrder = null;
                            _stopRule = null;
                        }).Apply(this);

                        ChildStrategies.Add(quoting);

                        _bidOrder.Stop();
                    }
                })
            .Once()
            .Apply(this);
        }

        //public override void StopAndClose() {
        //    if (IsClosePositionsOnStop) {
        //        this.AddWarningLog("Остановка с закрытием позиций, остаток {0}", Position);

        //        if (PositionManager.Position != 0) {
        //            var order = PositionManager.Position > 0
        //                ? MarketSell((int)PositionManager.Position.Abs())
        //                : MarketBuy((int)PositionManager.Position.Abs());

        //            RegisterOrder(order);
        //        }
        //    }

        //    Stop();
        //}

        private Order MarketSell(int volume) {
            return this.CreateOrder(Sides.Sell, Security.BestBid.Price - 100 * Security.PriceStep, volume);
        }

        private Order MarketBuy(int volume) {
            return this.CreateOrder(Sides.Buy, Security.BestAsk.Price + 100 * Security.PriceStep, volume);
        }


    }
}
