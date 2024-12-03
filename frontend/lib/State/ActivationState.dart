import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import '../Services/user_api_handler.dart';

class ActivationState with ChangeNotifier {
  var _apiHandler = UserApiHandler();

  void setApiHandler(UserApiHandler apiHandler) {
    _apiHandler = apiHandler;
  }

  String? _errorMessage;
  bool _isLoading = false;
  bool _accountActivated = false;

  String? get errorMessage => _errorMessage;
  bool get isLoading => _isLoading;
  bool get accountActivated => _accountActivated;

  ActivationState({UserApiHandler? apiHandler}) {
    _apiHandler = apiHandler ?? UserApiHandler();
  }
  // Funções para configuração em testes
  void setAccountActivated(bool value) {
    _accountActivated = value;
    notifyListeners();
  }

  void setErrorMessage(String? message) {
    _errorMessage = message;
    notifyListeners();
  }
  // Function to login
  Future<void> activateAccount(String token) async {

    if (token.isEmpty) {
      _errorMessage = 'Ocorreu um erro!';
      notifyListeners();
      return;
    }

    _isLoading = true;
    notifyListeners();

    var response = await _apiHandler.activateAccount(token);
    _isLoading = false;

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
