using System;

using Xamarin.Forms;

namespace BLE.Client.Pages
{
    public class PageIssueProduct : ContentPage
    {
        public PageIssueProduct()
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

