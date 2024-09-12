# IProperty Interface

The IProperty interface, defined in the ISynergy.Framework.Core.Abstractions.Base namespace, serves as a blueprint for implementing property-related functionality in a class. This interface is designed to provide a standardized way of handling property changes, tracking modifications, and managing the state of properties.

The purpose of this code is to define a contract that classes can implement to gain property management capabilities. It doesn't take any direct inputs or produce outputs on its own, as it's an interface definition. Instead, it outlines methods and properties that implementing classes must provide.

The interface includes several key components:

- An event called ValueChanged, which can be triggered when a property's value is modified.
- A ResetChanges method, which allows reverting any changes made to the property.
- A MarkAsClean method, which can be used to indicate that the property's current state should be considered as the original or clean state.
- An IsDirty property, which indicates whether the property has been modified from its original state.
- An IsOriginalSet property, which likely indicates whether an original value has been set for the property.
- A Name property, which represents the name of the property.

The interface achieves its purpose by defining these members, which together provide a framework for tracking and managing property changes. When a class implements this interface, it gains the ability to notify listeners of value changes, reset modifications, mark the current state as clean, and check if the property has been modified.

The [JsonIgnore] attribute on some properties indicates that these should be excluded from JSON serialization, which is useful when converting objects to JSON format.

By using this interface, developers can create consistent property management across different classes in their application, enabling features like change tracking, undo functionality, or dirty checking in data-bound user interfaces.