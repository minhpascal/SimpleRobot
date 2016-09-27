using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommonRobot;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace SimpleRobot
{
      [DisplayName(@"Настройки SimpleRobot")]
        public class StrategyParam : INotifyPropertyChanged
        {
            private string secFuture = @"SRU6@FORTS"; //инстурмент1
            private string secStock = @"SBER@TQBR"; //инстурмент 2
            private string portFt; //портфель для фьюча
            private string portSt; //портфель для акции
            private int secFutureVolume = 1; //объем инстурмента 1
            private int secStockVolume = 10; //объем инструмента 2

            private int _div = 2; //диреватив
            private int _t = 90; //кол-во
            private int _spread = 8; //спред
            private int _offset = 8; //оффсет
            private int _stop = 360; //cтоп


            /// <summary>
            /// Минимальный размер спреда в шагах цены для выставления ордеров
            /// </summary>

            [DisplayName(@"Спред")]
            [Description(@"Минимальный размер спреда в шагах цены для выставления ордеров")]
            [Category(@"Параметры")]
            //[CategoryOrder(1)]
            [PropertyOrder(0)]
            public int Spread
            {
                get { return _spread; }
                set
                {
                    _spread = value;
                    OnPropertyChanged("Spread");
                }
            }
            /// <summary>
            /// Количество дней до погашения (дни)
            /// </summary>
            [DisplayName(@"Time experation")]
            [Description(@"Количество дней до погашения (дни)")]
            [Category(@"Параметры")]
            //[CategoryOrder(1)]
            [PropertyOrder(0)]
            public int TimeExp
            {
                get { return _t; }
                set
                {
                    _t = value;
                    OnPropertyChanged("Time experation");
                }
            }
            /// <summary>
            /// Дериватив
            /// </summary>
            [DisplayName(@"DIRV")]
            [Description(@"Параметр дериватива")]
            [Category(@"Параметры")]
            //[CategoryOrder(1)]
            [PropertyOrder(0)]
            public int DIRV
            {
                get { return _div; }
                set
                {
                    _div = value;
                    OnPropertyChanged("DIRV");
                }
            }
            /// <summary>
            /// Отступ в шагах от цены
            /// </summary>

            [DisplayName(@"Отступ")]
            [Description(@"Отступ в шагах цены от края стакана для ордеров. > 0 - вдаль от спреда, < 0 - вгубь спреда")]
            [Category(@"Параметры")]
            //[CategoryOrder(1)]
            [PropertyOrder(0)]
            public int Offset
            {
                get { return _offset; }
                set
                {
                    _offset = value;
                    OnPropertyChanged("Offset");
                }
            }
            /// <summary>
            /// Сколько держать позицию
            /// </summary>
            [DisplayName(@"Стоп (сек)")]
            [Description(@"Стоп в секундах сколько держать открытой позицию")]
            [Category(@"Параметры")]
            //[CategoryOrder(1)]
            [PropertyOrder(0)]
            public int Stop
            {
                get { return _stop; }
                set
                {
                    _stop = value;
                    OnPropertyChanged("Stop");
                }
            }
            /// <summary>
            /// Счет для фьюча
            /// </summary>
            [DisplayName(@"Счет для фьюча")]
            [Description(@"Выберите торговый счет для фьючерса.")]
            [Category(@"Параметры")]
            [PropertyOrder(0)]
            [Editor(typeof(PortfolioNameEditor), typeof(PortfolioNameEditor))]
            public string PortfolioForFuture
            {
                get { return portFt; }

                set
                {
                    if (value == portFt)
                        return;
                    portFt = value;
                    OnPropertyChanged("Portfolio For Future");
                }
            }
            /// <summary>
            /// Счет для акции
            /// </summary>
            [DisplayName(@"Счет для акции")]
            [Description(@"Выберите торговый счет для ценной бумаги.")]
            [Category(@"Параметры")]
            [PropertyOrder(0)]
            [Editor(typeof(PortfolioNameEditor), typeof(PortfolioNameEditor))]
            public string PortfolioForStock
            {
                get { return portSt; }

                set
                {
                    if (value == portSt)
                        return;
                    portSt = value;
                    OnPropertyChanged("Portfolio For Stock");
                }
            }
            /// <summary>
            /// Фьючерс
            /// </summary>
            [DisplayName(@"Контракт")]
            [Description(@"Фьючерсный контракт.")]
            [Category(@"Параметры")]
            [PropertyOrder(0)]
            [Editor(typeof(SecurityIdEditor), typeof(SecurityIdEditor))]
            public string SecFuture
            {
                get { return secFuture; }
                set
                {
                    if (value == secFuture)
                        return;
                    secFuture = value;
                    OnPropertyChanged("Future");
                }
            }
            /// <summary>
            /// Объем фьюча
            /// </summary>
            [DisplayName(@"Объем контракта")]
            [Description(@"Объем фьючерсного контракта.")]
            [Category(@"Параметры")]
            [PropertyOrder(0)]
            public int SecFutureVolume
            {
                get { return secFutureVolume; }
                set
                {
                    if (value == secFutureVolume)
                        return;

                    secFutureVolume = value;
                    OnPropertyChanged("SecurityFutureVolume");
                }
            }
            /// <summary>
            /// Акция
            /// </summary>
            [DisplayName(@"Ценная бумага")]
            [Description(@"Выберите ценную бумагу")]
            [Category(@"Параметры")]
            [PropertyOrder(0)]
            [Editor(typeof(SecurityIdEditor), typeof(SecurityIdEditor))]
            public string SecStock
            {
                get { return secStock; }
                set
                {
                    if (value == secStock)
                        return;

                    secStock = value;
                    OnPropertyChanged("Stock");
                }
            }

            /// <summary>
            /// Объем акции
            /// </summary>
            [DisplayName(@"Лот ценной бумаги")]
            [Description(@"Объем ценной бумаги.")]
            [Category(@"Параметры")]
            [PropertyOrder(0)]
            public int SecStockVolume
            {
                get { return secStockVolume; }
                set
                {
                    if (value == secStockVolume)
                        return;

                    secStockVolume = value;
                    OnPropertyChanged("SecurityStockVolume");
                }
            }


            //====================================================================================

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

