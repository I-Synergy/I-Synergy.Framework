using ISynergy.Core.Views.Library;
using ISynergy.Services;
using ISynergy.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.UI.Xaml.Controls.Maps;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ISynergy.Views.Library
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapsWindow : IMapsWindow
    {
        private MapsViewModel ViewModel => DataContext as MapsViewModel;

        public MapsWindow()
        {
            this.InitializeComponent();
            this.Loaded += MapsWindow_Loaded;
        }

        private async void MapsWindow_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.DataContext is MapsViewModel && this.DataContext != null)
            {
                MapsViewModel vm = this.DataContext as MapsViewModel;

                try
                {
                    await ActivatorUtilities.CreateInstance<IBusyService>(ViewModel.BaseService.ServiceProvider).StartBusyAsync();

                    Geopoint myLocation = new Geopoint(new BasicGeoposition { Latitude = 51.3774194, Longitude = 6.0791655 });
                    var MyLandmarks = new List<MapElement>();

                    var accessStatus = await Geolocator.RequestAccessAsync();

                    if (accessStatus == GeolocationAccessStatus.Allowed)
                    {
                        // Get the current location.
                        Geolocator geolocator = new Geolocator();
                        Geoposition pos = await geolocator.GetGeopositionAsync();
                        myLocation = pos.Coordinate.Point;

                        var myLocationNeedle = new MapIcon
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
                            var locationNeedle = new MapIcon
                            {
                                Location = location.Locations.FirstOrDefault().Point,
                                NormalizedAnchorPoint = new Point(0.5, 1.0),
                                ZIndex = 0,
                                Title = address
                            };

                            MyLandmarks.Add(locationNeedle);

                            var routeResult = await MapRouteFinder.GetDrivingRouteAsync(
                                myLocation,
                                location.Locations.FirstOrDefault().Point,
                                MapRouteOptimization.TimeWithTraffic,
                                MapRouteRestrictions.None);

                            if (routeResult.Status == MapRouteFinderStatus.Success)
                            {
                                vm.TimeToArrival = routeResult.Route.EstimatedDuration.TotalMinutes;
                                vm.DistcanceToArrival = routeResult.Route.LengthInMeters / 1000;

                                // Use the route to initialize a MapRouteView.
                                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);

                                // Add the new MapRouteView to the Routes collection
                                // of the MapControl.
                                BingMaps.Routes.Add(viewOfRoute);

                                // Fit the MapControl to the route.
                                await BingMaps.TrySetViewBoundsAsync(
                                      routeResult.Route.BoundingBox,
                                      null,
                                      Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Default);
                            }
                        }
                    }

                    var LandmarksLayer = new MapElementsLayer
                    {
                        ZIndex = 1,
                        MapElements = MyLandmarks
                    };

                    BingMaps.Layers.Add(LandmarksLayer);
                    BingMaps.LandmarksVisible = true;
                }
                finally
                {
                    await ActivatorUtilities.CreateInstance<IBusyService>(ViewModel.BaseService.ServiceProvider).EndBusyAsync();
                }
            }
        }
    }
}
