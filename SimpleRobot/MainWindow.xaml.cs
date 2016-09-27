using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Ecng.Xaml;
using StockSharp.Algo;
using StockSharp.Algo.Strategies;
using StockSharp.Logging;
using StockSharp.Quik;
using StockSharp.Xaml;
using StockSharp.BusinessEntities;
using StockSharp.Messages;
using TraderConnection;
using CommonRobot;
using System.Windows.Input;

namespace SimpleRobot
{
    using StockSharp.Messages;
    public partial class MainWindow : Window
    {
        public static RoutedCommand ConnectCommand = new RoutedCommand();
        public static RoutedCommand DisconnectCommand = new RoutedCommand();
        public static RoutedCommand ExitCommand = new RoutedCommand();
        public static RoutedCommand SettingsCommand = new RoutedCommand();

        private readonly SafeConnection _safeConnection;
        public SecurityTypes? SecurityTypes { get; private set; }
        public ObservableCollection<Trade> Trades { get; private set; }
        //public ObservableCollection<QuoteS> Quotes { get; private set; }
        public ObservableCollection<StrategyEx> Strategies { set; get; }
        readonly MonitorWindow _monitorWindow = new MonitorWindow();
        readonly LogManager _logManager = new LogManager();
        private QuikTrader _trader = new QuikTrader();
    


        private StrategyEx strategyEx;
        //private Security secFuture;
        //private Security secStock;
        //private Portfolio portSt;
        //private Portfolio portFt;

        public MainWindow() {
            InitializeComponent();
            var quikPath = QuikTerminal.GetDefaultPath();
            _safeConnection = new SafeConnection(new QuikTrader(quikPath));
            _safeConnection.Trader.NewSecurities += (s) => { };
            _safeConnection.Trader.MarketDepthsChanged += (depths) => { foreach (var depth in depths) { } };
            SafeConnectionSingle.Init(_safeConnection);
            _safeConnection.NewException += e => { Debug.WriteLine(e.ToString()); };
            Strategies = new ObservableCollection<StrategyEx>();

            InitialStrategies();
            this.DataContext = this;
            InitLog();
        }
        protected override void OnClosing(CancelEventArgs e) {
            _monitorWindow.DeleteHideable();
            _monitorWindow.Close();
            base.OnClosing(e);
        }
        private void InitLog() {
            _logManager.Sources.Add(_safeConnection.Trader);
            _logManager.Listeners.Add(new FileLogListener("log.txt"));
            _logManager.Listeners.Add(new GuiLogListener(_monitorWindow));
        } //Окно логов

        private async void btnConnect_Click(object sender, RoutedEventArgs e) {
            if (_safeConnection == null)
                return;
            var BtnConnect = sender as Button;

            if (_safeConnection.IsConnected) {
                BtnConnect.Content = "Connect";
                BtnConnect.IsEnabled = false;
                Action disconnected = null;
                disconnected = () =>
                {
                    _safeConnection.Trader.Disconnected -= disconnected;
                    this.GuiAsync(() => { BtnConnect.IsEnabled = true; });
                };
                _safeConnection.Trader.Disconnected += disconnected;
                await Task.Run(() => _safeConnection.DisConnectSafe());
            } else {
                BtnConnect.Content = "Disconnect";
                BtnConnect.IsEnabled = false;
                _safeConnection.Trader.Connected += () => this.GuiAsync(() => BtnConnect.IsEnabled = true);
                await Task.Run(() => _safeConnection.ConnectSafe());
            }
        }

        private void InitialStrategies() {
            Strategies.Clear();

            Strategies.Add(
                new StrategyEx(new StrategyParam(), new Creator())
                {
                    Name = "Simple Strategy",
                    Description = "Если спред больше эмпирического значения:" +
                    "покупаем бумагу, продаем фьючерс.",
                });
            Strategies.Add(
                new StrategyEx(new StrategyParam(), new Creator())
                {
                    Name = "Strategy Sync",
                    Description = "Стартегия по перекладке с учетом спреда." +
                    " Расчет спреда и анализ часового avg",
                });
            Strategies.Add(
                new StrategyEx(new StrategyParam(), new Creator())
                {
                    Name = "Arbitrage Strategy",
                    Description = "Арбитражная стратегия (в разработке)",
                });

        }
        private List<Strategy> GetAllStrategy() {
            var listStrategy = new List<Strategy>();

            foreach (var item in StrategiesList.Items) {
                var strategy = ((StrategyEx)item).Strategy;
                if (strategy != null)
                    listStrategy.Add(strategy);
            }
            return listStrategy;

        }


        private void StrategiesList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var strategyEx = ((StrategyEx)(StrategiesList.SelectedItem));

