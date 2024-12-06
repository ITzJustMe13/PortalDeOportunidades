import 'dart:convert';
//import 'dart:ffi';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/Review.dart';
import 'package:frontend/Services/handler.dart';
import '../Models/Opportunity.dart';

class OpportunityApiHandler extends Handler{
  late final String baseUri;
  final storage = FlutterSecureStorage();
  final timeout = const Duration(seconds: 30);

  OpportunityApiHandler({
    String? baseUri,
  }) {
    this.baseUri = baseUri ?? "$apiIP/api/Opportunity";
  }

  Future<Opportunity?> getOpportunityByID(int id) async {
    final uri = Uri.parse('$baseUri/$id');

    try {
      final response = await client.get(
        uri,
        headers: {
          'Content-type': 'application/json; charset=UTF-8',
        },
      );

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final opportunity = Opportunity.fromJson(jsonDecode(response.body));
        return opportunity;
      } else {
        return null;
      }
    } catch (e) {
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
        return [];
      } else {
        return null;
      }
    } catch (e) {
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
        return [];
      } else {
        return null;
      }
    } catch (e) {
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
        return [];
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  Future<List<Opportunity>> searchOpportunities(
      String? keyword,
      int? vacancies,
      double? minPrice,
      double? maxPrice,
      OppCategory? category,
      Location? location) async {
    String query = "";
    if (keyword != null) {
      query += "keyword=$keyword&";
    }
    if (vacancies != null) {
      query += "vacancies=$vacancies&";
    }
    if (minPrice != null) {
      query += "minPrice=$minPrice&";
    }
    if (maxPrice != null) {
      query += "maxPrice=$maxPrice&";
    }
    if (category != null) {
      query += "category=${categoryToInt(category)}&";
    }
    if (location != null) {
      query += "location=${locationToInt(location)}&";
    }

    final uri = Uri.parse('$baseUri/Search?$query');

    try {
      final response = await client.get(uri).timeout(timeout);

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        final searchedOpp =
            jsonList.map((json) => Opportunity.fromJson(json)).toList();
        return searchedOpp;
      } else if (response.statusCode == 404) {
        return [];
      } else {
        return [];
      }
    } catch (e) {
      return [];
    }
  }

  Future<List<Review>?> getReviewsByOppId(int oppId) async {
    final uri = Uri.parse('$baseUri/Reviews/$oppId');

    try {
      final response = await client.get(uri).timeout(timeout);

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        final reviews =
            jsonList.map((json) => Review.fromJson(json)).toList();
        return reviews;
      } else if (response.statusCode == 404) {
        return [];
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  Future<Opportunity?> createOpportunity(Opportunity opportunity) async {
    final uri = Uri.parse(baseUri);
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
        body: jsonEncode(opportunity.toJson()),
      );

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final Opp = Opportunity.fromJson(jsonDecode(response.body));
        return Opp;
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  /// Documentation for deleteOpportunity
  /// Endpoint that sends a delete Opportunity request
  /// @param oppId : io of the opportunity
  /// @returns: true if it was delete sucessefully, false if not
  Future<bool> deleteOpportunity(int oppId) async {
    final uri = Uri.parse('$baseUri/$oppId');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      return false;
    }

    try {
      final response = await client.delete(uri, headers: {
        'Authorization': 'Bearer $accessToken',
      });

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        return true;
      } else {
        return false;
      }
    } catch (e) {
      return false;
    }
  }

  Future<bool> activateOpportunity(int oppId) async {
    final uri = Uri.parse('$baseUri/$oppId/activate');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      return false;
    }

    try {
      final response = await client.put(uri, headers: {
        'Authorization': 'Bearer $accessToken',
      });

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        return true;
      } else {
        return false;
      }
    } catch (e) {
      return false;
    }
  }

  Future<bool> deactivateOpportunity(int oppId) async {
    final uri = Uri.parse('$baseUri/$oppId/deactivate');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      return false;
    }

    try {
      final response = await client.put(uri, headers: {
        'Authorization': 'Bearer $accessToken',
      });

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        return true;
      } else {
        return false;
      }
    } catch (e) {
      return false;
    }
  }

  Future<bool> editOpportunity(int oppId, Opportunity opportunity) async {
    final uri = Uri.parse('$baseUri/$oppId/Edit');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
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
        return true;
      } else {
        return false;
      }
    } catch (e) {
      return false;
    }
  }
}
