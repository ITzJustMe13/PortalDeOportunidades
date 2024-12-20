import 'dart:convert';
//import 'dart:ffi';
import 'package:frontend/Models/Review.dart';
import 'package:frontend/Services/handler.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class ReviewApiHandler extends Handler{
  late final String baseUri;
  final storage = FlutterSecureStorage();
  final timeout = const Duration(seconds: 60);

  ReviewApiHandler({
    String? baseUri,
  }) {
    this.baseUri = baseUri ?? "$apiIP/api/Review";
  }

  Future<Review?> getReviewById(int reviewId) async {
    final uri = Uri.parse('$baseUri/$reviewId');
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
        final review = Review.fromJson(jsonDecode(response.body));
        return review;
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  Future<List<Review>> getReviewsByUserId(int userId) async {
    final uri = Uri.parse('$baseUri/getReviewsByUser/$userId');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      return [];
    }

    try {
      final response = await client.get(uri, headers: {
        'Authorization': 'Bearer $accessToken',
        'Content-Type': 'application/json',
      });

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        final userReviews =
            jsonList.map((json) => Review.fromJson(json)).toList();
        return userReviews;
      } else {
        return [];
      }
    } catch (e) {
      return [];
    }
  }

  Future<Review?> createReview(Review review) async {
    final uri = Uri.parse(baseUri);
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
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
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  Future<bool> deleteReview(int reviewId) async {
    final uri = Uri.parse('$baseUri/$reviewId');
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

  Future<bool> editReview(int id, Review review) async {
    final uri = Uri.parse('$baseUri/$id/Edit');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      return false;
    }

    try {
      final reviewJson = review.toJson();

      final response = await client.put(uri, headers: {
            'Authorization': 'Bearer $accessToken',
            'Content-Type': 'application/json',
          },
          body: jsonEncode(reviewJson));

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
