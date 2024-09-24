namespace SnowballSpin.Saving.Converters
{
    class StringConverter : ISaveValueTypeConverter
    {
        public object Convert(string value)
        {
            return value;
        }
    }
}
