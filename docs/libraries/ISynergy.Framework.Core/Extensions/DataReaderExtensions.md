# DataReaderExtensions.cs

This code defines a static class called DataReaderExtensions, which contains a method named MapToList. The purpose of this method is to convert data from a database (read through an IDataReader) into a list of objects of a specified type.

The MapToList method takes one input: an IDataReader object, which is typically used to read data from a database. It also uses a generic type parameter T, which represents the type of objects that will be created and added to the list.

The output of this method is a List, where T is the type specified when calling the method. This list contains objects created from the data in the IDataReader, with each object representing a row from the database.

Here's how the method achieves its purpose:

- It creates an empty list to store the objects.
- It enters a loop that continues as long as there's data to read from the IDataReader.
- For each row of data, it creates a new object of type T.
- It then goes through each property of the newly created object and tries to match it with a column from the database.
- If a match is found and the database value isn't null, it sets the property value on the object.
- After setting all the matching properties, it adds the object to the list.
- Once all rows have been processed, it returns the completed list.

An important part of the logic is how it handles different types of properties. If a property is an enum type, it sets the value directly. For other types, it converts the database value to the property's type before setting it.

This method is useful because it automates the process of converting database rows into objects, saving programmers from having to write this conversion code manually for each type of object they want to create from database data.