using System.Device.Gpio;
using System.Linq;

namespace LedBarTest.Utils
{
    public class LedProgressBar
    {
        private readonly GpioController controller;
        private readonly int data;
        private readonly int clock;
        private readonly int latch;

        public LedProgressBar(GpioController controller, int data, int clock, int latch)
        {
            this.controller = controller;
            this.data = data;
            this.clock = clock;
            this.latch = latch;

            controller.OpenPin(data, PinMode.Output);
            controller.OpenPin(clock, PinMode.Output);
            controller.OpenPin(latch, PinMode.Output);
        }

        public void Write(byte value)
        {
            controller.Write(latch, PinValue.Low);

            var bits = Enumerable.Range(0, 8)
                .Select(n => GetBit(value, n))
                .ToList();

            bits.ForEach(i => SerialWrite(i));

            controller.Write(latch, PinValue.High);

        }

        private static int GetBit(byte value, int position)
        {
            return (value & (1 << position)) >> position;
        }

        private void SerialWrite(PinValue value)
        {
            controller.Write(clock, PinValue.Low);
            controller.Write(data, value);
            controller.Write(clock, PinValue.High);
        }
    }
}