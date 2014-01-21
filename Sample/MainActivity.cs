using Android.App;
using Android.Content;
using Android.Views.Animations;
using Android.Widget;
using Android.OS;
using SmoothProgressBarSharp;

namespace Sample
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ProgressBar _progressBar1;
        private ProgressBar _progressBar2;
        private ProgressBar _progressBar3;
        private ProgressBar _progressBar4;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_main);

            _progressBar1 = FindViewById<ProgressBar>(Resource.Id.progressbar1);
            _progressBar2 = FindViewById<ProgressBar>(Resource.Id.progressbar2);
            _progressBar3 = FindViewById<ProgressBar>(Resource.Id.progressbar3);
            _progressBar4 = FindViewById<ProgressBar>(Resource.Id.progressbar4);

            _progressBar1.IndeterminateDrawable = new SmoothProgressDrawable.Builder(this).Interpolator(new LinearInterpolator()).Build();
            _progressBar2.IndeterminateDrawable = new SmoothProgressDrawable.Builder(this).Interpolator(new AccelerateInterpolator()).Build();
            _progressBar3.IndeterminateDrawable = new SmoothProgressDrawable.Builder(this).Interpolator(new DecelerateInterpolator()).Build();
            _progressBar4.IndeterminateDrawable = new SmoothProgressDrawable.Builder(this).Interpolator(new AccelerateDecelerateInterpolator()).Build();

            FindViewById<Button>(Resource.Id.button_make).Click += (s, e) =>
            {
                var intent = new Intent(this, typeof (MakeCustomActivity));
                StartActivity(intent);
            };
        }
    }
}

