using Android.Views;
using Xamarin.Forms;
using ImageCropSample;
using Xamarin.Forms.Platform.Android;
using Com.Theartofdev.Edmodo.Cropper;
using Android.Graphics;
using System.IO;

[assembly: ExportRenderer(typeof(CropView), typeof(ImageCropSample.Droid.Renderer.CropViewRenderer))]
namespace ImageCropSample.Droid.Renderer
{
    public class CropViewRenderer : PageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            var page = Element as CropView;
            if (page != null)
            {
                var cropImageView = new CropImageView(Context);
                cropImageView.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                Bitmap bitmp = BitmapFactory.DecodeByteArray(page.Image, 0, page.Image.Length);
                cropImageView.SetImageBitmap(bitmp);

                var grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Star
                });
                grid.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto
                });
                grid.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto
                });

                var stackLayout = new StackLayout { Children = { cropImageView } };
               
                var rotateButton = new Button { Text = "Rotate" };

                rotateButton.Clicked += (sender, ex) =>
                {
                    cropImageView.RotateImage(90);
                };

                var finishButton = new Button { Text = "Finished" };
                finishButton.Clicked += (sender, ex) =>
                {
                    Bitmap cropped = cropImageView.CroppedImage;
                    using (MemoryStream memory = new MemoryStream())
                    {
                        cropped.Compress(Bitmap.CompressFormat.Png, 100, memory);
                        page.CroppedImage = memory.ToArray();
                    }
                    page.Navigation.PopModalAsync();
                };
                
                grid.Children.Add(stackLayout, 0, 0);
                grid.Children.Add(rotateButton, 0, 1);
                grid.Children.Add(finishButton, 0, 2);
                page.Content = grid;
            }
        }
    }
}