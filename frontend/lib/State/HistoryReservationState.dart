import 'dart:async'; // Import for Timer
import 'package:flutter/material.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/Reservation.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:frontend/Services/reservation_api_handler.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:http/http.dart' as http;

class HistoryReservationState with ChangeNotifier {
  final _reservationApiHandler = ReservationApiHandler(http.Client());
  final _userApiHandler = UserApiHandler(http.Client());
  final _opportunityApiHandler = OpportunityApiHandler(http.Client());

  List<Map<Reservation, Opportunity>> _reservationList = [];
  bool _isLoading = false;
  bool _isCancelling = false;
  String _err = "";
  Timer? _timer; // Timer to periodically update the list

  HistoryReservationState() {
    getReservationList();
    _startAutoRefresh(); 
  }

  List<Map<Reservation, Opportunity>> get reservationList => _reservationList;
  bool get isLoading => _isLoading;
  String get error => _err;
  bool get isCancelling => _isCancelling;

  Future<void> getReservationList() async {
    _isLoading = true;
    notifyListeners();

    final userId = await _userApiHandler.getStoredUserID();

    if (userId == -1) {
      _isLoading = false;
      _err = "Erro ao carregar reservas";
      _reservationList = [];
      notifyListeners();
      return;
    }

    List<Reservation> reservations =
        await _reservationApiHandler.getAllReservationsByUserId(userId) ?? [];

    if (reservations.isEmpty) {
      _isLoading = false;
      _reservationList = [];
      notifyListeners();
      return;
    }

    _reservationList.clear(); // Clear the old list before updating
    for (var reservation in reservations) {
      var opportunity = await _opportunityApiHandler
          .getOpportunityByID(reservation.opportunityId);

      if (opportunity != null) {
        _reservationList.add({reservation: opportunity});
      }
    }

    _reservationList.sort((a, b) =>
        b.keys.first.reservationDate.compareTo(a.keys.first.reservationDate));

    _isLoading = false;
    notifyListeners();
  }

  Future<bool> cancelReservation(Reservation reservation) async {
    _isCancelling = true;
    notifyListeners();

    bool isSuccess = await _reservationApiHandler
        .deleteReservation(reservation.reservationId ?? -1);

    if (!isSuccess) {
      _isCancelling = false;
      _err = "Erro ao cancelar reserva";
      notifyListeners();
      return false;
    }

    _reservationList.removeWhere((element) =>
        element.keys.first.reservationId == reservation.reservationId);
    _isCancelling = false;
    notifyListeners();
    return true;
  }

  void _startAutoRefresh() {
    _timer = Timer.periodic(Duration(minutes: 1), (timer) {
      getReservationList();
    });
  }

  @override
  void dispose() {
    _timer?.cancel(); // Cancel the timer when the state is disposed
    super.dispose();
  }
}
