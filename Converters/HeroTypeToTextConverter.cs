using HeroDex.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroDex.Converters
{
    public class HeroTypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                null => "All supers",         // Для "Все"
                HeroType.Hero => "Heroes",    // Для Героя
                HeroType.Villain => "Villains", // Для Злодея
                HeroType.AntiHero => "AntiHeroes", // Для Антигероя
                _ => "Unknown"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
