namespace SnowballSpin.Saving.Converters
{
    class IntConverter : ISaveValueTypeConverter
    {
        public object Convert(string value)
        {
            return int.Parse(value);
        }
    }
}
