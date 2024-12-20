import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'dart:convert'; // For jsonEncode
import 'package:frontend/Models/Reservation.dart';
import 'package:frontend/Models/Impulse.dart';

class PaymentService {
// Create a secure storage instance
  final storage = FlutterSecureStorage();

// Save Impulse DTO to local storage
  Future<void> saveImpulse(Impulse impulse) async {
    try {
      // Convert the Impulse DTO to a JSON string
      String impulseJson = jsonEncode(impulse.toJson());
      await storage.write(key: 'impulseDto', value: impulseJson);
    } catch (e) {
      return;
    }
  }

// Save Reservation DTO to local storage
  Future<void> saveReservation(Reservation reservation) async {
    try {
      // Convert the Reservation DTO to a JSON string
      String reservationJson = jsonEncode(reservation.toJson());
      await storage.write(key: 'reservationDto', value: reservationJson);
    } catch (e) {
      return;
    }
  }

// Retrieve Impulse DTO from local storage
  Future<Impulse?> getStoredImpulse() async {
    try {
      final String? storedImpulse = await storage.read(key: 'impulseDto');
      if (storedImpulse != null) {
        // Decode the JSON string and return the Impulse object
        return Impulse.fromJson(jsonDecode(storedImpulse));
      }
      return null; // Return null if no impulse is found
    } catch (e) {
      return null;
    }
  }

// Retrieve Reservation DTO from local storage
  Future<Reservation?> getStoredReservation() async {
    try {
      final String? storedReservation =
          await storage.read(key: 'reservationDto');
      if (storedReservation != null) {
        // Decode the JSON string and return the Reservation object
        return Reservation.fromJson(jsonDecode(storedReservation));
      }
      return null; // Return null if no reservation is found
    } catch (e) {
      return null;
    }
  }

  Future<void> deleteImpulse() async {
    try {
      await storage.delete(key: 'impulseDto');
    } catch (e) {
      return;
    }
  }

  Future<void> deleteReservation() async {
    try {
      await storage.delete(key: 'reservationDto');
    } catch (e) {
      return;
    }
  }
}
