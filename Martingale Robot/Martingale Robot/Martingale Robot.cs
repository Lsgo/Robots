// -------------------------------------------------------------------------------------------------
//
//    This code is a cAlgo API sample.
//
//    This robot is intended to be used as a sample and does not guarantee any particular outcome or
//    profit of any kind. Use it at your own risk
//
//    All changes to this file will be lost on next application start.
//    If you are going to modify this file please make a copy using the "Duplicate" command.
//
//    The "Sample Martingale Robot" creates a random Sell or Buy order. If the Stop loss is hit, a new 
//    order of the same type (Buy / Sell) is created with double the Initial Volume amount. The robot will 
//    continue to double the volume amount for  all orders created until one of them hits the take Profit. 
//    After a Take Profit is hit, a new random Buy or Sell order is created with the Initial Volume amount.
//
// -------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class MartingaleRobot : Robot
    {
        [Parameter("Initial Volume", DefaultValue = 10000, MinValue = 0)]
        public int InitialVolume { get; set; }

        [Parameter("Stop Loss", DefaultValue = 40)]
        public int StopLoss { get; set; }

        [Parameter("Take Profit", DefaultValue = 40)]
        public int TakeProfit { get; set; }


        [Parameter("Martingale Multiplier", DefaultValue = 2.0)]
        public double Multiplier { get; set; }

        public int loss_count = 1;

        private Random random = new Random();

        protected override void OnStart()
        {
            Positions.Closed += OnPositionsClosed;

            ExecuteOrder(InitialVolume, GetRandomTradeType());
        }

        private void ExecuteOrder(long volume, TradeType tradeType)
        {
            var result = ExecuteMarketOrder(tradeType, Symbol, volume, "Martingale", StopLoss, TakeProfit);

            if (result.Error == ErrorCode.NoMoney)
                Stop();
        }

        private void OnPositionsClosed(PositionClosedEventArgs args)
        {
            Print("Closed");
            var position = args.Position;

            if (position.Label != "Martingale" || position.SymbolCode != Symbol.Code)
                return;

            if (position.GrossProfit > 0)
            {
                loss_count = 1;
                ExecuteOrder(InitialVolume, GetRandomTradeType());
            }
            else
            {
                //如果连输4次转换买卖方向
                loss_count = loss_count + 1;
                if (loss_count == 4)
                {
                    if (position.TradeType == TradeType.Buy)
                    {
                        ExecuteOrder((int)(position.Volume * Multiplier), TradeType.Sell);
                    }
                    else
                    {
                        ExecuteOrder((int)(position.Volume * Multiplier), TradeType.Buy);
                    }
                }
                else
                {
                    ExecuteOrder((int)(position.Volume * Multiplier), position.TradeType);
                }
            }
        }

        private TradeType GetRandomTradeType()
        {
            return random.Next(2) == 0 ? TradeType.Buy : TradeType.Sell;
        }
    }
}
