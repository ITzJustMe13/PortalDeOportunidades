import 'dart:convert';
import 'package:http/http.dart' as http;

class UserApiHandler {
  final String baseUri = "https://localhost:7235/api/user";

  // You should initialize the http.Client in the constructor or elsewhere in your code.
  final http.Client client;

  UserApiHandler(this.client);

  // Get user by ID
  Future<Map<String, dynamic>?> getUserByID(int id) async {
    // Construct the full URL with the ID parameter
    final uri = Uri.parse('$baseUri/$id');

    // Perform the GET request
    final response = await client.get(uri);

    // Handle response
    if (response.statusCode == 200) {
      // Parse the response body (assuming it's in JSON format)
      final Map<String, dynamic> user = jsonDecode(response.body);
      return user; // Return the parsed user data
    } else if (response.statusCode == 404) {
      // Handle not found (User was not found or DB context missing)
      print('User not found or DB context missing');
      return null;
    } else {
      // Handle other error responses
      print('Error: ${response.statusCode}');
      return null;
    }
  }
}
