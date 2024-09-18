# MercatorProjection

The MercatorProjection class is a base class for implementing Mercator projections in geography. Its purpose is to provide a foundation for converting longitude and latitude coordinates on a globe to X and Y coordinates on a flat map, which is a common task in cartography and mapping applications.

This class doesn't take any direct inputs or produce outputs on its own. Instead, it sets up the structure for its derived classes to implement specific Mercator projection algorithms. It defines properties and methods that will be used by these derived classes to perform the actual coordinate conversions.

The class is initialized with a reference ellipsoid (referenceGlobe), which represents the shape of the Earth used for calculations. It also has a property called ReferenceMeridian, which is typically set to the Greenwich Meridian (0 degrees longitude) by default.

The main logic this base class provides is the setup for coordinate conversion methods (ToEuclidian and FromEuclidian) and distance calculation methods (EuclidianDistance and GeodesicDistance). These methods are declared as abstract, meaning they must be implemented by any class that inherits from MercatorProjection.

An important concept in this class is the handling of the reference meridian. The class ensures that the reference meridian is always normalized to be within the valid range of longitudes (-180 to 180 degrees). This is done through the NormalizeLongitude method, which is called whenever the ReferenceMeridian property is set.

The class also implements the IEquatable interface, allowing two MercatorProjection objects to be compared for equality based on their reference globes.

In summary, this MercatorProjection class serves as a blueprint for creating specific Mercator projection implementations. It provides the basic structure and common functionality that all Mercator projections would need, while leaving the specific projection algorithms to be implemented by derived classes.