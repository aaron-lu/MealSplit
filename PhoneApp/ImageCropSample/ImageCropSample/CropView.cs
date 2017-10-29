using System;
using Xamarin.Forms;

namespace ImageCropSample
{
    public class CropView : ContentPage
    {
        public byte[] Image;
        private Action<byte[]> onCropDoneAction;
        public byte[] CroppedImage { get; set; }

        public CropView(byte[] imageAsByte, Action<byte[]> onCropDoneAction)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Color.Black;
            Image = imageAsByte;

            this.onCropDoneAction = onCropDoneAction;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (CroppedImage != null)
                onCropDoneAction?.Invoke(CroppedImage);
        }
    }
}
