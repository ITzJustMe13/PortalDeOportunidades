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
  

  void logout() {
    storage.delete(key: 'accessToken');
    storage.delete(key: 'currentUser');
  }

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

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final data = jsonDecode(response.body);
        await storage.write(key: 'accessToken', value: data['token']);
        final authenticatedUser = User.fromJson(data['user']);
        await storage.write(
            key: 'currentUser', value: authenticatedUser.userId.toString());
        return authenticatedUser;
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  Future<User?> getStoredUser() async {
    final String? storedUser = await storage.read(key: 'currentUser');

    try {
      if (storedUser != null && int.parse(storedUser) != -1) {
        User? user = await getUserByID(int.parse(storedUser));
        if (user != null) {
          return user;
        }
      }
      return null;
    } catch (e) {
      return null;
    }
  }

  Future<int> getStoredUserID() async {
    final String? storedUser = await storage.read(key: 'currentUser');

    try {
      if (storedUser != null) {
        int? userID = int.parse(storedUser);
        return userID;
      }
      return -1;
    } catch (e) {
      return -1;
    }
  }

  /// Get user by ID
  Future<User?> getUserByID(int id) async {
    final uri = Uri.parse('$baseUri/$id');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      return null;
    }

    try {
      final response = await client.get(uri, headers: {
        'Authorization': 'Bearer $accessToken',
      });

      if (response!.statusCode >= 200 && response.statusCode <= 299) {
        final user = User.fromJson(jsonDecode(response.body));
        return user;
      } else if (response.statusCode == 404) {
        return null;
      } else {
        return null;
      }
    } catch (e) {
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

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final createdUser = User.fromJson(jsonDecode(response.body));
        return createdUser;
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  // Delete User
  Future<bool> deleteUser(int id) async {
    final uri = Uri.parse('$baseUri/$id');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      return false;
    }

    try {
      final response = await client.delete(
        uri,
        headers: {
          'Authorization': 'Bearer $accessToken',
        },
      );

      if (response!.statusCode >= 200 && response.statusCode <= 299) {
        return true;
      } else if (response.statusCode == 404) {
        return false;
      } else {
        return false;
      }
    } catch (e) {
      return false;
    }
  }

  //Edit User
  Future<bool> editUser(int id, User updatedUser) async {
    final uri = Uri.parse('$baseUri/$id');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
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

      if (response!.statusCode >= 200 && response.statusCode <= 299) {
        return true;
      } else if (response.statusCode == 404) {
        return false;
      } else {
        return false;
      }
    } catch (e) {
      return false;
    }
  }

  //ADD FAVORITE
  Future<Favorite?> addFavorite(Favorite favorite) async {
    final uri = Uri.parse('$baseUri/favorite');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
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

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final createdFavorite = Favorite.fromJson(jsonDecode(response.body));
        return createdFavorite;
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  //Get FavoriteByID
  Future<Favorite?> getFavoriteByID(int userId, int opportunityId) async {
    final uri = Uri.parse('$baseUri/favorite/$userId/$opportunityId');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      return null;
    }

    try {
      final response = await client.get(uri, headers: {
        'Authorization': 'Bearer $accessToken',
      });

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final favorite = Favorite.fromJson(jsonDecode(response.body));
        return favorite;
      } else if (response.statusCode == 404) {
        return null;
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  //Get Favorites
  Future<List<Favorite>?> getFavorites(int userId) async {
    final uri = Uri.parse('$baseUri/favorites/$userId');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      return null;
    }

    try {
      final response = await client.get(uri, headers: {
        'Authorization': 'Bearer $accessToken',
        'Content-Type': 'application/json',
      });

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        final favorites =
            jsonList.map((json) => Favorite.fromJson(json)).toList();
        return favorites;
      } else if (response.statusCode == 404) {
        return [];
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  /// Documentation for deleteFavoriteById
  ///Endpoint that sends a delete Favorite request
  /// @param: userId : id of the user
  /// @param oppId : io of the opportunity
  /// @returns: true if it was delete sucessefully, false if not
  Future<bool> deleteFavoriteById(int userId, int oppId) async{
    final uri = Uri.parse('$baseUri/favorite/$userId/$oppId/delete');
    final String? accessToken = await storage.read(key: 'accessToken');
    
    if (accessToken == null) {
      return false;
    }

    try {
      final response = await client.delete(uri, headers: {
        'Authorization': 'Bearer $accessToken',
      });

      if (response!.statusCode >= 200 && response.statusCode <= 299) {
        return true;
      } else {
        return false;
      }
    } catch (e) {
      return false;
    }
  }

  // Impulse
  Future<Impulse?> impulseOpportunity(Impulse impulse) async {
    final uri = Uri.parse('$baseUri/impulse');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      return null;
    }

    try {
      final response = await client?.post(uri, headers: {
        'Authorization': 'Bearer $accessToken',
        'Content-Type': 'application/json',
      },
      body: jsonEncode(impulse.toJson()));

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final createdImpulse = Impulse.fromJson(jsonDecode(response.body));
        return createdImpulse;
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  // get created Opp
  Future<List<Opportunity>?> getCreatedOpportunities(int userId) async {
    final uri = Uri.parse('$baseUri/created-opportunities/$userId');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      return null;
    }
    try {
      final response = await client.get(uri, headers: {
        'Authorization': 'Bearer $accessToken',
        'Content-Type': 'application/json',
      });

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        final Opportunities =
            jsonList.map((json) => Opportunity.fromJson(json)).toList();
        return Opportunities;
      } else if (response.statusCode == 404) {
        return [];
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  Future<bool> activateAccount(String token) async {
    final uri = Uri.parse('$baseUri/activate?token=$token');

    try {
      final response = await client.get(uri, headers: {
        'Content-Type': 'application/json',
      });

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final dynamic jsonResponse = response.body;
        if (jsonResponse == "Account activated successfully.") {
          return true;
        }
      }
      return false;
    } catch (e) {
      return false;
    }
  }
}
