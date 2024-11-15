import 'dart:convert';
import 'package:frontend/Models/User.dart';
import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class UserApiHandler {
  final String baseUri = "https://localhost:7235/api/User";
  final http.Client client;
  final storage = FlutterSecureStorage();

  UserApiHandler(this.client);

  // Login method
  Future<Map<String, dynamic>?> login(String email, String password) async {
    final uri = Uri.parse('$baseUri/login');

    try {
      final response = await client.post(
        uri,
        headers: {
          'Content-Type': 'application/json',
        },
        body: jsonEncode({
          'email': email,
          'password': password,
        }),
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      } else if (response.statusCode == 401) {
        print('Unauthorized: ${response.body}');
        return null;
      } else {
        print('Error: ${response.statusCode} - ${response.reasonPhrase}');
        return null;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return null;
    }
  }

  /// Get user by ID
  Future<Map<String, dynamic>?> getUserByID(int id) async {
    final uri = Uri.parse('$baseUri/$id');

    try {
      final response = await client.get(uri);

      if (response.statusCode == 200) {
        final user = jsonDecode(response.body) as Map<String, dynamic>;
        return user;
      } else if (response.statusCode == 404) {
        print('User not found or DB context missing');
        return null;
      } else {
        print('Error: ${response.statusCode} - ${response.reasonPhrase}');
        return null;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return null;
    }
  }

  //Create New User
  Future<User?> createUser(User user) async {
    final uri = Uri.parse(baseUri);

    try {
      final response = await client.post(
        uri,
        headers: {
          'Content-Type': 'application/json',
        },
        body: jsonEncode(user.toJson()),
      );

      if (response.statusCode == 201) {
        final createdUser = User.fromJson(jsonDecode(response.body));
        return createdUser;
      } else {
        print('Error: ${response.statusCode} - ${response.reasonPhrase}');
        return null;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return null;
    }
  }

  Future<bool> deleteUser(int id) async {
    final uri = Uri.parse('$baseUri/$id');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      print('Error: No access token found');
      return false;
    }

    try {
      final response = await client.delete(
        uri,
        headers: {
          'Authorization': 'Bearer $accessToken',
        },
      );

      if (response.statusCode == 204) {
        print('User deleted successfully.');
        return true;
      } else if (response.statusCode == 404) {
        print('Error: ${response.body}');
        return false;
      } else {
        print('Error: ${response.statusCode} - ${response.reasonPhrase}');
        return false;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return false;
    }
  }
}
