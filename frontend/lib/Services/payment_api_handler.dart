import 'dart:convert';
import 'package:frontend/Models/Impulse.dart';
import 'package:frontend/Models/Reservation.dart';
import 'package:frontend/Services/handler.dart';
import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:url_launcher/url_launcher.dart';

class PaymentApiHandler extends handler{
   String baseUri ;
  final http.Client client;
  final storage = FlutterSecureStorage();

  PaymentApiHandler(this.client, {baseUri="$this.apiIP/api/Payment"}):super();
  

  Future<String?> createReservationCheckoutSession(
      Reservation reservation) async {
    final uri = Uri.parse('$baseUri/Checkout-Reservation');

    try {
      final response = await client.post(
        uri,
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(reservation.toJson()),
      );

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        return response.body;  // The response is now just the session URL string
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }

  Future<String?> createImpulseCheckoutSession(Impulse impulse) async {
    final uri = Uri.parse('$baseUri/Checkout-Impulse');
    try {
      final response = await client.post(uri,
          headers: {
            'Content-Type': 'application/json',
          },
          body: jsonEncode(impulse.toJson()));

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        return response.body;
      } else {
        return null;
      }
    } catch (e) {
      return null;
    }
  }
}
