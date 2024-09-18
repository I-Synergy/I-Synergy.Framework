# GlobalCoordinates

The GlobalCoordinates struct is designed to represent and manage geographical coordinates on a globe, specifically latitude and longitude. Its purpose is to provide a convenient way to work with these coordinates while ensuring they stay within valid ranges.

This struct takes two inputs: latitude and longitude, both represented as Angle objects. These angles represent the position on the Earth's surface. Latitude measures the north-south position, while longitude measures the east-west position.

The struct doesn't produce direct outputs, but it provides methods to access, modify, and compare coordinates. It also ensures that the coordinates are always in a standardized format, which is crucial for consistent geographical calculations.

The main logic of this struct revolves around the concept of "canonicalization". This means that regardless of the input values, the struct will adjust them to fit within standard ranges: latitude between -90 and +90 degrees, and longitude between -180 and +180 degrees. This is important because, for example, a latitude of 100 degrees doesn't make sense in real-world terms and needs to be converted to its equivalent valid value.

The struct achieves its purpose through several key features:

- It implements IComparable and IEquatable interfaces, allowing coordinates to be compared and checked for equality.
- It stores latitude and longitude as private Angle objects, ensuring data encapsulation.
- The constructor takes latitude and longitude as inputs and immediately calls a method to canonicalize these values.

While the full canonicalization logic isn't shown in this excerpt, we can infer that it's an important part of the struct's functionality. This process likely involves mathematical operations to adjust out-of-range values to their correct equivalents within the standard coordinate system.

In summary, the GlobalCoordinates struct provides a robust way to handle geographical coordinates, ensuring they're always in a valid format and offering methods to work with them effectively in geographical calculations and comparisons.