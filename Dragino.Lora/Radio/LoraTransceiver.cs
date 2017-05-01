using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;
using Dragino.Radio.Transceivers;

namespace Dragino.Radio
{
    internal class LoraTransceiver : Transceiver
    {
        public static void ValidateSettings(TransceiverSettings settings)
        {
            // In LoRa mode, only bandwidth 125, 250 & 500 is allowed
            if ((settings.BandWidth != BandWidth.BandWidth_125_00_kHz) &&
                (settings.BandWidth != BandWidth.BandWidth_250_00_kHz) &&
                (settings.BandWidth != BandWidth.BandWidth_500_00_kHz))
            {
                throw new ArgumentException("In LoRa mode, only bandwidth 125 kHz, 250 kHz or 500 kHz is allowed");
            }
        }

        public LoraTransceiver(SpiDevice spiDevice, TransceiverSettings settings, TransceiverPinSettings pinSettings)
            : base(spiDevice, settings, pinSettings)
        {
        }

        protected override async Task Init()
        {
            Pins.Reset.Write(GpioPinValue.Low);
            await Task.Delay(10);

            Pins.Reset.Write(GpioPinValue.High);
            await Task.Delay(10);

            CommonRegisterVersion version = RegisterManager.Read<CommonRegisterVersion>();
            if (version.Value == 0x12)
            {
                Debug.WriteLine("SX1276/77/78/79 detected, starting.");
            }
            else
            {
                throw new Exception("Unrecognized transceiver.");
            }

            RegisterManager.Write(new CommonRegisterOpMode(
                CommonRegisterOpMode.LongRangeModeEnum.LoRaMode,
                CommonRegisterOpMode.AccessSharedRegEnum.AccessLoRaRegisterPage,
                CommonRegisterOpMode.LowFrequencyModeOnEnum.HighFrequencyMode,
                CommonRegisterOpMode.DeviceModeEnum.Sleep));

            uint frf = (uint)(((ulong)Settings.Frequency << 19) / 32000000);
            RegisterManager.Write(new CommonRegisterFrf(frf));

            RegisterManager.Write(new LoraRegisterSyncWord(Settings.LoraSyncWord.Value));

            RegisterManager.Write(new LoraRegisterModemConfig1(
                ConvertBandWidth(Settings.BandWidth),
                ConvertCodingRate(Settings.CodingRate),
                LoraRegisterModemConfig1.HeaderModeEnum.ExplicitHeaderMode));

            RegisterManager.Write(new LoraRegisterModemConfig2(
                ConvertSpreadingFactor(Settings.SpreadingFactor),
                LoraRegisterModemConfig2.TxModeEnum.NormalMode,
                Settings.CrcEnabled ? LoraRegisterModemConfig2.RxPayloadCrcEnum.CrcEnabled : LoraRegisterModemConfig2.RxPayloadCrcEnum.CrcDisabled,
                Settings.SymbolTimeout));

            RegisterManager.Write(new LoraRegisterModemConfig3(
                Settings.EnableLowDataRateOptimize ? LoraRegisterModemConfig3.LowDataRateOptimizeEnum.Enabled : LoraRegisterModemConfig3.LowDataRateOptimizeEnum.Disabled,
                LoraRegisterModemConfig3.AgcAutoEnum.Enabled));

            RegisterManager.Write(new LoraRegisterMaxPayloadLength(0x80));
            RegisterManager.Write(new LoraRegisterPayloadLength(0x40));
            RegisterManager.Write(new LoraRegisterHopPeriod(0xFF));

            RegisterManager.Write(new LoraRegisterFifoRxBaseAddr(0x00));
            RegisterManager.Write(new LoraRegisterFifoTxBaseAddr(0x80));

            byte fifoRxBaseAddr = RegisterManager.Read<LoraRegisterFifoRxBaseAddr>().FifoRxBaseAddr;
            RegisterManager.Write(new LoraRegisterFifoAddrPtr(fifoRxBaseAddr));

            RegisterManager.Write(new CommonRegisterLna(
                CommonRegisterLna.LnaGainEnum.G1Max,
                CommonRegisterLna.LnaBoostLfEnum.DefaultLnaCurrent,
                CommonRegisterLna.LnaBoostHfEnum.BoostOn));

            RegisterManager.Write(new CommonRegisterOpMode(
                CommonRegisterOpMode.LongRangeModeEnum.LoRaMode,
                CommonRegisterOpMode.AccessSharedRegEnum.AccessLoRaRegisterPage,
                CommonRegisterOpMode.LowFrequencyModeOnEnum.HighFrequencyMode,
                CommonRegisterOpMode.DeviceModeEnum.ReceiveContinuous));
        }

