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
    public class WmsProviderAsync 
    {
        #region Fields

        private HttpClient HttpClient { get; set; }
        private CultureInfo engUs = new CultureInfo("en-US");
        #endregion

        public WmsProviderAsync()
        {
            HttpClient = new HttpClient();
            CRS = "EPSG:3857";
        //    Apparently Core 2.0 stuff not in 1.1???
        //    ServicePointManager.DefaultConnectionLimit = 20;
        //    Test
        //    ServicePoint sp = ServicePointManager.FindServicePoint(new Uri("http://jordbrugsanalyser.dk"));
        //    sp.ConnectionLimit = 20;

            // 
        }


        //public async Task<byte[]> GetFeaturesInView(BoundingBox box, double resolution,
        //    CancellationToken cancellationToken)
        //{
        //    var view = new Viewport { Resolution = resolution, Center = box.GetCentroid(), Width = (box.Width / resolution), Height = (box.Height / resolution) };
        //    if (view.Height < 1 || view.Width < 1)
        //        return null;

        //    var url =
        //        $"https://geodata.nationaalgeoregister.nl/windkaart/wms?LAYERS=windsnelheden100m&TRANSPARENT=true&VERSION=1.3.0&SERVICE=WMS&REQUEST=GetMap&STYLES=&FORMAT=image/png&CRS={CRS}&BBOX={box.MinX.ToString(engUs)},{box.MinY.ToString(engUs)},{box.MaxX.ToString(engUs)},{box.MaxY.ToString(engUs)}&WIDTH={(int)view.Width}&HEIGHT={(int)view.Height}";
        //    var response = await HttpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        //    var responseBody = await response.Content.ReadAsByteArrayAsync();
        //    return responseBody;
        //}

        /// <summary>
        /// Very bad WMS - the worst I could find fo. Fetch delay does not help if user sendts a few requests (wait more that the default 1 sek) the maps locks up.
        /// </summary>
        public async Task<byte[]> GetFeaturesInView(BoundingBox box, double resolution,
            CancellationToken cancellationToken)
        {
            var view = new Viewport { Resolution = resolution, Center = box.GetCentroid(), Width = (box.Width / resolution), Height = (box.Height / resolution) };
            if (view.Height < 1 || view.Width < 1)
                return null;

            var url =
                $"http://jordbrugsanalyser.dk/geoserver/ows?LAYERS=Marker12&TRANSPARENT=True&VERSION=1.3.0&SERVICE=WMS&REQUEST=GetMap&STYLES=&FORMAT=image/png&CRS={CRS}&BBOX={box.MinX.ToString(engUs)},{box.MinY.ToString(engUs)},{box.MaxX.ToString(engUs)},{box.MaxY.ToString(engUs)}&WIDTH={(int)view.Width}&HEIGHT={(int)view.Height}";
            var response = await HttpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsByteArrayAsync();
            return responseBody;
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
