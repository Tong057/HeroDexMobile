using HeroDex.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroDex.Converters
{
    internal class HeroTypeToBudgeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is HeroType heroType)
            {
                return heroType switch
                {
                    HeroType.Hero => Colors.Green,
                    HeroType.Villain => Colors.Red,
                    HeroType.AntiHero => Colors.DimGray,
                    _ => Colors.Transparent // Default for undefined values
                };
            }

            return Colors.Transparent; // Default for invalid input
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
