using HeroDex.ViewModels;

namespace HeroDex.Views;

public partial class HeroesListPage : ContentPage
{
    private bool _isFloatingButtonVisible = true;
    private HeroesListViewModel _heroesListVM;

    public HeroesListPage(HeroesListViewModel heroesListVM)
	{
		InitializeComponent();

		BindingContext = heroesListVM;
        _heroesListVM = heroesListVM;
	}

    protected override async void OnAppearing()
    {
        await Task.Run(_heroesListVM.LoadHeroes);
    }

    protected async void OnCollectionViewScrolled(object sender, DevExpress.Maui.CollectionView.DXCollectionViewScrolledEventArgs e)
    {
        if (e.Delta > 5)
        {
            // Scrolling down
            if (_isFloatingButtonVisible)
            {
                _isFloatingButtonVisible = false;
                await HideButtonAsync();
            }
        }
        else if (e.Delta < 0)
        {
            // Scrolling up
            if (!_isFloatingButtonVisible)
            {
                await ShowButtonAsync();
                _isFloatingButtonVisible = true;
            }
        }
    }

    private async Task ShowButtonAsync()
    {
        if (FloatingButton != null)
        {
            FloatingButton.IsVisible = true;
            await Task.WhenAll(
                FloatingButton.TranslateTo(0, 0, 300, Easing.CubicInOut),
                FloatingButton.FadeTo(1, 300, Easing.CubicInOut)
            );
        }
    }

    private async Task HideButtonAsync()
    {
        if (FloatingButton != null)
        {
            await Task.WhenAll(
                FloatingButton.TranslateTo(0, 100, 300, Easing.CubicInOut),
                FloatingButton.FadeTo(0, 300, Easing.CubicInOut)
            );

            FloatingButton.IsVisible = false;
        }
    }
}