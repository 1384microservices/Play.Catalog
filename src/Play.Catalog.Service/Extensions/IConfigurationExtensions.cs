using Play.Catalog.Service.Settings;

namespace Play.Catalog.Service.Extensions;

public static class IConfigurationExtensions
{
    public static T? GetSection<T>(this IConfiguration configuration)
    {
        return configuration.GetSection(typeof(T).Name).Get<T>();
    }

    public static ServiceSettings? GetServiceSettings(this IConfiguration configuration)
    {
        return configuration.GetSection<ServiceSettings>();
    }

    public static MongoDbSettings? GetMongoDbSettings(this IConfiguration configuration)
    {
        return configuration.GetSection<MongoDbSettings>();
    }


}
