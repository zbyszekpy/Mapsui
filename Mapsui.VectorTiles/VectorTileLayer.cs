using System;
using System.Net;
using System.Net.Http;
using BruTile;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Rendering;

namespace Mapsui.VectorTiles
{
    public class VectorTileLayer : GenericTileLayer<VectorTileParser>
    {
        public VectorTileLayer(Func<ITileSource> tileSourceInitializer) : base(tileSourceInitializer) { }

        public VectorTileLayer(ITileSource source, IFetchStrategy fetchStrategy, ITileRenderStrategy tileRenderStrategy, int minTiles = 200, int maxTiles = 300, int maxRetries = 2) :
            base(source, minTiles, maxTiles, maxRetries, fetchStrategy, tileRenderStrategy)
        {
        }

        public static byte[] FetchTile(Uri url)
        {
            var gzipWebClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
            return gzipWebClient.GetByteArrayAsync(url).Result;
        }
    }
}