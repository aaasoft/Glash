namespace GlashClientDesktop.Converters
{
    public class ObjectToBoolConverter : ObjectNullToTConverter<bool>
    {
        public ObjectToBoolConverter()
        {
            NullValue = false;
            NotNullValue = true;
        }
    }
}
