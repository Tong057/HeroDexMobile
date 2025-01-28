using CommunityToolkit.Maui.Views;
using HeroDex.Models;

namespace HeroDex.Views.Popups;

public partial class CreateAbilityPopup : Popup
{

    public event EventHandler<Ability> AbilityCreated;
    public Ability Ability { get; set; }
	public CreateAbilityPopup()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
        int.TryParse(PowerField.Text, out int power);

        Ability = new Ability
        {
            Name = NameField.Text,
            Description = DescriptionField.Text,
            Power = power
        };

        AbilityCreated?.Invoke(this, Ability);
        this.Close();
    }
}