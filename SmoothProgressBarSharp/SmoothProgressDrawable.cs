using System;
using System.Globalization;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views.Animations;
using Java.Lang;
using Math = System.Math;

namespace SmoothProgressBarSharp
{
    public class SmoothProgressDrawable : Drawable, IAnimatable
    {
        private const long FrameDuration = 1000 / 60;
        private const float OffsetPerFrame = 0.01f;

        private IInterpolator _interpolator;
        private Rect _bounds;
        private readonly Paint _paint;
        private Color[] _colors;
        private int _colorIndex;
        private float _currentOffset;
        private int _seperatorLength;
        private int _sectionsCount;
        private float _speed;
        private bool _reversed;
        private bool _newTurn;
        private bool _mirrorMode;
        private float _maxOffset;

        private SmoothProgressDrawable(IInterpolator interpolator, int sectionsCount, int seperatorLength, Color[] colors,
            float strokeWidth, float speed, bool reversed, bool mirrorMode)
        {
            IsRunning = false;
            _interpolator = interpolator;
            _sectionsCount = sectionsCount;
            _seperatorLength = seperatorLength;
            _speed = speed;
            _reversed = reversed;
            _colors = colors;
            _colorIndex = 0;
            _mirrorMode = mirrorMode;

            _maxOffset = 1f / _sectionsCount;

            _paint = new Paint {StrokeWidth = strokeWidth, AntiAlias = false, Dither = false};
            _paint.SetStyle(Paint.Style.Stroke);
        }

        public IInterpolator Interpolator
        {
            get { return _interpolator; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _interpolator = value;
                InvalidateSelf();
            }
        }

        public Color[] Colors
        {
            get { return _colors; }
            set
            {
                if (value == null || value.Length == 0)
                    throw new ArgumentNullException("value", "value must not be null or empty");
                _colorIndex = 0;
                _colors = value;
                InvalidateSelf();
            }
        }

        public Color Color
        {
            get { return Colors != null ? Colors[0] : Color.Transparent; }
            set { Colors = new[] {value}; }
        }

        public float Speed
        {
            get { return _speed; }
            set
            {
                if (value < 0f) throw new ArgumentOutOfRangeException("value", "value must be >= 0");
                _speed = value;
                InvalidateSelf();
            }
        }

        public int SectionsCount
        {
            get { return _sectionsCount; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("value", "value must be > 0");
                _sectionsCount = value;
                _maxOffset = 1f / _sectionsCount;
                _currentOffset %= _maxOffset;
                InvalidateSelf();
            }
        }

        public int SeperatorLength
        {
            get { return _seperatorLength; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("value", "value must be >= 0");
                _seperatorLength = value;
                InvalidateSelf();
            }
        }

        public float StrokeWidth
        {
            get { return _paint.StrokeWidth; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("value", "value must be >= 0");
                _paint.StrokeWidth = value;
                InvalidateSelf();
            }
        }

        public bool Reversed
        {
            get { return _reversed; }
            set
            {
                if (_reversed == value) return;
                _reversed = value;
                InvalidateSelf();
            }
        }

        public bool MirrorMode
        {
            get { return _mirrorMode; }
            set
            {
                if (_mirrorMode == value) return;
                _mirrorMode = value;
                InvalidateSelf();
            }
        }

        public override void Draw(Canvas canvas)
        {
            _bounds = Bounds;
            canvas.ClipRect(_bounds);

            if (_reversed)
            {
                canvas.Translate(_bounds.Width(), 0);
                canvas.Scale(-1, 1);
            }

            DrawStrokes(canvas);
        }

        private void DrawStrokes(Canvas canvas)
        {
            var prevValue = 0f;
            var boundsWidth = _bounds.Width();
            if (_mirrorMode) boundsWidth /= 2;
            var width = boundsWidth + _seperatorLength + _sectionsCount;
            var centerY = _bounds.CenterY();
            var xSectionWidth = 1f / _sectionsCount;

            if (_newTurn)
            {
                _colorIndex = DecrementColor(_colorIndex);
                _newTurn = false;
            }

            var currentIndexColor = _colorIndex;

            for (var i = 0; i <= _sectionsCount; ++i)
            {
                var xOffset = xSectionWidth * i + _currentOffset;
                var prev = Math.Max(0f, xOffset - xSectionWidth);
                var ratioSectionWidth = Math.Abs(
                    _interpolator.GetInterpolation(prev) -
                    _interpolator.GetInterpolation(Math.Min(xOffset, 1f)));
                float sectionWidth = (int)(width * ratioSectionWidth);

                var spaceLength = sectionWidth + prev < width ? Math.Min(sectionWidth, _seperatorLength) : 0f;

                var drawLength = sectionWidth > spaceLength ? sectionWidth - spaceLength : 0;
                var end = prevValue + drawLength;
                if (end > prevValue)
                {
                    DrawLine(canvas, boundsWidth,
                            Math.Min(boundsWidth, prevValue), centerY, Math.Min(boundsWidth, end), centerY,
                            currentIndexColor);
                }
                prevValue = end + spaceLength;
                currentIndexColor = IncrementColor(currentIndexColor);
            }
        }

