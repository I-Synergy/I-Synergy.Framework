using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.UI.Xaml;
using Maps = Windows.UI.Xaml.Controls.Maps;

namespace ISynergy.Framework.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapsWindow : IMapsWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapsWindow"/> class.
        /// </summary>
        public MapsWindow()
        {
            InitializeComponent();
            Loaded += MapsWindow_Loaded;
        }

        /// <summary>
        /// Handles the Loaded event of the MapsWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void MapsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MapsViewModel vm)
            {
                try
                {
                    await ServiceLocator.Default.GetInstance<IBusyService>().StartBusyAsync();

                    var bingMapsOptions = ServiceLocator.Default.GetInstance<IOptions<BingMapsOptions>>().Value;
                    var myLocation = new Geopoint(new BasicGeoposition { Latitude = 51.3774194, Longitude = 6.0791655 });
                    var MyLandmarks = new List<Maps.MapElement>();

                    var accessStatus = await Geolocator.RequestAccessAsync();

                    if (accessStatus == GeolocationAccessStatus.Allowed)
                    {
                        // Get the current location.
                        var geolocator = new Geolocator();
                        var pos = await geolocator.GetGeopositionAsync();
                        myLocation = pos.Coordinate.Point;

                        var myLocationNeedle = new Maps.MapIcon
                        {
                            Location = myLocation,
                            NormalizedAnchorPoint = new Point(0.5, 1.0),
                            ZIndex = 0,
                            Title = "My location"
                        };

                        MyLandmarks.Add(myLocationNeedle);

                        BingMaps.Center = myLocation;
                    }

                    var address = vm.Address;

                    if (address != null)
                    {
                        var location = await MapLocationFinder.FindLocationsAsync(
                            address,
                            myLocation,
                            1);

                        if (location.Status == MapLocationFinderStatus.Success)
                        {
                            var locationNeedle = new Maps.MapIcon
                            {
                                Location = location.Locations.FirstOrDefault()?.Point,
                                NormalizedAnchorPoint = new Point(0.5, 1.0),
                                ZIndex = 0,
                                Title = address
                            };

                            MyLandmarks.Add(locationNeedle);

                            var routeResult = await MapRouteFinder.GetDrivingRouteAsync(
                                myLocation,
                                location.Locations.FirstOrDefault()?.Point,
                                MapRouteOptimization.TimeWithTraffic,
                                MapRouteRestrictions.None);

                            if (routeResult.Status == MapRouteFinderStatus.Success)
                            {
                                vm.TimeToArrival = routeResult.Route.EstimatedDuration.TotalMinutes;
                                vm.DistcanceToArrival = routeResult.Route.LengthInMeters / 1000;

                                // Use the route to initialize a MapRouteView.
                                var viewOfRoute = new Maps.MapRouteView(routeResult.Route);

                                // Add the new MapRouteView to the Routes collection
                                // of the MapControl.
                                BingMaps.Routes.Add(viewOfRoute);

                                // Fit the MapControl to the route.
                                await BingMaps.TrySetViewBoundsAsync(
                                      routeResult.Route.BoundingBox,
                                      null,
                                      Maps.MapAnimationKind.Default);
                            }
                        }
                    }

                    var LandmarksLayer = new Maps.MapElementsLayer
                    {
                        ZIndex = 1,
                        MapElements = MyLandmarks
                    };

                    BingMaps.Layers.Add(LandmarksLayer);
                    BingMaps.LandmarksVisible = true;
                    BingMaps.MapServiceToken = bingMapsOptions.MapServiceToken;
                }
                finally
                {
                    await ServiceLocator.Default.GetInstance<IBusyService>().EndBusyAsync();
                }
            }
        }
    }
}