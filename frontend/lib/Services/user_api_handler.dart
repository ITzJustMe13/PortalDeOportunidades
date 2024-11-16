import 'dart:convert';
import 'package:frontend/Models/Favorite.dart';
import 'package:frontend/Models/Impulse.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/User.dart';
import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class UserApiHandler {
  final String baseUri = "https://localhost:7235/api/User";
  final http.Client client;
  final storage = FlutterSecureStorage();

  UserApiHandler(this.client);

  // Login method
  Future<User?> login(String email, String password) async {
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
        final data = jsonDecode(response.body);
        await storage.write(key: 'accessToken', value: data['token']);
        final authenticatedUser = User.fromJson(data['authenticatedUserDTO']);
        return authenticatedUser;
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
  Future<User?> getUserByID(int id) async {
    final uri = Uri.parse('$baseUri/$id');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      print('Error: No access token found');
      return null;
    }

    try {
      final response = await client.get(uri, headers: {
        'Authorization': 'Bearer $accessToken',
      });

      if (response.statusCode == 200) {
        final user = User.fromJson(jsonDecode(response.body));
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

  // Delete User
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

  //Edit User
  Future<bool> editUser(int id, User updatedUser) async {
    final uri = Uri.parse('$baseUri/$id');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      print('Error: No access token found');
      return false;
    }

    try {
      final userJson = updatedUser.toJson();

      final response = await client.put(
        uri,
        headers: {
          'Authorization': 'Bearer $accessToken',
          'Content-Type': 'application/json',
        },
        body: jsonEncode(userJson),
      );

      if (response.statusCode == 200) {
        print('User updated successfully.');
        return true;
      } else if (response.statusCode == 404) {
        print('Error: User not found');
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

  //ADD FAVORITE
  Future<Favorite?> addFavorite(Favorite favorite) async {
    final uri = Uri.parse('$baseUri/favorite');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      print('Error: No access token found');
      return null;
    }

    try {
      final response = await client.post(
        uri,
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $accessToken',
        },
        body: jsonEncode(favorite.toJson()),
      );

      if (response.statusCode == 201) {
        final createdFavorite = Favorite.fromJson(jsonDecode(response.body));
        return createdFavorite;
      } else {
        print('Error: ${response.statusCode} - ${response.reasonPhrase}');
        return null;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return null;
    }
  }

  //Get FavoriteByID
  Future<Favorite?> getFavoriteByID(int userId, int opportunityId) async {
    final uri = Uri.parse('$baseUri/favorite/$userId/$opportunityId');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      print('Error: No access token found');
      return null;
    }

    try {
      final response = await client.get(uri, headers: {
        'Authorization': 'Bearer $accessToken',
      });

      if (response.statusCode == 200) {
        final favorite = Favorite.fromJson(jsonDecode(response.body));
        return favorite;
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

  //Get Favorites
  Future<List<Favorite>?> getFavorites(int userId) async {
    final uri = Uri.parse('$baseUri/favorites/$userId');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      print('Error: No access token found');
      return null;
    }

    try {
      final response = await client.get(uri, headers: {
        'Authorization': 'Bearer $accessToken',
        'Content-Type': 'application/json',
      });

      if (response.statusCode == 200) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        final favorites =
            jsonList.map((json) => Favorite.fromJson(json)).toList();
        return favorites;
      } else if (response.statusCode == 404) {
        print('No favorites found!');
        return [];
      } else {
        print('Error: ${response.statusCode} - ${response.reasonPhrase}');
        return null;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return null;
    }
  }

  // Impulse
  Future<Impulse?> impulseOpportunity(Impulse impulse) async {
    final uri = Uri.parse('$baseUri/impulse');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      print('Error: No access token found');
      return null;
    }

    try {
      final response = await client.get(uri, headers: {
        'Authorization': 'Bearer $accessToken',
        'Content-Type': 'application/json',
      });

      if (response.statusCode == 201) {
        final createdImpulse = Impulse.fromJson(jsonDecode(response.body));
        return createdImpulse;
      } else {
        print('Error: ${response.statusCode} - ${response.reasonPhrase}');
        return null;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return null;
    }
  }

  // get created Opp
  Future<List<Opportunity>?> getCreatedOpportunities(int userId) async {
    final uri = Uri.parse('$baseUri/created-opportunities/$userId');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      print('Error: No access token found');
      return null;
    }
    try {
      final response = await client.get(uri, headers: {
        'Authorization': 'Bearer $accessToken',
        'Content-Type': 'application/json',
      });

      if (response.statusCode == 200) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        final Opportunities =
            jsonList.map((json) => Opportunity.fromJson(json)).toList();
        return Opportunities;
      } else if (response.statusCode == 404) {
        print('No Opportunities found!');
        return [];
      } else {
        print('Error: ${response.statusCode} - ${response.reasonPhrase}');
        return null;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return null;
    }
  }
}
