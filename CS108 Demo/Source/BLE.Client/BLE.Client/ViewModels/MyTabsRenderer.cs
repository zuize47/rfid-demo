using System;
namespace BLE.Client.ViewModels
{
    [assembly: ExportRenderer(typeof(PageDemo), typeof(MyTabsRenderer))]
    public class MyTabsRenderer
    {
        public MyTabsRenderer()
        {
        }
    }
}
