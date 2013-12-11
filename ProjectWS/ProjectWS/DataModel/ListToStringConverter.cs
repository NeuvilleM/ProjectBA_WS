using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ProjectWS.DataModel
{
    //[ValueConversion(typeof(List<string>), typeof(string))]
    class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            List<String> ss = value as List<string>;
            string sol = "";
            foreach (string s in ss)
            {
                sol += s;
            }
            return sol;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
