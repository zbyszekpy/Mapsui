using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mapsui.Fetcher;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Utilities;

namespace Mapsui.Samples.Common.Maps.Demo
{
    public class WmsLayerAsync : BaseLayer
    {
        #region Fields

        private WmsProviderAsync DataSource { get; set; }
       
        private CancellationTokenSource CancellationTokenSource { get; set; }

        private readonly object _syncRoot = new object();
        private IFeature _wmsRaster;

        private IFeature WmsRaster
        {
            get
            {
                lock (_syncRoot)
                {
                    return _wmsRaster;
                }
            }
            set
            {
                lock (_syncRoot)
                {
                    _wmsRaster = value;
                }
            }
        }

        
        #endregion

        public WmsLayerAsync(string layername) : base(layername)
        {
            Enabled = true;
            CancellationTokenSource = new CancellationTokenSource();
            DataSource = new WmsProviderAsync();
        }

        public override void AbortFetch()
        {
            CancellationTokenSource.Cancel();
        }

        public override async void ViewChanged(bool majorChange, BoundingBox extent, double resolution)
        {
            if (!Enabled || DataSource == null || !majorChange)
                return;

            try
            {
                if (!CancellationTokenSource.IsCancellationRequested)
                    CancellationTokenSource.Cancel();

                CancellationTokenSource = new CancellationTokenSource();
                var data = await DataSource.GetFeaturesInView(extent, resolution, CancellationTokenSource.Token)
                    .ConfigureAwait(false);
                if (data == null)
                    return;

                // TODO Without the clone the extent is not updated on pan / pinch somewhere inside the APi => the feature is not tracked on the map
                //WmsRaster = ToFeature(data, extent);
                // TODO Works - but VERY hard to figure out :-)
                WmsRaster = ToFeature(data, extent.Clone());
                
                OnDataChanged(new DataChangedEventArgs(null, false, null, Name));
               
            }
            catch (Exception e)
            {
                Busy = false;
                // TODO catch task cancelled exception and retry on all other 
                // RetryOnFault(  
                //() => DownloadStringAsync(url), 3, () => Task.Delay(1000));
                // https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/consuming-the-task-based-asynchronous-pattern
                int a = 0;
            }
        }

        public async Task<T> RetryOnFault<T>(
            Func<Task<T>> function, int maxTries, Func<Task> retryWhen)
        {
            for (int i = 0; i < maxTries; i++)
            {
                try { return await function().ConfigureAwait(false); }
                catch { if (i == maxTries - 1) throw; }
                await retryWhen().ConfigureAwait(false);
            }
            return default(T);
        }

        private static IFeature ToFeature(byte[] data, BoundingBox bbox)
        {
            var feature =  new Feature(){ Geometry = new Raster(new MemoryStream(data), bbox)};

            // To debug in RasterRenderer
            feature["a"] = 1;
            //if (feature.Fields.Any())
            //{
            //    Debug.WriteLine(feature.Geometry.GetBoundingBox());
            //    Debug.WriteLine(destination.ToString());
            //}
            
            return feature;
        }

        public override void ClearCache()
        {
            WmsRaster = null;
        }

        public override bool? IsCrsSupported(string crs)
        {
            return true;
        }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            if (WmsRaster.Geometry == null)
                return new List<IFeature>();

            return box.Intersects(WmsRaster.Geometry.GetBoundingBox()) ? new List<IFeature>() { WmsRaster } : new List<IFeature>();
        }

        
        public override BoundingBox Envelope
        {
            get
            {
                if (DataSource == null) return null;

                lock (DataSource)
                {
                    return ProjectionHelper.GetTransformedBoundingBox(Transformation, DataSource.GetExtents(), DataSource.CRS, CRS);
                }
            }
        }
    }
}
