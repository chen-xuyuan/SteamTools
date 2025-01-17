using Avalonia.Controls.Primitives;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using Avalonia.Styling;
using FluentAvalonia.Styling;
using ReactiveUI;
using System;
using System.Application.Models.Settings;
using System.Application.Services;
using System.Application.UI.ViewModels;

// ReSharper disable once CheckNamespace
namespace Avalonia.Controls
{
    public abstract class FluentWindow<TViewModel> : ReactiveWindow<TViewModel>, IStyleable where TViewModel : class
    {
        Type IStyleable.StyleKey => typeof(Window);

        public FluentWindow(bool isSaveStatus = true)
        {
            //if (OperatingSystem2.IsWindows)
            //{
            ExtendClientAreaToDecorationsHint = true;
            ExtendClientAreaTitleBarHeightHint = -1;

            //}
            TransparencyLevelHint = WindowTransparencyLevel.AcrylicBlur;
            SystemDecorations = SystemDecorations.Full;

            this.GetObservable(WindowStateProperty)
            .Subscribe(x =>
            {
                PseudoClasses.Set(":maximized", x == WindowState.Maximized);
                PseudoClasses.Set(":fullscreen", x == WindowState.FullScreen);
            });

            //this.GetObservable(IsExtendedIntoWindowDecorationsProperty)
            //    .Subscribe(x =>
            //    {
            //        SystemDecorations = SystemDecorations.Full;
            //        //TransparencyLevelHint = WindowTransparencyLevel.AcrylicBlur;
            //    });

            if (isSaveStatus)
            {
                Opened += FluentWindow_Opened;
                if (CanResize)
                    PositionChanged += FluentWindow_PositionChanged;
            }

            if (!ViewModelBase.IsInDesignMode)
            {
                if (OperatingSystem2.IsWindows7)
                {
                    IDesktopPlatformService.Instance.FixFluentWindowStyleOnWin7(PlatformImpl.Handle.Handle);
                }
                else if (OperatingSystem2.IsWindows10AtLeast)
                {
                    AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>().ForceNativeTitleBarToTheme(this);
                }
            }
        }

        private void FluentWindow_PositionChanged(object? sender, PixelPointEventArgs e)
        {
            if (_isOpenWindow == false)
                return;
            if (DataContext is WindowViewModel vm)
            {
                vm.SizePosition.X = e.Point.X;
                vm.SizePosition.Y = e.Point.Y;
            }
        }

        protected bool _isOpenWindow;
        protected virtual void FluentWindow_Opened(object? sender, EventArgs e)
        {
            _isOpenWindow = true;
            if (DataContext is WindowViewModel vm)
            {
                if (vm.SizePosition.X > 0 && vm.SizePosition.Y > 0)
                {
                    var point = new PixelPoint(vm.SizePosition.X, vm.SizePosition.Y);
                    if (Screens.Primary.WorkingArea.Contains(point))
                        Position = point;
                }

                if (vm.SizePosition.Width > 0
                    && Screens.Primary.WorkingArea.Width >= vm.SizePosition.Width)
                    Width = vm.SizePosition.Width;

                if (vm.SizePosition.Height > 0
                    && Screens.Primary.WorkingArea.Height >= vm.SizePosition.Height)
                    Height = vm.SizePosition.Height;

                HandleResized(new Size(Width, Height), PlatformResizeReason.Application);

                this.WhenAnyValue(x => x.ClientSize)
                    .Subscribe(x =>
                    {
                        vm.SizePosition.Width = x.Width;
                        vm.SizePosition.Height = x.Height;
                    });

                //this.GetObservable(WidthProperty).Subscribe(v =>
                //{
                //    vm.SizePosition.Width = v;
                //});
                //this.GetObservable(HeightProperty).Subscribe(v =>
                //{
                //    vm.SizePosition.Height = v;
                //});
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            if (OperatingSystem2.IsWindows)
            {
                ExtendClientAreaChromeHints =
                    ExtendClientAreaChromeHints.PreferSystemChrome;
            }
            else if (OperatingSystem2.IsMacOS)
            {
                ExtendClientAreaChromeHints =
                    ExtendClientAreaChromeHints.PreferSystemChrome;
            }
            else
            {
                ExtendClientAreaChromeHints =
                    ExtendClientAreaChromeHints.SystemChrome;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (DataContext is IDisposable disposable) disposable.Dispose();
        }
    }

    public abstract class FluentWindow : FluentWindow<WindowViewModel>
    {
        public FluentWindow(bool isSaveStatus = true) : base(isSaveStatus)
        {

        }
    }
}