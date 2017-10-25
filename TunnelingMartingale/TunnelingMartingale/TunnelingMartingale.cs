//--------------------------------------------------------------------
//                                              Tunneling Martingale |
//                                       Made for the CTDN Community |
//                                       Contact: Waxavi@outlook.com |
//--------------------------------------------------------------------


using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class TunnelingMartingale : Robot
    {
        [Parameter("Long Entry Price", DefaultValue = 1.0)]
        public double _LongEntry { get; set; }
        [Parameter("Short Entry Price", DefaultValue = 1.0)]
        public double _ShortEntry { get; set; }
        [Parameter("Lot Size", DefaultValue = 0.01)]
        public double _LotSize { get; set; }
        [Parameter("$ Target", DefaultValue = 50)]
        public double _MoneyTarget { get; set; }
        [Parameter("Max attemps", DefaultValue = 5)]
        public int _MaxAttemps { get; set; }

        string _Label = "label";
        TradeType _Buy = TradeType.Buy;
        TradeType _Sell = TradeType.Sell;

        double _NetProfit = 0;
        int _CurrentAttemps = 1;

        private void PositionsOnOpened(PositionOpenedEventArgs args)
        {
            if (args.Position.Label == _Label)
            {
                Print("Current Attempt: {0}", _CurrentAttemps);
                var _pending = PendingOrders.Where(item => item.Label == _Label);
                if (_pending.Count() != 0)
                    CancelPendingOrder(_pending.First());
            }
        }

        private void PositionsOnClosed(PositionClosedEventArgs args)
        {
            if (args.Position.NetProfit + _NetProfit >= _MoneyTarget)
                return;

            _NetProfit += args.Position.NetProfit;
            _CurrentAttemps++;

            if (_CurrentAttemps > _MaxAttemps)
                Stop();
            else
            {
                TradeType _tt = args.Position.TradeType == _Buy ? _Sell : _Buy;

                if (args.Position.TradeType == _Buy)
                {
                    ExecuteMarketOrder(_Sell, Symbol, Symbol.QuantityToVolume(_LotSize * 2), _Label, (_LongEntry - _ShortEntry) / Symbol.PipSize, null);
                }
                else
                {
                    ExecuteMarketOrder(_Buy, Symbol, Symbol.QuantityToVolume(_LotSize * 2), _Label, (_LongEntry - _ShortEntry) / Symbol.PipSize, null);
                }

            }

        }

        //Get a custom label for the trades of this instance
        protected string GetLabel(string _TType)
        {
            int x = Server.Time.Millisecond;
            bool quit = false;
            _Label = _TType + x.ToString() + this.TimeFrame.ToString();

            var _CLD = History.FindLast(_Label);
            var _CLO = Positions.Find(_Label);
            while (_CLD != null || _CLO != null)
            {
                x++;
                _Label = _TType + x.ToString();
                _CLD = History.FindLast(_Label);
                _CLO = Positions.Find(_Label);
                Print("Theres a duplicated Label, finding another one");
            }

            if (PendingOrders.Count != 0)
            {
                while (true)
                {
                    foreach (var pen in PendingOrders)
                    {
                        if (pen.Label == _Label)
                        {
                            _Label = _TType + x.ToString();
                            Print("Theres a duplicated Label, finding another one");
                            break;
                        }
                        else if (pen == PendingOrders.Last())
                        {
                            quit = true;
                        }
                    }

                    if (quit)
                    {
                        break;
                    }
                }
            }

            return _Label;
        }


        protected override void OnStart()
        {
            Positions.Opened += PositionsOnOpened;
            Positions.Closed += PositionsOnClosed;

            if (_LongEntry < _ShortEntry)
                Stop();

            if (Symbol.Ask > _LongEntry || Symbol.Bid < _ShortEntry)
                Stop();

            PlaceStopOrder(_Buy, Symbol, Symbol.QuantityToVolume(_LotSize), _LongEntry, _Label, (_LongEntry - _ShortEntry) / Symbol.PipSize, null, null);
            PlaceStopOrder(_Sell, Symbol, Symbol.QuantityToVolume(_LotSize), _ShortEntry, _Label, (_LongEntry - _ShortEntry) / Symbol.PipSize, null, null);
        }

        protected override void OnTick()
        {
            var _pos = Positions.Find(_Label);
            if (_pos != null)
            {
                if (_pos.NetProfit + _NetProfit >= _MoneyTarget)
                {
                    ClosePosition(_pos);
                    Stop();
                }
            }
        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
    }
}
