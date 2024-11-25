import 'dart:convert';
import 'dart:ffi';
import 'package:frontend/Models/Review.dart';
import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class ReviewApiHandler {
  final String baseUri = "https://localhost:7235/api/Review";
  final http.Client client;
  final storage = FlutterSecureStorage();

  ReviewApiHandler(this.client);

  Future<Review?> getReviewById(int reviewId) async {
    final uri = Uri.parse('$baseUri/$reviewId');
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

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final review = Review.fromJson(jsonDecode(response.body));
        return review;
      } else {
        print('Error: ${response.statusCode} - ${response.reasonPhrase}');
        return null;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return null;
    }
  }

  Future<Review?> createReview(Review review) async {
    final uri = Uri.parse(baseUri);
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      print('Error: No access token found');
      return null;
    }

    try {
      final response = await client.post(uri,
          headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer $accessToken',
          },
          body: jsonEncode(review.toJson()));

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final createdReview = Review.fromJson(jsonDecode(response.body));
        return createdReview;
      } else {
        print('Error: ${response.statusCode} - ${response.reasonPhrase}');
        return null;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return null;
    }
  }

  Future<bool> deleteReview(int reviewId) async {
    final uri = Uri.parse('$baseUri/$reviewId');
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
        print('Review deleted successfully.');
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

  Future<bool> editReview(int id, double score, String? desc) async {
    final uri = Uri.parse('$baseUri/$id/Edit?score=$score&desc=$desc');
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
        print('Review updated successfully.');
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