        private static LoraRegisterModemConfig1.BandWidthEnum ConvertBandWidth(BandWidth bandWidth)
        {
            switch (bandWidth)
            {
                case BandWidth.BandWidth_007_80_kHz:
                    return LoraRegisterModemConfig1.BandWidthEnum.BandWidth_007_80_kHz;
                case BandWidth.BandWidth_010_40_kHz:
                    return LoraRegisterModemConfig1.BandWidthEnum.BandWidth_010_40_kHz;
                case BandWidth.BandWidth_015_60_kHz:
                    return LoraRegisterModemConfig1.BandWidthEnum.BandWidth_015_60_kHz;
                case BandWidth.BandWidth_020_80_kHz:
                    return LoraRegisterModemConfig1.BandWidthEnum.BandWidth_020_80_kHz;
                case BandWidth.BandWidth_031_25_kHz:
                    return LoraRegisterModemConfig1.BandWidthEnum.BandWidth_031_25_kHz;
                case BandWidth.BandWidth_041_70_kHz:
                    return LoraRegisterModemConfig1.BandWidthEnum.BandWidth_041_70_kHz;
                case BandWidth.BandWidth_062_50_kHz:
                    return LoraRegisterModemConfig1.BandWidthEnum.BandWidth_062_50_kHz;
                case BandWidth.BandWidth_125_00_kHz:
                    return LoraRegisterModemConfig1.BandWidthEnum.BandWidth_125_00_kHz;
                case BandWidth.BandWidth_250_00_kHz:
                    return LoraRegisterModemConfig1.BandWidthEnum.BandWidth_250_00_kHz;
                case BandWidth.BandWidth_500_00_kHz:
                    return LoraRegisterModemConfig1.BandWidthEnum.BandWidth_500_00_kHz;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bandWidth), bandWidth, null);
            }
        }

        private LoraRegisterModemConfig1.CodingRateEnum ConvertCodingRate(CodingRate codingRate)
        {
            switch (codingRate)
            {
                case CodingRate.FourOfFive:
                    return LoraRegisterModemConfig1.CodingRateEnum.Rate1;
                case CodingRate.FourOfSix:
                    return LoraRegisterModemConfig1.CodingRateEnum.Rate2;
                case CodingRate.FourOfSeven:
                    return LoraRegisterModemConfig1.CodingRateEnum.Rate3;
                case CodingRate.FourOfEight:
                    return LoraRegisterModemConfig1.CodingRateEnum.Rate4;
                default:
                    throw new ArgumentOutOfRangeException(nameof(codingRate), codingRate, null);
            }
        }

        private static LoraRegisterModemConfig2.SpreadingFactorEnum ConvertSpreadingFactor(SpreadingFactor spreadingFactor)
        {
            switch (spreadingFactor)
            {
                case SpreadingFactor.SF7:
                    return LoraRegisterModemConfig2.SpreadingFactorEnum.Sf_0128_ChipsPerSymbol;
                case SpreadingFactor.SF8:
                    return LoraRegisterModemConfig2.SpreadingFactorEnum.Sf_0256_ChipsPerSymbol;
                case SpreadingFactor.SF9:
                    return LoraRegisterModemConfig2.SpreadingFactorEnum.Sf_0512_ChipsPerSymbol;
                case SpreadingFactor.SF10:
                    return LoraRegisterModemConfig2.SpreadingFactorEnum.Sf_1024_ChipsPerSymbol;
                case SpreadingFactor.SF11:
                    return LoraRegisterModemConfig2.SpreadingFactorEnum.Sf_2048_ChipsPerSymbol;
                case SpreadingFactor.SF12:
                    return LoraRegisterModemConfig2.SpreadingFactorEnum.Sf_4096_ChipsPerSymbol;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}