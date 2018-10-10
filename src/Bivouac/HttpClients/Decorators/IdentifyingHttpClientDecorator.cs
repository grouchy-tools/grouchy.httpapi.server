using Bivouac.Abstractions;
using Bivouac.Extensions;
using Burble.Abstractions;

namespace Bivouac.HttpClients.Decorators
{
    public class IdentifyingHttpClientDecorator : IHttpClientDecorator
    {
        private readonly IGetCorrelationId _correlationIdGetter;
        private readonly IGenerateGuids _guidGenerator;
        private readonly IGetServiceName _serviceNameGetter;
        private readonly IGetServiceVersion _serviceVersionGetter;

        public IdentifyingHttpClientDecorator(
            IGetCorrelationId correlationIdGetter,
            IGenerateGuids guidGenerator,
            IGetServiceName serviceNameGetter,
            IGetServiceVersion serviceVersionGetter)
        {
            _correlationIdGetter = correlationIdGetter;
            _guidGenerator = guidGenerator;
            _serviceNameGetter = serviceNameGetter;
            _serviceVersionGetter = serviceVersionGetter;
        }

        public IHttpClient Decorate(IHttpClient httpClient)
        {
            // TODO: Need a better environment
            return httpClient.AddIdentifyingHeaders(_correlationIdGetter, _guidGenerator, _serviceNameGetter, _serviceVersionGetter);
        }
    }
}