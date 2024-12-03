import 'dart:convert';
import 'package:frontend/Models/Reservation.dart';
import 'package:frontend/Services/handler.dart';
import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class ReservationApiHandler extends Handler{
  late final String baseUri;
  final storage = FlutterSecureStorage();

  ReservationApiHandler({
    String? baseUri,
  }) {
    this.baseUri = baseUri ?? "$apiIP/api/Reservation";
  }

  Future<List<Reservation>?> getAllActiveReservationsByUserId(
      int userId) async {
    final uri = Uri.parse('$baseUri/$userId/AllActiveReservations');
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
        final activeReservations =
            jsonList.map((json) => Reservation.fromJson(json)).toList();
        return activeReservations;
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  Future<List<Reservation>?> getAllReservationsByUserId(int userId) async {
    final uri = Uri.parse('$baseUri/$userId/AllReservations');
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
        final reservations =
            jsonList.map((json) => Reservation.fromJson(json)).toList();
        return reservations;
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  Future<Reservation?> getReservationById(int reservationId) async {
    final uri = Uri.parse('$baseUri/$reservationId');
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
        final reservation = Reservation.fromJson(jsonDecode(response.body));
        return reservation;
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  Future<Reservation?> createReservation(Reservation reservation) async {
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
          body: jsonEncode(reservation.toJson()));

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final createdReservation =
            Reservation.fromJson(jsonDecode(response.body));
        return createdReservation;
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  Future<bool> deactivateReservation(int reservationId) async {
    final uri = Uri.parse('$baseUri/$reservationId/deactivate');
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

  Future<bool> editReservation(
      int reservationId, Reservation reservation) async {
    final uri = Uri.parse('$baseUri/$reservationId');
    final String? accessToken = await storage.read(key: 'accessToken');

    if (accessToken == null) {
      return false;
    }

    try {
      final resJson = reservation.toJson();

      final response = await client.put(uri,
          headers: {
            'Authorization': 'Bearer $accessToken',
            'Content-Type': 'application/json',
          },
          body: jsonEncode(resJson));

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        return true;
      } else {
        return false;
      }
    } catch (e) {
      return false;
    }
  }

  Future<bool> deleteReservation(int reservationId) async {
    final uri = Uri.parse('$baseUri/$reservationId');
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
}
