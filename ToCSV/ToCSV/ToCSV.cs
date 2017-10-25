// -------------------------------------------------------------------------------
//
//    Takes all data from the market and writes each bar to a csv file called data.csv
//		The first line is the name of the indicator 
//
// -------------------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC)]
    public class ToCSV : Robot
    {
        private StreamWriter file;
        private AccumulativeSwingIndex _ASI;
        private Aroon _aroon;
        private BollingerBands _boll;
        private ChaikinVolatility _chaikinVolatility;
        private CommodityChannelIndex _commodityChannelIndex;
        private DetrendedPriceOscillator _slowDetrendedPriceOscillator;
        private DetrendedPriceOscillator _fastDetrendedPriceOscillator;
        private DirectionalMovementSystem _dms;
        private ExponentialMovingAverage _emaFast;
        private ExponentialMovingAverage _emaSlow;
        private FractalChaosBands _fcb;
        //private HighLowBands _hlb;
        private HighMinusLow _hml;
        private HistoricalVolatility _HisVol;
        private LinearRegressionForecast _linRF;
        private LinearRegressionIntercept _linRI;
        private LinearRegressionRSquared _linRRSqr;
        private LinearRegressionSlope _linRS;
        private MacdCrossOver _MACDcross;
        private MacdHistogram _MACDHis;
        private MassIndex _masIn;
        private MedianPrice _medPrice;
        private MomentumOscillator _MomOsc;
        private MovingAverage _movingAv;
        //private MovingAverage _movingAv25;
        private ParabolicSAR _parSAR;
        //private PerformanceIndex _perIndex;
        private PriceOscillator _priceOsc;
        private RainbowOscillator _rainOsc;
        private RelativeStrengthIndex _relStrengthIndex;
        private StandardDeviation _standardDer;
        private StochasticOscillator _stochsticOsc;
        private SwingIndex _swingIndex;
        private Trix _trix;
        private TrueRange _TrueRange;
        private TypicalPrice _TypicalPrice;
        private UltimateOscillator _UltimateOsc;
        private VerticalHorizontalFilter _VHF;
        private Vidya _Vidya;
        private WeightedClose _WightClose;
        private WeightedMovingAverage _WMA;
        private WellesWilderSmoothing _WWilderSmoothing;
        private WilliamsAccumulationDistribution _WAccDis;
        private WilliamsPctR _WPctR;

        //user defined variables
        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter("Slow Periods", DefaultValue = 25, MinValue = 3)]
        public int SlowPeriods { get; set; }

        [Parameter("Fast Periods", DefaultValue = 5, MinValue = 2)]
        public int FastPeriods { get; set; }

        [Parameter("Std", DefaultValue = 14)]
        public int std { get; set; }

        [Parameter("MAType")]
        public MovingAverageType MAType { get; set; }

        [Parameter("Rate of Change", DefaultValue = 10)]
        public int roc { get; set; }

        [Parameter("R", DefaultValue = 0.65)]
        public double R { get; set; }

        [Parameter("Bar History", DefaultValue = 256)]
        public int barHis { get; set; }

        [Parameter("Rainbow Oscillator level", DefaultValue = 9)]
        public int RainOscLevel { get; set; }


        //called when robot is started
        protected override void OnStart()
        {
            // Create new file 
            file = new System.IO.StreamWriter("c:\\data.csv");

            // Write the first line with the names of each variable
            file.WriteLine("Time,OpenP, HighP, LowP, Close Price,Accumulative Swing Index,Aroon Up,Aroon Down,Bollinger Bands Main,Bollinger Bands Top,Bollinger Bands Bottom,Chaikin Volatility,Commodity Channel Index,Detrended Price Oscillator Slow,Detrended Price Oscillator Fast,Directional Movement System ADX,Directional Movement System DIMinus,Directional Movement System DIPlus,Exponential Moving Average Fast,Exponential Moving Average Slow,Fractal Chaos Bands High,Fractal Chaos Bands Low,High Minus Low,Historical Volatility,Linear Regression Forecast,Linear Regression Intercept,Linear Regression R Squared,Linear Regression Slope,Macd Cross Over Signal,Macd Histogram,Macd Signal,Mass Index,Median Price,Momentum Oscillator,Moving Average Slow,Parabolic SAR,Performance Index,Price Oscillator,Rainbow Oscillator,Relative Strength Index,Standard Deviation,Stochastic Oscillator Percent D,Stochastic Oscillator Percent K,SwingIndex,Trix,True Range,Typical Price,Ultimate Oscillator,Vertical Horizontal Filter,Vidya,Weighted Close,Weighted Moving Average,Welles Wilder Smoothing,Williams Accumulation Distribution,Williams Pct R");

            //Setup the variable calls
            _ASI = Indicators.AccumulativeSwingIndex(SlowPeriods);

            _aroon = Indicators.Aroon(SlowPeriods);

            _boll = Indicators.BollingerBands(Source, SlowPeriods, std, MAType);

            _chaikinVolatility = Indicators.ChaikinVolatility(SlowPeriods, roc, MAType);

            _commodityChannelIndex = Indicators.CommodityChannelIndex(SlowPeriods);

            _slowDetrendedPriceOscillator = Indicators.DetrendedPriceOscillator(Source, SlowPeriods, MAType);
            _fastDetrendedPriceOscillator = Indicators.DetrendedPriceOscillator(Source, FastPeriods, MAType);

            _dms = Indicators.DirectionalMovementSystem(SlowPeriods);

            _emaFast = Indicators.ExponentialMovingAverage(Source, FastPeriods);
            _emaSlow = Indicators.ExponentialMovingAverage(Source, SlowPeriods);

            _fcb = Indicators.FractalChaosBands(SlowPeriods);

            //_hlb = Indicators.HighLowBands(SlowPeriods);

            _hml = Indicators.HighMinusLow();

            _standardDer = Indicators.StandardDeviation(Source, SlowPeriods, MAType);


            _HisVol = Indicators.HistoricalVolatility(Source, SlowPeriods, barHis);


            _linRF = Indicators.LinearRegressionForecast(Source, SlowPeriods);

            _linRI = Indicators.LinearRegressionIntercept(Source, SlowPeriods);

            _linRRSqr = Indicators.LinearRegressionRSquared(Source, SlowPeriods);

            _linRS = Indicators.LinearRegressionSlope(Source, SlowPeriods);

            _MACDcross = Indicators.MacdCrossOver(SlowPeriods, FastPeriods, 3);

            _MACDHis = Indicators.MacdHistogram(SlowPeriods, FastPeriods, 3);

            _masIn = Indicators.MassIndex(SlowPeriods);

            _medPrice = Indicators.MedianPrice();

            _MomOsc = Indicators.MomentumOscillator(Source, SlowPeriods);

            _movingAv = Indicators.MovingAverage(Source, SlowPeriods, MAType);
            // _movingAv25 = Indicators.MovingAverage(Source, SlowPeriods, MAType);

            _parSAR = Indicators.ParabolicSAR(FastPeriods, SlowPeriods);

            //_perIndex = Indicators.PerformanceIndex(Source);

            _priceOsc = Indicators.PriceOscillator(Source, SlowPeriods, FastPeriods, MAType);

            _rainOsc = Indicators.RainbowOscillator(Source, RainOscLevel, MAType);

            _relStrengthIndex = Indicators.RelativeStrengthIndex(Source, SlowPeriods);

            _stochsticOsc = Indicators.StochasticOscillator(SlowPeriods, FastPeriods, SlowPeriods, MAType);

            _swingIndex = Indicators.SwingIndex(SlowPeriods);

            _trix = Indicators.Trix(Source, SlowPeriods);

            _TrueRange = Indicators.TrueRange();

            _TypicalPrice = Indicators.TypicalPrice();

            _UltimateOsc = Indicators.UltimateOscillator(FastPeriods, (SlowPeriods - FastPeriods), SlowPeriods);

            _VHF = Indicators.VerticalHorizontalFilter(Source, SlowPeriods);

            _Vidya = Indicators.Vidya(Source, SlowPeriods, R);

            _WightClose = Indicators.WeightedClose();

            _WMA = Indicators.WeightedMovingAverage(Source, SlowPeriods);

            _WWilderSmoothing = Indicators.WellesWilderSmoothing(Source, SlowPeriods);

            _WAccDis = Indicators.WilliamsAccumulationDistribution();

            _WPctR = Indicators.WilliamsPctR(SlowPeriods);
        }

        // called at the end of each time interval 
        protected override void OnBar()
        {
            //crete a string builder to build up each line of the csv file
            StringBuilder tmp = new StringBuilder();
            int currBar = MarketSeries.Close.Count - 2;
            //Add each of the indicators to the string builder
            tmp.Append(MarketSeries.OpenTime.LastValue);
            tmp.Append(",");
            tmp.Append(MarketSeries.Open[currBar]);
            tmp.Append(",");
            tmp.Append(MarketSeries.High[currBar]);
            tmp.Append(",");
            tmp.Append(MarketSeries.Low[currBar]);
            tmp.Append(",");
            tmp.Append(MarketSeries.Close[currBar]);
            tmp.Append(",");
            tmp.Append(_ASI.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_aroon.Up.LastValue);
            tmp.Append(",");
            tmp.Append(_aroon.Down.LastValue);
            tmp.Append(",");
            tmp.Append(_boll.Main.LastValue);
            tmp.Append(",");
            tmp.Append(_boll.Top.LastValue);
            tmp.Append(",");
            tmp.Append(_boll.Bottom.LastValue);
            tmp.Append(",");
            tmp.Append(_chaikinVolatility.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_commodityChannelIndex.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_slowDetrendedPriceOscillator.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_fastDetrendedPriceOscillator.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_dms.ADX.LastValue);
            tmp.Append(",");
            tmp.Append(_dms.DIMinus.LastValue);
            tmp.Append(",");
            tmp.Append(_dms.DIPlus.LastValue);
            tmp.Append(",");
            tmp.Append(_emaFast.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_emaSlow.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_fcb.High.LastValue);
            tmp.Append(",");
            tmp.Append(_fcb.Low.LastValue);
            tmp.Append(",");
            // tmp.Append(_hlb.Median.LastValue);
            // tmp.Append(",");
            //tmp.Append(_hlb.Top.LastValue);
            //tmp.Append(",");
            //tmp.Append(_hlb.Bottom.LastValue);
            //tmp.Append(",");
            tmp.Append(_hml.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_HisVol.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_linRF.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_linRI.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_linRRSqr.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_linRS.Result.LastValue);
            tmp.Append(",");

            tmp.Append(_MACDcross.MACD.LastValue);
            tmp.Append(",");
            tmp.Append(_MACDcross.Histogram.LastValue);
            tmp.Append(",");
            tmp.Append(_MACDcross.Signal.LastValue);
            tmp.Append(",");
            //tmp.Append(_MACDHis.Histogram.LastValue);
            //tmp.Append(",");
            tmp.Append(_MACDHis.Signal.LastValue);
            tmp.Append(",");
            tmp.Append(_masIn.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_medPrice.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_MomOsc.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_movingAv.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_parSAR.Result.LastValue);
            tmp.Append(",");
            //tmp.Append(_perIndex.Result.LastValue);
            //tmp.Append(",");
            tmp.Append(_priceOsc.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_rainOsc.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_relStrengthIndex.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_standardDer.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_stochsticOsc.PercentD.LastValue);
            tmp.Append(",");
            tmp.Append(_stochsticOsc.PercentK.LastValue);
            tmp.Append(",");
            tmp.Append(_swingIndex.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_trix.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_TrueRange.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_TypicalPrice.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_UltimateOsc.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_VHF.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_Vidya.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_WightClose.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_WMA.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_WWilderSmoothing.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_WAccDis.Result.LastValue);
            tmp.Append(",");
            tmp.Append(_WPctR.Result.LastValue);
            tmp.Append(",");

            //write the sring builder to the file
            file.WriteLine(tmp.ToString());
        }

        // called when the robot stops 
        protected override void OnStop()
        {
            //close the file 
            file.Close();
        }
    }
}
