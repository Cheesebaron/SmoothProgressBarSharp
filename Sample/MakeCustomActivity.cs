using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views.Animations;
using Android.Widget;
using SmoothProgressBarSharp;

namespace Sample
{
    [Activity(Label = "Custom")]
    public class MakeCustomActivity : Activity
    {
        private SmoothProgressBar _progressBar;
        private CheckBox _checkBoxMirror;
        private CheckBox _checkBoxReversed;
        private Spinner _spinnerInterpolators;
        private SeekBar _seekBarSectionsCount;
        private SeekBar _seekBarStrokeWidth;
        private SeekBar _seekBarSeparatorLength;
        private SeekBar _seekBarSpeed;
        private Button _button;
        private TextView _textViewSpeed;
        private TextView _textViewStrokeWidth;
        private TextView _textViewSeparatorLength;
        private TextView _textViewSectionsCount;

        private float _speed;
        private int _strokeWidth;
        private int _separatorLength;
        private int _sectionsCount;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_custom);

            _progressBar = FindViewById<SmoothProgressBar>(Resource.Id.progressbar);
            _checkBoxMirror = FindViewById<CheckBox>(Resource.Id.checkbox_mirror);
            _checkBoxReversed = FindViewById<CheckBox>(Resource.Id.checkbox_reversed);
            _spinnerInterpolators = FindViewById<Spinner>(Resource.Id.spinner_interpolator);
            _seekBarSectionsCount = FindViewById<SeekBar>(Resource.Id.seekbar_sections_count);
            _seekBarStrokeWidth = FindViewById<SeekBar>(Resource.Id.seekbar_stroke_width);
            _seekBarSeparatorLength = FindViewById<SeekBar>(Resource.Id.seekbar_separator_length);
            _seekBarSpeed = FindViewById<SeekBar>(Resource.Id.seekbar_speed);
            _button = FindViewById<Button>(Resource.Id.button);
            _textViewSpeed = FindViewById<TextView>(Resource.Id.textview_speed);
            _textViewSectionsCount = FindViewById<TextView>(Resource.Id.textview_sections_count);
            _textViewSeparatorLength = FindViewById<TextView>(Resource.Id.textview_separator_length);
            _textViewStrokeWidth = FindViewById<TextView>(Resource.Id.textview_stroke_width);

            _seekBarSpeed.ProgressChanged += (s, e) =>
            {
                _speed = ((float) e.Progress + 1) / 10;
                _textViewSpeed.Text = "Speed: " + _speed;
            };

            _seekBarSectionsCount.ProgressChanged += (s, e) =>
            {
                _sectionsCount = e.Progress + 1;
                _textViewSectionsCount.Text = "Sections count: " + _sectionsCount;
            };

            _seekBarSeparatorLength.ProgressChanged += (s, e) =>
            {
                _separatorLength = e.Progress;
                _textViewSeparatorLength.Text = string.Format("Separator length: {0}dp", _separatorLength);
            };

            _seekBarStrokeWidth.ProgressChanged += (s, e) =>
            {
                _strokeWidth = e.Progress;
                _textViewStrokeWidth.Text = string.Format("Stroke width: {0}dp", _strokeWidth);
            };

            _seekBarSeparatorLength.Progress = 4;
            _seekBarSectionsCount.Progress = 4;
            _seekBarStrokeWidth.Progress = 4;
            _seekBarSpeed.Progress = 9;

            _spinnerInterpolators.Adapter = new ArrayAdapter<string>(this,
                Android.Resource.Layout.SimpleSpinnerDropDownItem,
                Resources.GetStringArray(Resource.Array.interpolators));

            _button.Click += (s, e) => SetValues();
        }

        private void SetValues()
        {
            _progressBar.SmoothProgressDrawableSpeed = _speed;
            _progressBar.SmoothProgressDrawableSectionsCount = _sectionsCount;
            _progressBar.SmoothProgressDrawableSeparatorLength = DpToPx(_separatorLength);
            _progressBar.SmoothProgressDrawableStrokeWidth = DpToPx(_strokeWidth);
            _progressBar.SmoothProgressDrawableReversed = _checkBoxReversed.Checked;
            _progressBar.SmoothProgressDrawableMirrorMode = _checkBoxMirror.Checked;

            IInterpolator interpolator;
            switch (_spinnerInterpolators.SelectedItemPosition)
            {
                case 1:
                    interpolator = new LinearInterpolator();
                    break;
                case 2:
                    interpolator = new AccelerateDecelerateInterpolator();
                    break;
                case 3:
                    interpolator = new DecelerateInterpolator();
                    break;
                default:
                    interpolator = new AccelerateInterpolator();
                    break;
            }

            _progressBar.SmoothProgressDrawableInterpolator = interpolator;
            _progressBar.SmoothProgressDrawableColors = IntsToColors(Resources.GetIntArray(Resource.Array.colors));
        }

        private int DpToPx(int dp)
        {
            var px = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip,
                    dp, Resources.DisplayMetrics);
            return px;
        }

        private static Color[] IntsToColors(IEnumerable<int> colors)
        {
            return colors.Select(x => new Color(x)).ToArray();
        }
    }
}