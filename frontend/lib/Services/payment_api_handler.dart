import 'dart:convert';
import 'package:frontend/Models/Impulse.dart';
import 'package:frontend/Models/Reservation.dart';
import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class PaymentApiHandler {
  final String baseUri = "https://localhost:7235/api/Payment";
  final http.Client client;
  final storage = FlutterSecureStorage();

  PaymentApiHandler(this.client);

  Future<String?> createReservationCheckoutSession(
      Reservation reservation) async {
    final uri = Uri.parse('$baseUri/Checkout-Reservation');

    try {
      final response = await client.post(uri,
          headers: {
            'Content-Type': 'application/json',
          },
          body: jsonEncode(reservation.toJson()));

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final sessionId = jsonDecode(response.body)['sessionId'];
        return sessionId;
      } else {
        print('Error: ${response.statusCode} - ${response.reasonPhrase}');
        return null;
      }
    } catch (e) {
      print('Exception occurred: $e');
      return null;
    }
  }

  Future<String?> createImpulseCheckoutSession(Impulse impulse) async {
    final uri = Uri.parse('$baseUri/Checkout-Impulse');

    print(impulse.toJson());

    try {
      final response = await client.post(uri,
          headers: {
            'Content-Type': 'application/json',
          },
          body: jsonEncode(impulse.toJson()));

      print(response);
      print(response.body);
      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final sessionId = jsonDecode(response.body)['sessionId'];
        return sessionId;
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
