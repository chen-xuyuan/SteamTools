using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using System.Application.Security;

namespace System.Application.UI.Activities
{
    /// <summary>
    /// 页面 - 启动屏幕
    /// </summary>
    [Register(JavaPackageConstants.Activities + nameof(SplashActivity))]
    [Activity(Theme = ManifestConstants.MainTheme_Splash,
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTask,
        ConfigurationChanges = ManifestConstants.ConfigurationChanges,
        NoHistory = true)]
    internal sealed class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //if (!DeviceSecurityCheckUtil.IsAllowStart(this)) return;
            this.StartActivity<MainActivity>();
            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
        }

        public override void OnBackPressed()
        {
            GoToPlatformPages.MockHomePressed(this);
            return;
        }
    }
}