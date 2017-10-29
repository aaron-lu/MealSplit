using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace ImageCropSample
{
	public class ImagePicker : ContentView
    {
        public Image image;
        public Label label;
        public MediaFile file;
        public static BindableProperty ImageDataProperty = BindableProperty.Create(
                                                                nameof(ImageData), 
                                                                typeof(byte[]), 
                                                                typeof(ImagePicker), 
                                                                defaultBindingMode: BindingMode.OneWayToSource);

        public byte[] ImageData
        {
            get
            {
                return (byte[])GetValue(ImageDataProperty);
            }
            set
            {
                SetValue(ImageDataProperty, value);
            }
        }

        public ImagePicker()
        {
			var grid = new Grid
            {
                BackgroundColor = Color.Gray
            };

			grid.GestureRecognizers.Add(new TapGestureRecognizer
            {
				Command = new Command(async (x) =>
                {
                    var action = await Application.Current.MainPage.DisplayActionSheet(null, "Cancel", null, "Photo Library", "Take Photo");

                    var mediaFile = await GetMediaFile(action);

                    if (mediaFile != null)
                    {
                        ImageData = await GetImageBytes(mediaFile);

                        file = mediaFile;
                        await Navigation.PushModalAsync(new CropView(ImageData, OnCropDone));
                    }
                }),
            });

            image = new Image
            {
                Aspect = Aspect.AspectFill
            };

            label = new Label
            {
                BackgroundColor = Color.FromHex("#4c000000"),
                HorizontalTextAlignment = TextAlignment.Center,
                LineBreakMode = LineBreakMode.WordWrap,
                Text = "Select image",
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            grid.Children.Add(image);
            grid.Children.Add(label);

			Content = grid;
        }

        private async Task<MediaFile> GetMediaFile(string action)
        {
            if (action == "Photo Library")
            {
                return await SelectPicture();
            }

            if (action == "Take Photo")
            {
                return await TakePicture();
            }

            return null;
        }

        private void OnCropDone(byte[] croppedImage)
        {
            try
            {
                if (croppedImage != null)
                {
                    label.Text = "Change image";
                    ImageData = croppedImage;
                    image.Source = ImageSource.FromStream(() => new MemoryStream(croppedImage));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #region Photos

        private async Task<MediaFile> SelectPicture()
        {
            try
            {
				var mediaFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
				{
					CompressionQuality = 50,
					PhotoSize = PhotoSize.Medium
				});

                return mediaFile;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<MediaFile> TakePicture()
        {
            try
            {
                var mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    DefaultCamera = CameraDevice.Rear,
                    AllowCropping = true,
                    CompressionQuality = 50,
                    PhotoSize = PhotoSize.Medium
                });

                
                return mediaFile;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static async Task<byte[]> GetImageBytes(MediaFile mediaFile)
        {
            byte[] imageAsByte = null;
            using (var memoryStream = new MemoryStream())
            {
                await mediaFile.GetStream().CopyToAsync(memoryStream);
                imageAsByte = memoryStream.ToArray();
            }

            return imageAsByte;
        }

        #endregion
    }
}
