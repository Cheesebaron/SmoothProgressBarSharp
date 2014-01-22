> Note: Some Xamarin.Android versions have problems with only calling the (IntPtr javaReference, JniHandleOwnership transfer) constructor. 4.11.0 has been tested to work with `SmoothProgressBar`. The `SmoothProgressDrawable` works on any version.

##Description

This is a port of [the SmoothProgressBar library] for Android to Xamarin.Android.

It is a small library allowing you to make a smooth indeterminate progress bar. You can either user your progress bars and set this drawable or use directly the `SmoothProgressBar`.

##How does it work

The original author made a nice [blog post] about that.

##Usage

-	Use directly SmoothProgressBar:

```xml
<smoothprogressbarsharp.SmoothProgressBar
	xmlns:android="http://schemas.android.com/apk/res/android"
  	xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="wrap_content"
    android:indeterminate="true"
    app:spb_sections_count="12"
    app:spb_color="#FF0000"
    app:spb_speed="2.0"
    app:spb_stroke_width="4dp"
    app:spb_stroke_separator_length="4dp"
    app:spb_reversed="false"
    app:spb_mirror_mode="false"
    />
```

-   Or instantiate a `SmoothProgressDrawable` and set it to your ProgressBar (do not forget to set the Horizontal Style)

```java
_progressBar.IndeterminateDrawable = new SmoothProgressDrawable.Builder(context)
    .Color(0xff0000)
    .Interpolator(new DecelerateInterpolator())
    .SectionsCount(4)
    .SeparatorLength(8)     //You should use Resources#getDimensionPixelSize
    .StrokeWidth(8f)         //You should use Resources#getDimension
    .Speed(2.0)             //2 times faster
    .Reversed(false)
    .MirrorMode(false)
    .Build();
```

You can also set many colors for one bar (see G+ app)

-   via xml (use the `app:spb_colors` attribute with a `integer-array` reference for that)

-   programmatically (use `SmoothProgressDrawable.Builder#Colors(int[])` method).


##License

```
"THE BEER-WARE LICENSE" (Revision 42):
You can do whatever you want with this stuff.
If we meet some day, and you think this stuff is worth it, you can buy me a beer in return.
```

[blog post]: http://antoine-merle.com/blog/2013/11/12/make-your-progressbar-more-smooth/
[the SmoothProgressBar library]: https://github.com/castorflex/SmoothProgressBar/
