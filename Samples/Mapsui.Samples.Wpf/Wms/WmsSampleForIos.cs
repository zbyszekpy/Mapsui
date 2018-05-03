using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapsui.Layers;
using Mapsui.Utilities;

namespace Mapsui.Samples.Common.Maps.Demo
{
    public static class WmsSampleForIos
    {
        public static Map CreateMap()
        {
            var map = new Map();

            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            map.Layers.Add(CreateLayer());
            
            return map;
        }

        public static Map CreateAsyncMap()
        {
            var map = new Map();
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            map.Layers.Add(CreateLayerAsync());

            return map;
        }

        public static ILayer CreateLayer()
        {
            return new ImageLayer("Sample") { DataSource = new WmsProvider() };
        }

        public static ILayer CreateLayerAsync()
        {
            return new WmsLayerAsync("Sample") ;
        }
    }
}
