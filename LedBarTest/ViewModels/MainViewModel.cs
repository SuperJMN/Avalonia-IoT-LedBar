using System;
using System.Collections;
using System.Device.Gpio;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using LedBarTest.Utils;
using ReactiveUI;

namespace LedBarTest.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        // GPIO PIN CONFIGURATION
        private const int Data = 17;
        private const int Clock = 22;
        private const int Latch = 27;

        private readonly LedProgressBar progressBar;
        private IDisposable animation;

        public MainViewModel()
        {
            var controller = new GpioController();
            progressBar = new LedProgressBar(controller, Data, Clock, Latch);

            Animate = ReactiveCommand.Create(ToggleAnimation);
            Dispose = ReactiveCommand.Create(() =>
            {
                controller.Dispose();
                animation?.Dispose();
            });
        }

        public string Greeting => "Avalonia loves IoT";

        public ReactiveCommand<Unit, Unit> Dispose { get; }

        public ReactiveCommand<Unit, Unit> Animate { get; }

        private void ToggleAnimation()
        {
            if (animation != null)
            {
                animation.Dispose();
                animation = null;
                return;
            }

            animation = CreateAnimation();
        }

        private IDisposable CreateAnimation()
        {
            var leftToRight = Enumerable.Range(0, 8)
                .Select(i => new BitArray(8, false) {[i] = true}).ToList();

            var frames = leftToRight.Concat(Enumerable.Reverse(leftToRight));

            return frames
                .ToObservable()
                .Select(bits => Observable.Return(bits).Delay(TimeSpan.FromMilliseconds(100)))
                .Concat()
                .Repeat()
                .Subscribe(bits => progressBar.Write(bits.ToByte()));
        }
    }
}