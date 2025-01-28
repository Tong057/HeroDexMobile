using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeroDex.Models;
using HeroDex.Models.Enums;
using HeroDex.Services;
using HeroDex.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroDex.ViewModels
{
    public partial class HeroesListViewModel : ObservableObject
    {
        private HeroesService _heroesService;
        public HeroesListViewModel(HeroesService heroesService)
        {
            _heroesService = heroesService;
            HeroesList = new ObservableCollection<Hero>();
        }

        [ObservableProperty]
        public partial ObservableCollection<Hero> HeroesList { get; set; }


        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial bool IsRefreshing { get; set; }

        [ObservableProperty]
        public partial string SearchText { get; set; }

        partial void OnSearchTextChanged(string value)
        {
            LoadHeroes();
        }

        [RelayCommand]
        private async Task RefreshHeroList()
        {
            try
            {
                await Task.Run(LoadHeroes);
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
        }

        [RelayCommand]
        private async Task DeleteHero(Hero hero)
        {
            try
            {
                await _heroesService.DeleteHeroAsync(hero.Id);
                HeroesList.Remove(hero);
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
        }

        [RelayCommand]
        private async Task GoToEditHeroPage(Hero hero)
        {
            await Shell.Current.GoToAsync($"{nameof(HeroPage)}?HeroId={hero.Id}");
        }


        [RelayCommand]
        private async Task GoToCreateHeroPage()
        {
            await Shell.Current.GoToAsync(nameof(HeroPage));
        }

        public async Task LoadHeroes()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var heroes = await _heroesService.GetAllHeroesAsync(type: CurrentHeroType.ToString(), name: SearchText);
                HeroesList.Clear();
                foreach (var h in heroes)
                {
                    HeroesList.Add(h);
                }
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        // Filtering

        private readonly List<HeroType?> _heroTypes = new() { null, HeroType.Hero, HeroType.Villain, HeroType.AntiHero };

        [ObservableProperty]
        public partial HeroType? CurrentHeroType { get; set; }

        private int _currentIndex = 0;

        [RelayCommand]
        private async Task ToggleHeroType()
        {
            _currentIndex = (_currentIndex + 1) % _heroTypes.Count;
            CurrentHeroType = _heroTypes[_currentIndex];
            await Task.Run(LoadHeroes);
        }
    }
}