            if (e.RemovedItems.Count > 0) {
                foreach (var item in e.RemovedItems) {
                    var strategy = ((StrategyEx)item).Strategy;
                }
            }
            if (e.AddedItems.Count > 0) {
                foreach (var item in e.AddedItems) {
                    var strategy = ((StrategyEx)item).Strategy;
                }
                if (strategyEx != null)
                    tbStrategyInfo.Text = strategyEx.Description;
            }
            foreach (var item in e.RemovedItems) {
                var strategy = ((StrategyEx)item).Strategy;

                if (strategy != null) {
                    StopGetInformationStrategy(strategy);
                }
            }
            if (StrategiesList.SelectedItems.Count != 1)
                return;

            if (strategyEx.Strategy == null) {

            } else {
                StartGetInformationStrategy(strategyEx.Strategy);
            }
        }
        private void StartGetInformationStrategy(Strategy strategy) {
            Debug.WriteLine("StartGetInformationStrategy " + strategy.Name);
        }
        private void StopGetInformationStrategy(Strategy strategy) {
            Debug.WriteLine("StopGetInformationStrategy " + strategy.Name);
        }
        private void ButtonCreateStartStrategy_Click(object sender, RoutedEventArgs e) {
            if (StrategiesList.SelectedItems.Count != 1) {
                MessageBox.Show("Выберите стратегию, не забудьте указать Properties.");
                return;
            }
            // Стакан
            strategyEx = ((StrategyEx)(StrategiesList.SelectedItem));
            if (strategyEx.Strategy == null) {

                strategyEx.CreateStrategy(
                   strategyEx.Properties,
                   _safeConnection.Trader);
                if (strategyEx.Strategy.ProcessState == ProcessStates.Started) {
                    strategyEx.Strategy.Stop();
                    ((Button)sender).Content = "Start";
                } else

                if (strategyEx.Strategy.ProcessState == ProcessStates.Stopped) {
                    if (!strategyEx.Strategy.Connector.RegisteredMarketDepths.Contains(strategyEx.Strategy.Security))
                        strategyEx.Strategy.Connector.RegisterMarketDepth(strategyEx.Strategy.Security);
                    strategyEx.Strategy.Start();
                    ((Button)sender).Content = "Stop";
                }
            }
        }


        private void btnLogsWindows_Click(object sender, RoutedEventArgs e) {
            _monitorWindow.ShowOrHide();
        }
        private void GetStreamData_Click(object sender, RoutedEventArgs e) {
            var strategyEx = ((StrategyEx)(StrategiesList.SelectedItem));
            if (strategyEx.Strategy == null) {
                try {
                    strategyEx.CreateStrategy(
                       strategyEx.Properties,
                       _safeConnection.Trader);

                    //strategyEx.Strategy.ProcessStateChanged += InfoSubscribe;
                    strategyEx.Strategy.PropertyChanged += (s, arg) =>
                    {
                        switch (arg.PropertyName) {
                            case "SecurityOne":
                                var id = arg.PropertyName;
                                var security = strategyEx.Strategy.Connector.Securities.FirstOrDefault(sec => sec.Id == id.ToString());

                                if (!strategyEx.Strategy.Connector.RegisteredMarketDepths.Contains(security))
                                    strategyEx.Strategy.Connector.RegisterMarketDepth(security);
                                break;
                            case "SecurityTwo":
                                var id2 = arg.PropertyName;
                                var security2 = strategyEx.Strategy.Connector.Securities.FirstOrDefault(sec => sec.Id == id2.ToString());

                                if (!strategyEx.Strategy.Connector.RegisteredMarketDepths.Contains(security2))
                                    strategyEx.Strategy.Connector.RegisterMarketDepth(security2);
                                break;
                        }
                    };
                    strategyEx.Strategy.Connector.MarketDepthsChanged += OnMarketDepthsChanged;
                } catch (Exception ex) {
                    MessageBox.Show(string.Format("Failed to create a strategy, reason: {0}", ex.Message));
                    return;
                }
                StartGetInformationStrategy(strategyEx.Strategy);
                _logManager.Sources.Add(strategyEx.Strategy);
            }
            if (strategyEx.Strategy.ProcessState == ProcessStates.Started) {
                strategyEx.Strategy.Connector.MarketDepthsChanged -= OnMarketDepthsChanged;
            } else if (strategyEx.Strategy.ProcessState == ProcessStates.Stopped) {
                if (!strategyEx.Strategy.Connector.RegisteredMarketDepths.Contains(strategyEx.Strategy.Security))
                    strategyEx.Strategy.Connector.RegisterMarketDepth(strategyEx.Strategy.Security);
            }
        }
        private void OnMarketDepthsChanged(IEnumerable<MarketDepth> depths) {
            foreach (var depth in depths) {
                if (depth.Security.Type == SecurityTypes.Stock) {
                    stockDepth.UpdateDepth(depth);
                } else if (depth.Security.Type == SecurityTypes.Future) {
                    futureDepth.UpdateDepth(depth);
                }
            }
        } //Подписываюсь на фьюч и акцию
        private void btnStop_Click(object sender, RoutedEventArgs e) {
            _monitorWindow.ShowOrHide();
        }

    }
}



