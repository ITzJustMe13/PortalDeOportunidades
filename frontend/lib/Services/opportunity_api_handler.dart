import 'dart:convert';
import 'package:http/http.dart' as http;
import '../Models/Opportunity.dart';

class OpportunityApiHandler {
  final String baseUri = "https://localhost:7235/api/Opportunity";

  // You should initialize the http.Client in the constructor or elsewhere in your code.
  final http.Client client;

  OpportunityApiHandler(this.client);

  Future<Opportunity?> getOpportunityByID(int id) async {
    // Construct the full URL with the ID parameter
    final uri = Uri.parse('$baseUri/$id');
    Opportunity data;
    // Perform the GET request
    try {
      // Perform the GET request
      final response = await client.get(
        uri,
        headers: <String, String>{
          'Content-type': 'application/json; charset=UTF-8',
        },
      ).timeout(const Duration(seconds: 30));

      print("Response Status: ${response.statusCode}");
      print("Response Body: ${response.body}");

      if (response.statusCode == 200) {
        // Parse the response body (assuming it's in JSON format)
        final Map<String, dynamic> jsonData = jsonDecode(response.body);
        // Create an Opportunity object from the response JSON
        return Opportunity.fromJson(jsonData);
      } else if (response.statusCode == 404) {
        // Handle not found (User was not found or DB context missing)
        print('Opportunity not found or DB context missing');
        return null;
      } else {
        // Handle other error responses
        print('Error: ${response.statusCode}');
        return null;
      }
    } catch (e) {
      // Print the error for debugging purposes
      print('Exception occurred: $e');
      return null;
    }
  }
}
