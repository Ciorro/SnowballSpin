namespace SnowballSpin.Saving.Converters
{
    class BoolConverter : ISaveValueTypeConverter
    {
        public object Convert(string value)
        {
            return bool.Parse(value);
        }
    }
}
