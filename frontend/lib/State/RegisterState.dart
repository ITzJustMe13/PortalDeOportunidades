import 'package:flutter/material.dart';
import 'package:frontend/Models/User.dart';
import 'package:http/http.dart' as http;
import '../Services/user_api_handler.dart';

class RegisterState with ChangeNotifier {
  var _apiHandler = UserApiHandler();

  String? _errorMessage;
  bool _isLoading = false;
  bool _isActivationSuccess = false;

  String? get errorMessage => _errorMessage;
  bool get isLoading => _isLoading;
  bool get isActivationSuccess => _isActivationSuccess;

  set userApiHandler(UserApiHandler handler) {
    _apiHandler = handler;
  }

  // Function to register
  Future<void> register(User user, BuildContext context) async {
    _isLoading = true;
    notifyListeners();

    User? createUser = await _apiHandler.createUser(user);

    _isLoading = false;

    if (createUser != null) {
      notifyListeners();

      _isActivationSuccess = true;
      return;
    }

    _isActivationSuccess = false;
    _errorMessage = 'Erro ao criar conta';
    notifyListeners();
  }
}
