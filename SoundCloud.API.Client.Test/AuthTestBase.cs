﻿using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using SoundCloud.API.Client.Internal.Infrastructure.Serialization;

namespace SoundCloud.API.Client.Test
{
    public abstract class AuthTestBase : TestBase
    {
        protected ISoundCloudClient soundCloudClient;
        protected Settings settings;

        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            var settingsJson = File.ReadAllText(Environment.CurrentDirectory + @"\settings.json", Encoding.UTF8);
            settings = JsonSerializer.Default.Deserialize<Settings>(settingsJson);

            ISoundCloudConnector soundCloudConnector = new SoundCloudConnector();
            soundCloudClient = soundCloudConnector.DirectConnect(settings.ClientId, settings.ClientSecret, settings.UserName, settings.Password);
        }
        
        protected class Settings
        {
            public string ClientId { get; set; } 
            public string ClientSecret { get; set; } 
            public string UserName { get; set; } 
            public string Password { get; set; }
            public string TestUserId { get; set; }
        }

        protected static void TestCollection<TResponse>(Func<int, int, TResponse[]> getResponse, int offset, int limit)
        {
            var entities = getResponse(offset, limit);
            Assert.IsTrue(entities.Length >= 0 && entities.Length <= limit);

            if (entities.Length < limit)
                return;

            entities = getResponse(limit, limit);
            Assert.IsTrue(entities.Length >= 0 && entities.Length <= limit);
        }

        protected static void TestDeleteAndPutEntity<TResponse>(Func<int, int, TResponse[]> selector, Func<TResponse, string> idGetter, Action<string> put, Action<string> delete)
        {
            var entities = selector(0, 1);
            Assert.IsTrue(entities.Length > 0, "Selector returned empty collection");

            var entity = entities[0];
            var expectedEntityId = idGetter(entity);
            delete(expectedEntityId);
            entities = selector(0, 1);
            Assert.IsTrue(entities.Length == 0 || idGetter(entities[0]) != expectedEntityId);

            put(expectedEntityId);
            entities = selector(0, 1);

            Assert.AreEqual(expectedEntityId, idGetter(entities[0]));
        }

        protected static void TestGetEntity<TResponse>(Func<int, int, TResponse[]> multiSelector, Func<string, TResponse> selector, Func<TResponse, string> idGetter)
        {
            var entities = multiSelector(0, 1);
            Assert.IsTrue(entities.Length > 0, "Selector returned empty collection");

            var entity = entities[0];
            var expectedId = idGetter(entity);
            var actual = selector(expectedId);

            Assert.AreEqual(expectedId, idGetter(actual));
        }
    }
}