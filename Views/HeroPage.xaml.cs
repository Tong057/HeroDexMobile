using HeroDex.ViewModels;

namespace HeroDex.Views;

// If Edit
[QueryProperty(nameof(HeroId), "HeroId")]
public partial class HeroPage : ContentPage
{
    private HeroViewModel _heroVM;
	public HeroPage(HeroViewModel heroVM)
	{
		InitializeComponent();

		BindingContext = heroVM;
        _heroVM = heroVM;
	}

    public string HeroId
    {
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Task.Run(async () => await _heroVM.InitializeEditMode(value));
            }
        }
    }
}