using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace LinguaSpace.Common.UI
{
  public class GreyImage : Image
  {
    private ImageSource _sourceC, _sourceG;
    private Brush _opacityMaskC, _opacityMaskG;

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      if (e.Property.Name.Equals("IsEnabled"))
      {
		if ((e.NewValue as bool?) == false)
        {
          Source = _sourceG;
          OpacityMask = _opacityMaskG;
        }
        else if ((e.NewValue as bool?) == true)
        {
          Source = _sourceC;
          OpacityMask = _opacityMaskC;
        }
      }
      else if (e.Property.Name.Equals("Source") &&
               !object.ReferenceEquals(Source, _sourceC) &&
               !object.ReferenceEquals(Source, _sourceG))  // only recache Source if it's the new one from outside
      {
        SetSources();
      }
      else if (e.Property.Name.Equals("OpacityMask") &&
               !object.ReferenceEquals(OpacityMask, _opacityMaskC) &&
               !object.ReferenceEquals(OpacityMask, _opacityMaskG)) // only recache opacityMask if it's the new one from outside
      {
        _opacityMaskC = OpacityMask;
      }

      base.OnPropertyChanged(e);
    }

    /// <summary>
    /// Cashes original ImageSource, creates and caches greyscale ImageSource and greyscale opacity mask
    /// </summary>
    private void SetSources()
    {
      _sourceC = Source;

      try
      {
        // create and cache greyscale ImageSource
        _sourceG = new FormatConvertedBitmap(new BitmapImage(new Uri(TypeDescriptor.GetConverter(Source).ConvertTo(Source, typeof(string)) as string)),
                                             PixelFormats.Gray16, null, 0);

        // create Opacity Mask for greyscale image as FormatConvertedBitmap does not keep transparency info
        _opacityMaskG = new ImageBrush(_sourceC);
        _opacityMaskG.Opacity = 0.5;
        
		this.Source = IsEnabled ? _sourceC : _sourceG;        
		this.OpacityMask = IsEnabled ? _opacityMaskC : _opacityMaskG;

		InvalidateProperty(IsEnabledProperty);
      }
      catch
      {
#if DEBUG
        MessageBox.Show(String.Format("The ImageSource used cannot be greyed out.\nUse BitmapImage or URI as a Source in order to allow greyscaling.\nSource type used: {0}", Source.GetType().Name),
                        "Unsupported Source in GreyableImage", MessageBoxButton.OK, MessageBoxImage.Warning);
#endif // DEBUG

        // in case greyscale image cannot be created set greyscale source to original Source
        _sourceG = Source;
      }
    }
  }
}
