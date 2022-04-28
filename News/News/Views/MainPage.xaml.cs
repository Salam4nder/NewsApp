using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using News.Models;

namespace News.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
 
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();

        }
        private async void NewsList_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            var tappedItem = (NewsItem)e.Item;
            await Navigation.PushAsync(new ArticleView(tappedItem.Url));
        }
    }
}