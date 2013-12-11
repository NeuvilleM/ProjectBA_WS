using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ProjectWS.DataModel
{
    class LinupToStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //30/11/2013 0:00:00
            string date = value as string;
            char[] chars = date.ToCharArray();
            string returndate = chars[0].ToString() + chars[1].ToString() + "/";
            returndate += chars[3].ToString() + chars[4].ToString()+"/";
            returndate += chars[6].ToString() + chars[7].ToString();
            return returndate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
