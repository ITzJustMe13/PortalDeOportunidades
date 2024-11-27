import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import '../Services/user_api_handler.dart';

class ActivationState with ChangeNotifier {
  final _apiHandler = UserApiHandler(http.Client());

  String? _errorMessage;
  bool _isLoading = false;
  bool _accountActivated = false;

  String? get errorMessage => _errorMessage;
  bool get isLoading => _isLoading;
  bool get accountActivated => _accountActivated;

  // Function to login
  Future<void> activateAccount(String token) async {

    print(token);
    if (token.isEmpty) {
      _errorMessage = 'Ocorreu um erro!';
      notifyListeners();
      return;
    }

    _isLoading = true;
    notifyListeners();

    var response = await _apiHandler.activateAccount(token);
    _isLoading = false;
    print(response);

    if (response == false) {
      _accountActivated = false;
      _errorMessage = 'Ocorreu um erro!';
      notifyListeners();
      return;
    }
    _accountActivated = true;
    notifyListeners();
    return;
  }
}
