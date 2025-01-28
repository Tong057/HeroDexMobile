using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeroDex.Models;
using HeroDex.Models.Enums;
using HeroDex.Services;
using HeroDex.Views.Popups;
using System.Collections.ObjectModel;

namespace HeroDex.ViewModels
{
    public partial class HeroViewModel : ObservableObject
    {

        private HeroesService _heroService;
        private ImageSource _thumbnailImageSource = "account.png";

        public HeroViewModel(HeroesService heroService)
        {
            _heroService = heroService;
            ImageSource = _thumbnailImageSource;
        }

        public string Id { get; set; }

        [ObservableProperty]
        public partial bool IsEditMode { get; set; } = false;

        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial string Name { get; set; }

        [ObservableProperty]
        public partial double Height { get; set; }

        [ObservableProperty]
        public partial double Weight{ get; set; }

        [ObservableProperty]
        public partial HeroType HeroType { get; set; }

        [ObservableProperty]
        public partial ImageSource ImageSource { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<Ability> AbilitiesList { get; set; } = new ObservableCollection<Ability>();

        [RelayCommand]
        private async Task SendHero()
        {
            try
            {
                if (IsBusy)
                    return;

                if (!ValidateHero(out var validationMessage))
                {
                    await Toast.Make(validationMessage).Show();
                    return;
                }

                IsBusy = true;
                var hero = new Hero
                {
                    Id = Id,
                    Name = Name,
                    Attributes = new Attributes
                    {
                        Height = Height,
                        Weight = Weight
                    },
                    Abilities = AbilitiesList.ToList(),
                    HeroType = HeroType
                };

                if (IsEditMode)
                {
                    await _heroService.UpdateHeroAsync(hero, ImageSource);
                    await Toast.Make("The Hero successfully updated!").Show();
                }
                else
                {
                    await _heroService.CreateHeroAsync(hero, ImageSource.Equals(_thumbnailImageSource) ? null : ImageSource);
                    await Toast.Make("The Hero successfully created!").Show();
                }

                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
            finally
            {
                IsBusy = false;
            }

        }

        [RelayCommand]
        private void ChangeHeroType(string type)
        {
            if (Enum.TryParse<HeroType>(type, out var heroType))
            {
                HeroType = heroType;
            }
        }

        [RelayCommand]
        private async Task CreateAbility()
        {
            var popup = new CreateAbilityPopup();
            popup.AbilityCreated += (s, e) => AbilitiesList.Add(e);
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        }

        [RelayCommand]
        private void DeleteAbility(Ability ability)
        {
            AbilitiesList.Remove(ability);
        }

        [RelayCommand]
        private async Task AddPhoto()
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select a photo",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                ImageSource = ImageSource.FromFile(result.FullPath);
            }
        }

        public async Task InitializeEditMode(string id)
        {
            try
            {
                IsBusy = true;

                IsEditMode = true;
                Id = id;
                var hero = await _heroService.GetHeroByIdAsync(id);
                var image = await _heroService.GetHeroImageAsync(id);
                Name = hero.Name;
                Height = hero.Attributes.Height;
                Weight = hero.Attributes.Weight;
                HeroType = hero.HeroType;

                foreach (var a in hero.Abilities)
                {
                    AbilitiesList.Add(a);
                }

                IsBusy = false;

                await Task.Delay(100);
                ImageSource = image;
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool ValidateHero(out string validationMessage)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                validationMessage = "Name is required!";
                return false;
            }

            if (Height <= 0)
            {
                validationMessage = "Height must be greater than 0!";
                return false;
            }

            if (Weight <= 0)
            {
                validationMessage = "Weight must be greater than 0!";
                return false;
            }

            validationMessage = string.Empty;
            return true;
        }


    }
}
