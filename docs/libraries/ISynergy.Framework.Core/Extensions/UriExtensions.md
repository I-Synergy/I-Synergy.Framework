# UriExtensions.cs

This code defines a static class called UriExtensions, which contains a method to add or update query parameters in a URI (Uniform Resource Identifier, commonly known as a web address).

The main purpose of this code is to provide a convenient way to manipulate URIs by adding or modifying query parameters. Query parameters are the part of a URL that comes after the question mark (?) and are used to pass information to a web server.

The method, named AddQueryParameter, takes three inputs:

- A Uri object (the original URI to modify)
- A string called "name" (the name of the query parameter to add or update)
- A string called "value" (the value to assign to the query parameter)

The output of this method is a new Uri object with the updated query parameter.

Here's how the method achieves its purpose:

- It first parses the existing query string from the input URI.
- If the given parameter name already exists in the query string, it removes it to avoid duplication.
- It then adds the new parameter with the provided name and value.
- Next, it creates a UriBuilder object to reconstruct the URI.
- If there are no query parameters after the addition, it sets the query string to empty.
- If there are parameters, it builds a new query string by iterating through all parameters, encoding them properly, and joining them with '&' symbols.
- Finally, it sets the new query string to the UriBuilder and returns the resulting Uri.

An important aspect of this code is how it handles the encoding of query parameters. It uses HttpUtility.UrlEncode to ensure that special characters in parameter names and values are properly encoded, which is crucial for creating valid URLs.

This method is particularly useful when working with web applications or APIs where you need to dynamically modify URLs by adding or updating query parameters. It simplifies the process of manipulating URIs while ensuring that the resulting URL is properly formatted and encoded.