        private void DrawLine(Canvas canvas, int canvasWidth, float startX, float startY, float stopX, float stopY, int currentIndexColor)
        {
            _paint.Color = _colors[currentIndexColor];

            if (!_mirrorMode)
            {
                canvas.DrawLine(startX, startY, stopX, stopY, _paint);
            }
            else
            {
                if (_reversed)
                {
                    canvas.DrawLine(canvasWidth + startX, startY, canvasWidth + stopX, stopY, _paint);
                    canvas.DrawLine(canvasWidth - startX, startY, canvasWidth - stopX, stopY, _paint);
                }
                else
                {
                    canvas.DrawLine(startX, startY, stopX, stopY, _paint);
                    canvas.DrawLine(canvasWidth * 2 - startX, startY, canvasWidth * 2 - stopX, stopY, _paint);
                }
            }

            canvas.Save();
        }

        private int IncrementColor(int colorIndex)
        {
            ++colorIndex;
            if (colorIndex >= _colors.Length) colorIndex = 0;
            return colorIndex;
        }

        private int DecrementColor(int colorIndex)
        {
            --colorIndex;
            if (colorIndex < 0) colorIndex = _colors.Length - 1;
            return colorIndex;
        }

        public override void SetAlpha(int alpha)
        {
            _paint.Alpha = alpha;
        }

        public override void SetColorFilter(ColorFilter cf)
        {
            _paint.SetColorFilter(cf);
        }

        public override int Opacity
        {
            get { return (int)Format.Transparent; }
        }

        public override void ScheduleSelf(IRunnable what, long when)
        {
            IsRunning = true;
            base.ScheduleSelf(what, when);
        }

        public void Start()
        {
            if (IsRunning) return;
            ScheduleSelf(Runner, DateTime.Now.Millisecond + FrameDuration);
            InvalidateSelf();
        }

        public void Stop()
        {
            if (!IsRunning) return;
            IsRunning = false;
            UnscheduleSelf(Runner);
        }

        public bool IsRunning { get; private set; }

        private void Runner()
        {
            _currentOffset += (OffsetPerFrame * _speed);
            if (_currentOffset >= _maxOffset)
            {
                _newTurn = true;
                _currentOffset -= _maxOffset;
            }
            ScheduleSelf(Runner, DateTime.Now.Millisecond + FrameDuration);
            InvalidateSelf();
        }

        public class Builder
        {
            private static IInterpolator _interpolator;
            private static int _sectionsCount;
            private static Color[] _colors;
            private static float _speed;
            private static bool _reversed;
            private static bool _mirrorMode;

            private static int _strokeSeparatorLength;
            private static float _strokeWidth;

            public Builder(Context context)
            {
                InitValues(context);
            }

            public SmoothProgressDrawable Build()
            {
                var ret = new SmoothProgressDrawable(_interpolator, _sectionsCount, _strokeSeparatorLength, _colors,
                    _strokeWidth, _speed, _reversed, _mirrorMode);
                return ret;
            }

            private static void InitValues(Context context)
            {
                var res = context.Resources;
                _interpolator = new AccelerateInterpolator();
                _sectionsCount = res.GetInteger(Resource.Integer.spb_default_sections_count);

                _colors = new[]{res.GetColor(Resource.Color.spb_default_color)};
                _speed = float.Parse(res.GetString(Resource.String.spb_default_speed), CultureInfo.InvariantCulture);
                _reversed = res.GetBoolean(Resource.Boolean.spb_default_reversed);

                _strokeSeparatorLength = res.GetDimensionPixelSize(Resource.Dimension.spb_default_stroke_separator_length);
                _strokeWidth = res.GetDimensionPixelOffset(Resource.Dimension.spb_default_stroke_width);
            }

            public Builder Interpolator(IInterpolator interpolator)
            {
                if (interpolator == null)
                    throw new ArgumentNullException("interpolator");
                _interpolator = interpolator;
                return this;
            }

            public Builder SectionsCount(int sectionsCount)
            {
                if (sectionsCount <= 0)
                    throw new ArgumentOutOfRangeException("sectionsCount", "SectionsCount must be > 0");
                _sectionsCount = sectionsCount;
                return this;
            }

            public Builder SeparatorLength(int separatorLength)
            {
                if (separatorLength < 0)
                    throw new ArgumentOutOfRangeException("separatorLength", "SeparatorLength must be >= 0");
                _strokeSeparatorLength = separatorLength;
                return this;
            }

            public Builder Color(Color color)
            {
                _colors = new[] { color };
                return this;
            }

            public Builder Colors(Color[] colors)
            {
                if (colors == null || colors.Length == 0)
                    throw new ArgumentNullException("colors", "Your color array must not be empty");
                _colors = colors;
                return this;
            }

            public Builder StrokeWidth(float width)
            {
                if (width < 0) throw new ArgumentOutOfRangeException("width", "The width must be >= 0");
                _strokeWidth = width;
                return this;
            }

            public Builder Speed(float speed)
            {
                if (speed < 0) throw new ArgumentOutOfRangeException("speed", "Speed must be >= 0");
                _speed = speed;
                return this;
            }

            public Builder Reversed(bool reversed)
            {
                _reversed = reversed;
                return this;
            }

            public Builder MirrorMode(bool mirrorMode)
            {
                _mirrorMode = mirrorMode;
                return this;
            }
        }
    }
}