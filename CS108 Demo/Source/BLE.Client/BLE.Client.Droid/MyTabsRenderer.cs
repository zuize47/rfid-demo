using System;
using System.ComponentModel;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using BLE.Client.Droid;
using BLE.Client.Pages;
using BLE.Client.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(PageDemo), typeof(MyTabsRenderer))]
namespace BLE.Client.Droid
{
    public class MyTabsRenderer : TabbedPageRenderer
    {
        public MyTabsRenderer()
        {
        }


        ViewPager pager;
        TabLayout layout;
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);




 
                pager = (ViewPager)ViewGroup.GetChildAt(0);

                layout = (TabLayout)ViewGroup.GetChildAt(1);

            ViewModelDemo.TabIndex = layout.SelectedTabPosition;


                System.Diagnostics.Debug.WriteLine("Tab111:" + layout.SelectedTabPosition);
           /* for (int i = 0; i < layout.TabCount; i++)
                {
                    var tab = layout.GetTabAt(i);
                    System.Diagnostics.Debug.WriteLine("Tab111:" + tab.Text);
                    if (tab.IsSelected)
                    {

                        System.Diagnostics.Debug.WriteLine("Tab:" + tab.Text);
                    }


                }
                */
          
        }
    }
}
