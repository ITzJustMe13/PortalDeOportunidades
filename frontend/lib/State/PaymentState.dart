import 'package:flutter/material.dart';
import 'package:frontend/Models/Impulse.dart';
import 'package:frontend/Services/payment_service.dart';
import 'package:frontend/Services/reservation_api_handler.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:http/http.dart' as http;

class PaymentState with ChangeNotifier {
  bool _isLoading = false;
  String _errorMessage = "";
  final ReservationApiHandler _reservationApiHandler =
      ReservationApiHandler(http.Client());

  final UserApiHandler _userApiHandler = UserApiHandler(http.Client());

  PaymentState();

  String get errorMessage => _errorMessage;
  bool get isLoading => _isLoading;

  // Handle Reservation
  Future<void> handleReservation() async {
    _isLoading = true;
    notifyListeners();

    final reservation = await PaymentService().getStoredReservation();
    if (reservation == null) {
      _errorMessage = 'Nenhuma reserva encontrada';
      _isLoading = false;
      notifyListeners();
      return;
    }
    var createdReservation =
        await _reservationApiHandler.createReservation(reservation);

    _isLoading = false;
    if (createdReservation == null) {
      _errorMessage = 'Erro ao criar reserva';
      notifyListeners();
      return;
    }

    PaymentService().deleteReservation();
    _errorMessage = "";
    notifyListeners();
  }

  // Handle Impulse
  Future<void> handleImpulse() async {
    _isLoading = true;
    notifyListeners();

    final impulse = await PaymentService().getStoredImpulse();
    if (impulse == null) {
      _errorMessage = 'Nenhum impulso encontrado';
      notifyListeners();
      return;
    }
    var createdImpulse = await _userApiHandler.impulseOpportunity(impulse);
    _isLoading = false;
    if (createdImpulse == null) {
      _errorMessage = 'Erro ao criar impulso';
      notifyListeners();
      return;
    }

    PaymentService().deleteImpulse();
    _errorMessage = "";
    notifyListeners();
  }
}