using web.ConfigExtension;

// this class return instance of custom configuration
public class DecryptingConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DecryptingConfigurationProvider(builder.Build());
    }
}