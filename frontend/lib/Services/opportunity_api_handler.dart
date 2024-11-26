import 'dart:convert';
//import 'dart:ffi';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';

import 'package:http/http.dart' as http;
import '../Models/Opportunity.dart';

class OpportunityApiHandler {

  final String baseUri = "https://localhost:7235/api/Opportunity";

  final http.Client client;
  final storage = FlutterSecureStorage();
  final timeout = const Duration(seconds: 30);


  OpportunityApiHandler(this.client);

  Future<Opportunity?> getOpportunityByID(int id) async {

    final uri = Uri.parse('$baseUri/$id');

    try {
      final response = await client.get(
        uri,
        headers: {
          'Content-type': 'application/json; charset=UTF-8',
        },
      ).timeout(timeout);

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final opportunity = Opportunity.fromJson(jsonDecode(response.body));
        return opportunity;
      } else {
        print('Error: ${response.statusCode} ${response.reasonPhrase}');
        return null;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return null;
    }
  }

  Future<List<Opportunity>?> getAllOpportunities() async {
    final uri = Uri.parse(baseUri);

    try {
      final response = await client.get(uri).timeout(timeout);

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        final opportunities =
            jsonList.map((json) => Opportunity.fromJson(json)).toList();
        return opportunities;
      } else if (response.statusCode == 404) {
        print('No opportunities found!');
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

  Future<List<Opportunity>?> getAllImpulsedOpportunities() async {
    final uri = Uri.parse('$baseUri/Impulsed');

    try {
      final response = await client.get(uri).timeout(timeout);

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        final impulsedOpp =
            jsonList.map((json) => Opportunity.fromJson(json)).toList();
        return impulsedOpp;
      } else if (response.statusCode == 404) {
        print('No impulsed opportunities found!');
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

  Future<List<Opportunity>?> getAllOpportunitiesByUserId(int userId) async {
    final uri = Uri.parse('$baseUri/User/$userId');

    try {
      final response = await client.get(uri).timeout(timeout);

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        final userOpp =
            jsonList.map((json) => Opportunity.fromJson(json)).toList();
        return userOpp;
      } else if (response.statusCode == 404) {
        print('No impulsed opportunities found!');
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

  Future<List<Opportunity>?> SearchOpportunities(
      String? keyword,
      int? vacancies,
      double? minPrice,
      double? maxPrice,
      OppCategory? category,
      Location? location) async {
    final uri = Uri.parse(
        '$baseUri/Search?keyword=$keyword&vacancies=$vacancies&minPrice=$minPrice&maxPrice=$maxPrice&category=$category&location=$location');

    try {
      final response = await client.get(uri).timeout(timeout);

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        final searchedOpp =
            jsonList.map((json) => Opportunity.fromJson(json)).toList();
        return searchedOpp;
      } else if (response.statusCode == 404) {
        print('No Searched opportunities found!');
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

  Future<Opportunity?> createOpportunity(Opportunity opportunity) async {
    final uri = Uri.parse(baseUri);
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      print('Error: No access token found');
      return null;
    }

    try {
      final response = await client.post(uri, headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer $accessToken',
      });

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final Opp = Opportunity.fromJson(jsonDecode(response.body));
        return Opp;
      } else {
        print('Error: ${response.statusCode} - ${response.reasonPhrase}');
        return null;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return null;
    }
  }

  Future<bool> deleteOpportunity(int oppId) async {
    final uri = Uri.parse('$baseUri/$oppId');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      print('Error: No access token found');
      return false;
    }

    try {
      final response = await client.delete(uri, headers: {
        'Authorization': 'Bearer $accessToken',
      });

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        print('Opportunity deleted successfully.');
        return true;
      } else {
        print('Error: ${response.statusCode} - ${response.reasonPhrase}');
        return false;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return false;
    }
  }

  Future<bool> activateOpportunity(int oppId) async {
    final uri = Uri.parse('$baseUri/$oppId/activate');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      print('Error: No access token found');
      return false;
    }

    try {
      final response = await client.put(uri, headers: {
        'Authorization': 'Bearer $accessToken',
      });

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        print('Opportunity activated successfully');
        return true;
      } else {
        print('Error: ${response.statusCode} - ${response.reasonPhrase}');
        return false;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return false;
    }
  }

  Future<bool> deactivateOpportunity(int oppId) async {
    final uri = Uri.parse('$baseUri/$oppId/deactivate');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      print('Error: No access token found');
      return false;
    }

    try {
      final response = await client.put(uri, headers: {
        'Authorization': 'Bearer $accessToken',
      });

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        print('Opportunity deactivated successfully');
        return true;
      } else {
        print('Error: ${response.statusCode} - ${response.reasonPhrase}');
        return false;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return false;
    }
  }

  Future<bool> editOpportunity(int oppId, Opportunity opportunity) async {
    final uri = Uri.parse('$baseUri/$oppId/Edit');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      print('Error: No access token found');
      return false;
    }

    try {
      final oppJson = opportunity.toJson();

      final response = await client.put(uri,
          headers: {
            'Authorization': 'Bearer $accessToken',
            'Content-Type': 'application/json',
          },
          body: jsonEncode(oppJson));

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        print('Opportunity updated successfully');
        return true;
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
