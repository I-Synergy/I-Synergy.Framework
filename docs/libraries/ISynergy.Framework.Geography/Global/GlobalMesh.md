# GlobalMesh

The GlobalMesh class is designed to create a grid-like structure that covers the Earth's surface. Its purpose is to divide the globe into smaller, manageable squares, which can be useful for various geographic calculations and analyses.

This class doesn't take any direct inputs when it's created, but it has a constructor that allows you to specify the size of each square in the mesh (in meters). If you don't specify a size, it defaults to 1000 meters (1 kilometer).

The main output of this class is a system for identifying and working with these square areas on the Earth's surface. It doesn't produce a physical map or image, but rather provides a way to reference and work with these grid squares programmatically.

To achieve its purpose, the GlobalMesh class uses a geographic system called UTM (Universal Transverse Mercator). UTM divides the Earth into 60 zones, and the GlobalMesh further subdivides these zones into smaller squares. The class ensures that it can handle the entire globe from 80° South to 84° North latitude.

An important aspect of the class is that it sets a minimum size for the mesh squares (1 meter) to prevent impractical or impossibly small divisions. It also calculates the maximum number of vertical meshes needed to cover any UTM grid, which helps in determining the total number of mesh squares and their positions.

The class is designed to be flexible, allowing for different mesh sizes while maintaining efficiency in calculations. It provides a foundation for more complex geographic operations that might need to work with standardized areas on the Earth's surface.