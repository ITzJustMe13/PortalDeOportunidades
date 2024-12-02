import 'package:flutter/material.dart';
import 'package:frontend/Models/Impulse.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Services/payment_api_handler.dart';
import 'package:frontend/Services/payment_service.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:http/http.dart' as http;
import 'package:url_launcher/url_launcher.dart';

class ChooseImpulseState with ChangeNotifier {
  final _apiHandler = UserApiHandler(http.Client());
  final _paymentApiHandler = PaymentApiHandler(http.Client());
  final _paymentService = PaymentService();

  String? _errorMessage;
  bool _isLoading = false;

  String? get errorMessage => _errorMessage;
  bool get isLoading => _isLoading;

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
        print('Could not launch $checkoutUrl');
        _isLoading = false;
        _errorMessage = 'Erro ao abrir checkout';
        notifyListeners();
        return;
      }
    } else {
      print('Failed to create checkout session');
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
