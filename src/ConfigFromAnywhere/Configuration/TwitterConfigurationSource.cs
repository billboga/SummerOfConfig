using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tweetinvi;

namespace ConfigFromAnywhere.Configuration
{
    public class TwitterConfigurationSource : IConfigurationSource
    {
        public TwitterConfigurationSource(
            string consumerKey,
            string consumerSecret,
            string userAccessToken,
            string userAccessSecret,
            string hashTag)
        {
            if (string.IsNullOrEmpty(consumerKey))
            {
                throw new ArgumentNullException(nameof(consumerKey));
            }

            if (string.IsNullOrEmpty(consumerSecret))
            {
                throw new ArgumentNullException(nameof(consumerSecret));
            }

            if (string.IsNullOrEmpty(userAccessToken))
            {
                throw new ArgumentNullException(nameof(userAccessToken));
            }

            if (string.IsNullOrEmpty(userAccessSecret))
            {
                throw new ArgumentNullException(nameof(userAccessSecret));
            }

            if (string.IsNullOrEmpty(hashTag))
            {
                throw new ArgumentNullException(nameof(hashTag));
            }

            if (!hashTag.StartsWith("#"))
            {
                throw new FormatException($"`{nameof(hashTag)}` must start with `#`. It's a Twitter hash tag.");
            }

            SetupTwitterStream(consumerKey, consumerSecret, userAccessToken, userAccessSecret, hashTag);
        }

        private IDictionary<string, string> data = new Dictionary<string, string>();

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new TwitterConfigurationProvider(data);
        }

        private void SetupTwitterStream(
            string consumerKey,
            string consumerSecret,
            string userAccessToken,
            string userAccessSecret,
            string hashTag)
        {
            Auth.SetUserCredentials(consumerKey, consumerSecret, userAccessToken, userAccessSecret);

            var user = User.GetAuthenticatedUser();

            if (user == null)
            {
                throw new InvalidOperationException("Check Twitter credentials. They do not appear valid.");
            }
            else
            {
                var stream = Stream.CreateFilteredStream();

                stream.AddTrack(hashTag);

                // We want the stream to only contain the current user's Tweets.
                stream.AddFollow(user);

                stream.MatchingTweetReceived += (sender, args) =>
                {
                    // Get the whole message sans hash tag.
                    var unParsedConfigurations = args.Tweet.FullText;
                    var hashTagIndex = unParsedConfigurations.IndexOf(hashTag, StringComparison.InvariantCultureIgnoreCase);

                    unParsedConfigurations = unParsedConfigurations.Remove(hashTagIndex, hashTag.Length)
                        .Replace(hashTag, "")
                        .Trim();

                    foreach (var unParsedConfiguration in unParsedConfigurations.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        /**
                         * Matches anything to the left and right of the equals-sign as long as
                         * there are no spaces between the equals-sign.
                         * `test=value` works, but `test= value`, `test =value`, and `test = value` do not.
                         */
                        var tokenMatch = Regex.Match(unParsedConfiguration, "^([^=]+)=([^ ].*)$");

                        if (tokenMatch.Success)
                        {
                            data[tokenMatch.Groups[1].Value] = tokenMatch.Groups[2].Value;
                        }
                    }
                };

                stream.StartStreamMatchingAllConditionsAsync();
            }
        }
    }
}
