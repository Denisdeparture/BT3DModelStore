namespace BuisnesLogic.Models.SerializationModels
{
    public abstract class EndPoint
    {
        private string _container;
        public string Url
        {
            get
            {
                return _container;
            }
            set
            {
                if (!value.StartsWith('/')) throw new ArgumentException("string was begin from / symb");
                _container = value;
            }
        }
    }
}
