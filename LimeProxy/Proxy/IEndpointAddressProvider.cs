using System.ServiceModel;

namespace LimeProxy.Proxy
{
    public interface IEndpointAddressProvider
    {
        EndpointAddress GetEndpointAddressForDataBase(string database);
    }

    class EndpointAddressProvider : IEndpointAddressProvider
    {
        private readonly ILimeConfigLookup _limeConfigLookup;

        public EndpointAddressProvider(ILimeConfigLookup limeConfigLookup)
        {
            _limeConfigLookup = limeConfigLookup;
        }

        public EndpointAddress GetEndpointAddressForDataBase(string database)
        {
            var url = _limeConfigLookup.GetUrlForDatabase(database);
            return new EndpointAddress(url);
        }
    }
}