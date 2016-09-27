#region Сборка StockSharp.Algo.Strategies, Version=4.3.14.0, Culture=neutral, PublicKeyToken=null
// C:\Users\neonn\Desktop\EDU\StockSharp.Edu\References\StockSharp.Algo.Strategies.dll
#endregion

using StockSharp.Messages;

namespace StockSharp.Algo.Strategies.Quoting
{
    //
    // Сводка:
    //     /// The strategy realizing volume quoting algorithm by the limited price. ///
    public  class LimitQuotingStrategy : QuotingStrategy
    {
        //
        // Сводка:
        //     /// Initializes a new instance of the StockSharp.Algo.Strategies.Quoting.LimitQuotingStrategy.
        //     ///
        //
        // Параметры:
        //   quotingDirection:
        //     Quoting direction.
        //
        //   quotingVolume:
        //     Total quoting volume.
        //
        //   limitPrice:
        //     The limited price for quoted orders.
        public extern LimitQuotingStrategy(Sides quotingDirection, decimal quotingVolume, decimal limitPrice);

        //
        // Сводка:
        //     /// The limited price for quoted orders. ///
        public decimal LimitPrice { get; set; }
        //
        // Сводка:
        //     /// To get the best price. If it is impossible to calculate the best price at
        //     the moment, then null will be returned. ///
        protected override decimal? BestPrice { get; }
    }
}