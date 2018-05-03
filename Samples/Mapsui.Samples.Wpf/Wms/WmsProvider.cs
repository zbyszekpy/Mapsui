using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mapsui.Geometries;
using Mapsui.Providers;

namespace Mapsui.Samples.Common.Maps.Demo
{
    public class WmsProvider : IProvider
    {
        #region Fields

        private HttpClient HttpClient { get; set; }
        private CultureInfo engUs = new CultureInfo("en-US");
        #endregion

        public WmsProvider()
        {
            HttpClient = new HttpClient();
            CRS = "EPSG:3857";
        //    Apparently Core 2.0 stuff not in 1.1???
        //    ServicePointManager.DefaultConnectionLimit = 2;
        //    ServicePoint sp = ServicePointManager.FindServicePoint(new Uri("http://jordbrugsanalyser.dk"));
        //    sp.ConnectionLimit = 2;
        }

        

        /// <summary>
        /// Works ok since fast response
        /// </summary>
        //public IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        //{
        //    var view = new Viewport
        //    {
        //        Resolution = resolution,
        //        Center = box.GetCentroid(),
        //        Width = (box.Width / resolution),
        //        Height = (box.Height / resolution)
        //    };
        //    var url =
        //        $"https://geodata.nationaalgeoregister.nl/windkaart/wms?LAYERS=windsnelheden100m&TRANSPARENT=True&VERSION=1.3.0&SERVICE=WMS&REQUEST=GetMap&STYLES=&FORMAT=image/png&CRS={CRS}&BBOX={box.MinX.ToString(engUs)},{box.MinY.ToString(engUs)},{box.MaxX.ToString(engUs)},{box.MaxY.ToString(engUs)}&WIDTH={(int)view.Width}&HEIGHT={(int)view.Height}";
        //    var data = HttpClient.GetByteArrayAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();

        //    var features = new Features();
        //    IRaster raster = null;

        //    var feature = features.New();
        //    feature.Geometry = new Raster(new MemoryStream(data), box);
        //    features.Add(feature);

        //    return features;

        //}


        /// <summary>
        /// Very bad WMS - the worst I could find fo. Fetch delay does not help if user sendts a few requests (wait more that the default 1 sek) the maps locks up.
        /// </summary>
        public IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            var view = new Viewport
            {
                Resolution = resolution,
                Center = box.GetCentroid(),
                Width = (box.Width / resolution),
                Height = (box.Height / resolution)
            };
            var url =
                $"http://jordbrugsanalyser.dk/geoserver/ows?LAYERS=Marker12&TRANSPARENT=True&VERSION=1.3.0&SERVICE=WMS&REQUEST=GetMap&STYLES=&FORMAT=image/png&CRS={CRS}&BBOX={box.MinX.ToString(engUs)},{box.MinY.ToString(engUs)},{box.MaxX.ToString(engUs)},{box.MaxY.ToString(engUs)}&WIDTH={(int)view.Width}&HEIGHT={(int)view.Height}";
            var data = HttpClient.GetByteArrayAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();

            var features = new Features();
            var feature = features.New();
            feature.Geometry = new Raster(new MemoryStream(data), box);
            feature["a"] = "1";
            features.Add(feature);

            return features;

        }



        public BoundingBox GetExtents()
        {
            return new BoundingBox(new Point(362316.61053745885, 6574443.710755909), new Point(806427.3851695063, 7087262.516992419));
        }

        public string CRS { get; set; }

        public bool? IsCrsSupported(string crs)
        {
            return true;
        }
    }
}
