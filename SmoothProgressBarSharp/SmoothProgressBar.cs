using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views.Animations;
using Android.Widget;

namespace SmoothProgressBarSharp
{
    public sealed class SmoothProgressBar : ProgressBar
    {
        private enum InterpolatorType
        {
            Linear = 1,
            AccelerateDecelerate = 2,
            Decelerate = 3
        }

        private SmoothProgressBar(IntPtr javaReference, JniHandleOwnership transfer) 
            : base(javaReference, transfer)
        {
        }

        public SmoothProgressBar(Context context) 
            : this(context, null)
        {
        }

        public SmoothProgressBar(Context context, IAttributeSet attrs) 
            : this(context, attrs, Resource.Attribute.spbStyle)
        {
        }

        public SmoothProgressBar(Context context, IAttributeSet attrs, int defStyle) 
            : base(context, attrs, defStyle)
        {
            var res = context.Resources;
            var a = context.ObtainStyledAttributes(attrs, Resource.Styleable.SmoothProgressBar, defStyle, 0);

            var color = a.GetColor(Resource.Styleable.SmoothProgressBar_spb_color,
                res.GetColor(Resource.Color.spb_default_color));
            var sectionsCount = a.GetInteger(Resource.Styleable.SmoothProgressBar_spb_sections_count,
                res.GetInteger(Resource.Integer.spb_default_sections_count));
            var separatorLength =
                a.GetDimensionPixelSize(Resource.Styleable.SmoothProgressBar_spb_stroke_separator_length,
                    res.GetDimensionPixelSize(Resource.Dimension.spb_default_stroke_separator_length));
            var strokeWidth = a.GetDimension(Resource.Styleable.SmoothProgressBar_spb_stroke_width,
                res.GetDimension(Resource.Dimension.spb_default_stroke_width));
            var speed = a.GetFloat(Resource.Styleable.SmoothProgressBar_spb_speed,
                float.Parse(res.GetString(Resource.String.spb_default_speed)));
            var iInterpolator = a.GetInteger(Resource.Styleable.SmoothProgressBar_spb_interpolator,
                res.GetInteger(Resource.Integer.spb_default_interpolator));
            var reversed = a.GetBoolean(Resource.Styleable.SmoothProgressBar_spb_reversed,
                res.GetBoolean(Resource.Boolean.spb_default_reversed));
            var mirrorMode = a.GetBoolean(Resource.Styleable.SmoothProgressBar_spb_mirror_mode,
                res.GetBoolean(Resource.Boolean.spb_default_mirror_mode));
            var colorsId = a.GetResourceId(Resource.Styleable.SmoothProgressBar_spb_colors, 0);
            a.Recycle();

            IInterpolator interpolator;
            switch (iInterpolator)
            {
                case (int)InterpolatorType.AccelerateDecelerate:
                    interpolator = new AccelerateDecelerateInterpolator();
                    break;
                case (int)InterpolatorType.Linear:
                    interpolator = new LinearInterpolator();
                    break;
                case (int)InterpolatorType.Decelerate:
                    interpolator = new DecelerateInterpolator();
                    break;
                default:
                    interpolator = new AccelerateInterpolator();
                    break;        
            }

            Color[] colors = null;
            if (colorsId != 0)
            {
                colors = IntsToColors(res.GetIntArray(colorsId));
            }

            var builder =
                new SmoothProgressDrawable.Builder(context).Speed(speed)
                    .Interpolator(interpolator)
                    .SectionsCount(sectionsCount)
                    .SeparatorLength(separatorLength)
                    .StrokeWidth(strokeWidth)
                    .Reversed(reversed)
                    .MirrorMode(mirrorMode);

            if (colors != null && colors.Length > 0)
                builder.Colors(colors);
            else
                builder.Color(color);

            IndeterminateDrawable = builder.Build();
        }

        private static Color[] IntsToColors(IEnumerable<int> colors)
        {
            return colors.Select(x => new Color(x)).ToArray();
        }

        public override IInterpolator Interpolator
        {
            get { return base.Interpolator; }
            set
            {
                base.Interpolator = value;
                var ret = IndeterminateDrawable;
                if (ret == null) return;
                var drawable = ret as SmoothProgressDrawable;
                if (drawable != null)
                    drawable.Interpolator = value;
            }
        }

        private SmoothProgressDrawable CheckIndeterminateDrawable()
        {
            var ret = IndeterminateDrawable;
            if (ret == null) 
                throw new InvalidOperationException("The drawable is not a SmoothProgressDrawable");
            var drawable = ret as SmoothProgressDrawable;
            if (drawable == null)
                throw new InvalidOperationException("The drawable is not a SmoothProgressDrawable");
            return (SmoothProgressDrawable) ret;
        }

        public void SetSmoothProgressDrawableInterpolator(IInterpolator interpolator)
        {
            CheckIndeterminateDrawable().Interpolator = interpolator;
        }

        public void SetSmoothProgressDrawableColors(int[] colors)
        {
            SetSmoothProgressDrawableColors(IntsToColors(colors));
        }

        public void SetSmoothProgressDrawableColors(Color[] colors)
        {
            CheckIndeterminateDrawable().Colors = colors;
        }

        public void SetSmoothProgressDrawableColor(Color color)
        {
            CheckIndeterminateDrawable().Color = color;
        }
        public void SetSmoothProgressDrawableColor(int color)
        {
            SetSmoothProgressDrawableColor(new Color(color));
        }

        public void SetSmoothProgressDrawableSpeed(float speed)
        {
            CheckIndeterminateDrawable().Speed = speed;
        }

        public void SetSmoothProgressDrawableSectionsCount(int sectionsCount)
        {
            CheckIndeterminateDrawable().SectionsCount = sectionsCount;
        }

        public void SetSmoothProgressDrawableSeparatorLength(int separatorLength)
        {
            CheckIndeterminateDrawable().SeperatorLength = separatorLength;
        }

        public void SetSmoothProgressDrawableStrokeWidth(float strokeWidth)
        {
            CheckIndeterminateDrawable().StrokeWidth = strokeWidth;
        }

        public void SetSmoothProgressDrawableReversed(bool reversed)
        {
            CheckIndeterminateDrawable().Reversed = reversed;
        }

        public void SetSmoothProgressDrawableMirrorMode(bool mirrorMode)
        {
            CheckIndeterminateDrawable().MirrorMode = mirrorMode;
        }
    }
}