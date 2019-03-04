using System;

using Xamarin.Forms;

namespace BLE.Client.Pages
{
    public class PageCheckProduct : ContentPage
    {
        public PageCheckProduct()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

