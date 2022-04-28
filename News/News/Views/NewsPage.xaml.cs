using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using News.Models;
using News.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace News.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsPage : ContentPage
    {
        private readonly NewsService _newsService;
        private NewsCategory _category;
        public NewsPage()
        {
            InitializeComponent();
            _newsService = new();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            _category = (NewsCategory) Enum.Parse(typeof(NewsCategory), Title);
            Label.Text = $"Today's {_category} Headlines";
            MainThread.BeginInvokeOnMainThread(async () => { await LoadNews();});
        }

        private async Task LoadNews()
        {
            try
            {
                var result = await _newsService.GetNewsAsync(_category);
                NewsList.ItemsSource = result.Articles;

            }
            catch (Exception e)
            {
                await DisplayAlert("Error", e.Message, "Dismiss");
            }
        }

        private async void NewsList_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            var tappedItem = (NewsItem)e.Item;
            await Navigation.PushAsync(new ArticleView(tappedItem.Url));
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            ActivityIndicator.IsRunning = true;
            await Task.Delay(5000);
            ActivityIndicator.IsRunning = false;
            await LoadNews();
        }
    }
}