using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace ImageCropSample
{
	public partial class MyPage : ContentPage
	{
		public MyPage()
		{
			InitializeComponent();
            BindingContext = new MyPageViewModel();
		}

        SelectMultipleBasePage<ItemizedFood> multiPage;

        private async void Button_Clicked(object sender, EventArgs e)
        {
            //var imageData = (BindingContext as MyPageViewModel).FrontImageData;
            ImageToText textGen = new ImageToText();
            ObservableCollection<ItemizedFood> itemList = await textGen.getTextAsync(image.file);
            if (multiPage == null)
                multiPage = new SelectMultipleBasePage<ItemizedFood>(itemList.ToList()) { Title = "Check all that apply" };

            await Navigation.PushAsync(multiPage);
        }
    }

    public class MyPageViewModel
    {
        public byte[] FrontImageData { get; set; }
        public byte[] RearImageData { get; set; }
    }
}
