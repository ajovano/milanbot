using MilanBotLib.ADO;
using System.Collections;
using System.Globalization;

namespace MilanBot.Utilities
{
    public class ADOWorkItemComparer : IComparer<ADOWorkItem>
    {
        public int Compare(ADOWorkItem x, ADOWorkItem y)
        {
            return x.ID.CompareTo(y.ID);
        }
    }

    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeStarted = (DateTime)value;
            var span = DateTime.Now - timeStarted;
            return span.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}