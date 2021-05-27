using Microsoft.Extensions.DependencyInjection;
using RfmUsb;

namespace RfmOpenThings
{
    public static class OpenThingsServiceExtensions
    {
        /// <summary>
        /// Add the <see cref="IOpenThingsService"/> dependency services to the <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the <see cref="IOpenThingsService"/> services</param>
        /// <returns></returns>
        public static IServiceCollection AddOpenThingsService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IOpenThingsService, OpenThingsService>();
            serviceCollection.AddRfmUsb();
            return serviceCollection;
        }
    }
}
