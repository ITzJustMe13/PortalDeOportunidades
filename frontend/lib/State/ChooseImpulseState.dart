import 'package:flutter/material.dart';
import 'package:frontend/Models/Impulse.dart';
import 'package:frontend/Services/payment_api_handler.dart';
import 'package:frontend/Services/payment_service.dart';
import 'package:frontend/Services/user_api_handler.dart';

class ChooseImpulseState with ChangeNotifier {
  var _apiHandler = UserApiHandler();
  var _paymentApiHandler = PaymentApiHandler();
  var _paymentService = PaymentService();

  String? _errorMessage;
  bool _isLoading = false;

  // Funções que podem ser injetadas para testes
  Future<bool> Function(String) canLaunch = (url) async => true; // Mockable
  Future<void> Function(String) launch = (url) async {}; // Mockable

  String? get errorMessage => _errorMessage;
  bool get isLoading => _isLoading;

  set isLoading(bool value) {
    _isLoading = value;
    notifyListeners(); // Notifica os ouvintes para atualizar a interface
  }

  set errorMessage(String? value) {
    _errorMessage = value;
    notifyListeners(); // Notifica os ouvintes sobre a mudança
  }

  set apiHandler(UserApiHandler handler) {
    _apiHandler = handler;
  }

  set paymentService(PaymentService service) {
    _paymentService = service;
  }

  set paymentApiHandler(PaymentApiHandler handler) {
    _paymentApiHandler = handler;
  }

  ChooseImpulseState();

  Future<void> impulse(int days, double value, int opportunityId) async {
    _isLoading = true;
    notifyListeners();
    var currentUser = await _apiHandler.getStoredUser();

    if (currentUser == null) {
      _errorMessage = 'Erro ao criar conta';
      _isLoading = false;
      notifyListeners();
      return;
    }

    final impulse = Impulse(
        opportunityId: opportunityId,
        userId: currentUser.userId,
        value: value,
        expireDate: DateTime.now().add(Duration(days: days)));

    await _paymentService.saveImpulse(impulse);

    String? checkoutUrl =
        await _paymentApiHandler.createImpulseCheckoutSession(impulse);

    if (checkoutUrl != null) {
      // url_launcher to open the checkout session in the user's browser.
      if (await canLaunch(checkoutUrl)) {
        await launch(checkoutUrl);
      } else {
        _isLoading = false;
        _errorMessage = 'Erro ao abrir checkout';
        notifyListeners();
        return;
      }
    } else {
      _errorMessage = 'Erro ao criar checkout session';
      _isLoading = false;
      notifyListeners();
      return;
    }

    _isLoading = false;
    notifyListeners();
    return;
  }
}