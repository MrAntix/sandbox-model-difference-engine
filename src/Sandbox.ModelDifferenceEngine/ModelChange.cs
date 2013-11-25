namespace Sandbox.ModelDifferenceEngine
{
    public class ModelChange
    {
        readonly string _path;
        readonly object _oldValue;
        readonly object _value;

        public ModelChange(string path, object oldValue, object value)
        {
            _path = path;
            _oldValue = oldValue;
            _value = value;
        }

        public string Path
        {
            get { return _path; }
        }

        public object OldValue
        {
            get { return _oldValue; }
        }

        public object Value
        {
            get { return _value; }
        }
    }
}