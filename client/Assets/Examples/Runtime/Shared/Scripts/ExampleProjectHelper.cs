using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Api.Stats;
using Beamable.Common.Api;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using Beamable.UI.Scripts;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Beamable.Examples.Shared
{
    /// <summary>
    /// Useful, reusable functionality
    /// for use in this example project
    /// </summary>
    public static class ExampleProjectHelper
    {

        //  Methods  --------------------------------------
        
        /// <summary>
        /// Get UI-friendly display name text for a content id
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public static string GetDisplayNameFromContentId(string contentId)
        {
            //Change "items.foo.Blah" to "Blah"
            var tokens = contentId.Split('.');
            return tokens[tokens.Length - 1];
        }
        
        /// <summary>
        /// Get a public stat using the <see cref="StatsService"/>
        /// </summary>
        public static async Task<string> GetPublicPlayerStat(IBeamableAPI beamableAPI, string statKey)
        {
            string domain = "game";
            string access = "public";
            string type = "player";
            long id = beamableAPI.User.id;

            Dictionary<string, string> getStats = new Dictionary<string, string>();
            // Get Value
            try
            {
                getStats = await beamableAPI.StatsService.GetStats(domain, access, type, id);
            }
            catch (Exception e)
            {
                Debug.Log("error: " + e.Message);
            }
    

            string value = "";
            if (getStats.ContainsKey(statKey))
            {
                getStats.TryGetValue(statKey, out value);
            }

            return value;
        }
        
        /// <summary>
        /// Set a public stat using the <see cref="StatsService"/>
        /// </summary>
        public static async Task<EmptyResponse> SetPublicPlayerStat(IBeamableAPI beamableAPI, string statKey, string statValue)
        {
            string access = "public";
            long id = beamableAPI.User.id;

            Dictionary<string, string> setStats =
                new Dictionary<string, string>() {{statKey, statValue}};

            await beamableAPI.StatsService.SetStats(access, setStats);

            return new EmptyResponse();
        }

        /// <summary>
        /// Determines if incoming content matches a given type
        /// </summary>
        /// <param name="clientContentInfo"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static bool IsMatchingClientContentInfo(ClientContentInfo clientContentInfo, string contentType)
        {
            return clientContentInfo.contentId.ToLower().Contains(contentType.ToLower());
        }
        
        
        /// <summary>
        /// Load full details of a Beamable Item
        /// </summary>
        /// <param name="beamContext"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<ItemContent> GetItemContentById(BeamContext beamContext, string id)
        {
            return await beamContext.Api.ContentService.GetContent(id, typeof(ItemContent)) as ItemContent;
        }
        
        /// <summary>
        /// Load full details of a Beamable Currency
        /// </summary>
        /// <param name="beamContext"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<CurrencyContent> GetCurrencyContentById(BeamContext beamContext, string id)
        {
            return await beamContext.Api.ContentService.GetContent(id, typeof(CurrencyContent)) as CurrencyContent;
        }

        /// <summary>
        /// Lazily load a AssetReferenceTexture2D into an Image
        /// </summary>
        /// <param name="assetReferenceTexture2D"></param>
        /// <param name="destinationImage"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddressablesLoadAssetAsync<T>(AssetReferenceTexture2D assetReferenceTexture2D, Image destinationImage)
        {
            AsyncOperationHandle<Texture2D> asyncOperationHandle1 = Addressables.LoadAssetAsync<Texture2D>(
               assetReferenceTexture2D);

            asyncOperationHandle1.Completed += (AsyncOperationHandle<Texture2D> asyncOperationHandle2) =>
            {
                Texture2D texture2D = asyncOperationHandle2.Result;
                destinationImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
                new Vector2(0.5f, 0.5f));

            };
        }

        /// <summary>
        /// Lazily Load an addressable <see cref="Sprite"/> into a UI <see cref="Image"/>.
        /// </summary>
        /// <param name="assetReferenceSprite"></param>
        /// <param name="destinationImage"></param>
        /// <typeparam name="T"></typeparam>
        public static async void AddressablesLoadAssetAsync<T>(AssetReferenceSprite assetReferenceSprite, Image destinationImage)
        {
            // Check before await
            if (destinationImage == null || assetReferenceSprite == null)
            {
                return;
            }

            // Hide it
            Sprite sprite = await AddressableSpriteLoader.LoadSprite(assetReferenceSprite);

            // Check after await
            if (destinationImage == null || assetReferenceSprite == null)
            {
                return;
            }

            if (sprite != null)
            {
                destinationImage.sprite = sprite;
                destinationImage.preserveAspect = true;
            }
        }
    }
}