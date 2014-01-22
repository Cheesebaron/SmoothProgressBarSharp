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
            var reversed = a.GetBoolean(Resource.Styleable.SmoothProgressBar_spb_reversed,
                res.GetBoolean(Resource.Boolean.spb_default_reversed));
            var mirrorMode = a.GetBoolean(Resource.Styleable.SmoothProgressBar_spb_mirror_mode,
                res.GetBoolean(Resource.Boolean.spb_default_mirror_mode));
            var colorsId = a.GetResourceId(Resource.Styleable.SmoothProgressBar_spb_colors, 0);
            var idInterpolator = a.GetResourceId(Resource.Styleable.SmoothProgressBar_android_interpolator, 0);
            a.Recycle();

            if (idInterpolator > 0)
                SetInterpolator(context, idInterpolator);

            Color[] colors = null;
            if (colorsId != 0)
                colors = IntsToColors(res.GetIntArray(colorsId));

            var builder =
                new SmoothProgressDrawable.Builder(context)
                    .Speed(speed)
                    .SectionsCount(sectionsCount)
                    .SeparatorLength(separatorLength)
                    .StrokeWidth(strokeWidth)
                    .Reversed(reversed)
                    .MirrorMode(mirrorMode);

            if (Interpolator != null) builder.Interpolator(Interpolator);

            if (colors != null && colors.Length > 0)
                builder.Colors(colors);
            else
                builder.Color(color);

            IndeterminateDrawable = builder.Build();
        }

        public void ApplyStyle(int styleResId)
        {
            var a = Context.ObtainStyledAttributes(null, Resource.Styleable.SmoothProgressBar, 0, styleResId);

            if (a.HasValue(Resource.Styleable.SmoothProgressBar_spb_color))
                SmoothProgressDrawableColor = a.GetColor(Resource.Styleable.SmoothProgressBar_spb_color, 0);
            if (a.HasValue(Resource.Styleable.SmoothProgressBar_spb_colors))
            {
                var colorsId = a.GetResourceId(Resource.Styleable.SmoothProgressBar_spb_colors, 0);
                if (colorsId != 0)
                {
                    var colors = Resources.GetIntArray(colorsId);
                    if (colors != null && colors.Length > 0)
                        SmoothProgressDrawableColors = IntsToColors(colors);
                }
            }
            if (a.HasValue(Resource.Styleable.SmoothProgressBar_spb_sections_count))
                SmoothProgressDrawableSectionsCount =
                    a.GetInteger(Resource.Styleable.SmoothProgressBar_spb_sections_count, 0);
            if (a.HasValue(Resource.Styleable.SmoothProgressBar_spb_stroke_separator_length))
                SmoothProgressDrawableSectionsCount =
                    a.GetDimensionPixelSize(Resource.Styleable.SmoothProgressBar_spb_stroke_separator_length, 0);
            if (a.HasValue(Resource.Styleable.SmoothProgressBar_spb_stroke_width))
                SmoothProgressDrawableStrokeWidth = a.GetDimension(
                    Resource.Styleable.SmoothProgressBar_spb_stroke_width, 0);
            if (a.HasValue(Resource.Styleable.SmoothProgressBar_spb_speed))
                SmoothProgressDrawableSpeed = a.GetFloat(Resource.Styleable.SmoothProgressBar_spb_speed, 0);
            if (a.HasValue(Resource.Styleable.SmoothProgressBar_spb_reversed))
                SmoothProgressDrawableReversed = a.GetBoolean(Resource.Styleable.SmoothProgressBar_spb_reversed, false);
            if (a.HasValue(Resource.Styleable.SmoothProgressBar_spb_mirror_mode))
                SmoothProgressDrawableMirrorMode = a.GetBoolean(Resource.Styleable.SmoothProgressBar_spb_mirror_mode,
                    false);
            var resId = a.GetResourceId(Resource.Styleable.SmoothProgressBar_android_interpolator, 0);
            if (resId > 0)
                SetInterpolator(Context, resId);

            a.Recycle();
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

        private SmoothProgressDrawable CheckedIndeterminateDrawable
        {
            get
            {
                var ret = IndeterminateDrawable;
                if (ret == null)
                    throw new InvalidOperationException("The drawable is not a SmoothProgressDrawable");
                var drawable = ret as SmoothProgressDrawable;
                if (drawable == null)
                    throw new InvalidOperationException("The drawable is not a SmoothProgressDrawable");
                return (SmoothProgressDrawable)ret;    
            }
        }

        public IInterpolator SmoothProgressDrawableInterpolator
        {
            get { return CheckedIndeterminateDrawable.Interpolator; }
            set { CheckedIndeterminateDrawable.Interpolator = value; }
        }

        public Color[] SmoothProgressDrawableColors
        {
            get { return CheckedIndeterminateDrawable.Colors; }
            set { CheckedIndeterminateDrawable.Colors = value; }
        }

        public Color SmoothProgressDrawableColor
        {
            get { return CheckedIndeterminateDrawable.Color; }
            set { CheckedIndeterminateDrawable.Color = value; }
        }

        public float SmoothProgressDrawableSpeed
        {
            get { return CheckedIndeterminateDrawable.Speed; }
            set { CheckedIndeterminateDrawable.Speed = value; }
        }

        public int SmoothProgressDrawableSectionsCount
        {
            get { return CheckedIndeterminateDrawable.SectionsCount; }
            set { CheckedIndeterminateDrawable.SectionsCount = value; }
        }

        public int SmoothProgressDrawableSeparatorLength
        {
            get { return CheckedIndeterminateDrawable.SeperatorLength; }
            set { CheckedIndeterminateDrawable.SeperatorLength = value; }
        }

        public float SmoothProgressDrawableStrokeWidth
        {
            get { return CheckedIndeterminateDrawable.StrokeWidth; }
            set { CheckedIndeterminateDrawable.StrokeWidth = value; }
        }

        public bool SmoothProgressDrawableReversed
        {
            get { return CheckedIndeterminateDrawable.Reversed; }
            set { CheckedIndeterminateDrawable.Reversed = value; }
        }

        public bool SmoothProgressDrawableMirrorMode
        {
            get { return CheckedIndeterminateDrawable.MirrorMode; }
            set { CheckedIndeterminateDrawable.MirrorMode = value; }
        }
    }
}