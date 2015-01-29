using System;
using System.Collections.Generic;
using System.Linq;
using SoundCloud.API.Client.Internal.Client;
using SoundCloud.API.Client.Internal.Infrastructure.Objects;
using SoundCloud.API.Client.Internal.Validation;
using SoundCloud.API.Client.Objects;
using SoundCloud.API.Client.Subresources.Helpers;

namespace SoundCloud.API.Client.Subresources
{
    public class TrackApi : ITrackApi
    {
        private readonly string trackId;
        private readonly ISoundCloudRawClient soundCloudRawClient;
        private readonly IPaginationValidator paginationValidator;
        private readonly string prefix;

        internal TrackApi(string trackId, ISoundCloudRawClient soundCloudRawClient, IPaginationValidator paginationValidator)
        {
            this.trackId = trackId;
            this.soundCloudRawClient = soundCloudRawClient;
            this.paginationValidator = paginationValidator;
            prefix = string.Format("tracks/{0}", trackId);
        }

        public SCTrack GetTrack()
        {
            return soundCloudRawClient.RequestApi<SCTrack>(prefix, string.Empty, HttpMethod.Get);
        }
        
        public void UpdateTrack(SCTrack track)
        {
            if (track.Id != trackId)
            {
                throw new SoundCloudApiException(string.Format("Context set for trackId = {0}. Create new context for update another track.", trackId));
            }

            var currentTrack = GetTrack();

            var diff = currentTrack.GetDiff(track);

            soundCloudRawClient.RequestApi(prefix, string.Empty, HttpMethod.Put, diff.ToDictionary(x => string.Format((string) "track[{0}]", (object) x.Key), x => x.Value));
        }

        public void DeleteTrack()
        {
            soundCloudRawClient.RequestApi(prefix, string.Empty, HttpMethod.Delete);
        }

        public SCComment[] GetComments(int offset = 0, int limit = 50)
        {
            return soundCloudRawClient.GetCollection<SCComment>(paginationValidator, prefix, "comments", offset, limit);
        }

        public SCComment GetComment(string commentId)
        {
            return soundCloudRawClient.RequestApi<SCComment>(prefix, string.Format("comments/{0}", commentId), HttpMethod.Get);
        }

        public void PostComment(string text, TimeSpan? timestamp)
        {
            var parameters = new Dictionary<string, object> { { "comment[body]", text } };
            if (timestamp.HasValue)
            {
                parameters.Add("comment[timestamp]", timestamp.Value.TotalMilliseconds);
            }

            soundCloudRawClient.RequestApi(prefix, "comments", HttpMethod.Post, parameters);
        }

        public void DeleteComment(string commentId)
        {
            soundCloudRawClient.RequestApi<SCComment>(prefix, string.Format("comments/{0}", commentId), HttpMethod.Delete);
        }

        public SCUser[] GetFavoriters(int offset = 0, int limit = 50)
        {
            return soundCloudRawClient.GetCollection<SCUser>(paginationValidator, prefix, "favoriters", offset, limit);
        }

        public SCUser GetFavoriter(string favoriterId)
        {
            return soundCloudRawClient.RequestApi<SCUser>(prefix, string.Format("favoriters/{0}", favoriterId), HttpMethod.Get);
        }
    }
}