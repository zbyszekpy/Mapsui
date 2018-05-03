using Mapsui.Layers;
using Mapsui.Providers.Wms;
using Mapsui.Utilities;

namespace Mapsui.Samples.Common.Desktop
{
    public static class WmsSample
    {
        public static Map CreateMap()
        {
            var map = new Map();// {CRS = "EPSG:28992"};
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            // The WMS request needs a CRS
            map.Layers.Add(CreateLayer());
            return map;
        }

        public static ILayer CreateLayer()
        {
            return new ImageLayer("Windsnelheden (PDOK)") {DataSource = CreateWmsProvider()};
        }

        private static WmsProvider CreateWmsProvider()
        {
            //const string wmsUrl = "https://geodata.nationaalgeoregister.nl/windkaart/wms?request=GetCapabilities";

            //const string wmsUrl =
            //    $"http://jordbrugsanalyser.dk/geoserver/ows?LAYERS=Marker12&TRANSPARENT=True&VERSION=1.3.0&SERVICE=WMS&REQUEST=GetMap&STYLES=&FORMAT=image/png&CRS={CRS}&BBOX={box.MinX.ToString(engUs)},{box.MinY.ToString(engUs)},{box.MaxX.ToString(engUs)},{box.MaxY.ToString(engUs)}&WIDTH={(int)view.Width}&HEIGHT={(int)view.Height}";

            const string wmsUrl = "http://jordbrugsanalyser.dk/geoserver/ows?request=GetCapabilities&service=wms";
            var provider = new WmsProvider(wmsUrl)
            {
                ContinueOnError = true,
                TimeOut = 20000,
                CRS = "EPSG:3857"
            };

            provider.AddLayer("Jordbrugsanalyser:Marker12");
            provider.SetImageFormat(provider.OutputFormats[0]);
            return provider;
        }
    }
}