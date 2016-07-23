using Microsoft.Extensions.Configuration;

namespace ConfigFromAnywhere.Configuration
{
    public static class TwitterConfigurationExtensions
    {
        public static IConfigurationBuilder AddTwitter(
            this IConfigurationBuilder configurationBuilder,
            string consumerKey,
            string consumerSecret,
            string userAccessToken,
            string userAccessSecret,
            string hashTag)
        {
            return configurationBuilder.Add(
                new TwitterConfigurationSource(consumerKey, consumerSecret, userAccessToken, userAccessSecret, hashTag));
        }
    }
}
