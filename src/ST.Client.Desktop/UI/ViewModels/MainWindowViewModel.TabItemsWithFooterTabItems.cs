using System.Collections.Generic;
using System.Linq;

namespace System.Application.UI.ViewModels
{
    partial class MainWindowViewModel
    {
        readonly Dictionary<Type, Lazy<TabItemViewModel>> mTabItems = new();
        public IEnumerable<TabItemViewModel> TabItems => mTabItems.Values.Select(x => x.Value);

        public IReadOnlyList<TabItemViewModel> FooterTabItems { get; private set; }

        IReadOnlyList<TabItemViewModel> InitTabItemsWithReturnFooterTabItems()
        {
            //AddTabItem<StartPageViewModel>();
            AddTabItem<CommunityProxyPageViewModel>();
            AddTabItem<ProxyScriptManagePageViewModel>();
            AddTabItem<SteamAccountPageViewModel>();
            AddTabItem<GameListPageViewModel>();
            AddTabItem<LocalAuthPageViewModel>();
            var isVersion_2_5_OR_GREATER =
#if DEBUG
                true;
#else
                new Version(System.Properties.ThisAssembly.Version) >= new Version(2, 5);
#endif

            if (isVersion_2_5_OR_GREATER)
            {
                AddTabItem<ArchiSteamFarmPlusPageViewModel>();
            }

            //AddTabItem<SteamIdlePageViewModel>();
#if !TRAY_INDEPENDENT_PROGRAM
            if (OperatingSystem2.IsWindows)
                AddTabItem<GameRelatedPageViewModel>();
#endif
            //AddTabItem<OtherPlatformPageViewModel>();

#if !TRAY_INDEPENDENT_PROGRAM
            if (AppHelper.EnableDevtools)
            {
                AddTabItem<DebugPageViewModel>();
                //FooterTabItems.Add(new DebugPageViewModel().AddTo(this));

                //if (AppHelper.IsSystemWebViewAvailable)
                //{
                //    AddTabItem<DebugWebViewPageViewModel>();
                //}
            }
#endif

            var footerTabItems = new List<TabItemViewModel>
            {
                SettingsPageViewModel.Instance,
                AboutPageViewModel.Instance,
            };
            return footerTabItems;
        }

        void AddTabItem<TabItemVM>() where TabItemVM : TabItemViewModel, new()
        {
            Lazy<TabItemViewModel> value = new(() => new TabItemVM()
#if !TRAY_INDEPENDENT_PROGRAM
            .AddTo(this)
#endif
            );
            mTabItems.Add(typeof(TabItemVM), value);
        }

        //void AddTabItem<TabItemVM>(Func<TabItemVM> func) where TabItemVM : TabItemViewModel
        //{
        //    Lazy<TabItemViewModel> value = new(func);
        //    mTabItems.Add(typeof(TabItemVM), value);
        //}

        TabItemVM GetTabItemVM<TabItemVM>() where TabItemVM : TabItemViewModel => (TabItemVM)mTabItems[typeof(TabItemVM)].Value;
    }
}
