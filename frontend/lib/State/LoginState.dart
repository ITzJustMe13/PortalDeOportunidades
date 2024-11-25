// lib/state/login_state.dart
import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:http/http.dart' as http;
import '../Services/user_api_handler.dart';

class LoginState with ChangeNotifier {
  final _storage = FlutterSecureStorage();
  final _apiHandler = UserApiHandler(http.Client(), null);

  String? _token;
  String? _errorMessage;
  bool _isLoading = false;

  String? get token => _token;
  String? get errorMessage => _errorMessage;
  bool get isLoading => _isLoading;

  // Function to login
  Future<void> login(String email, String password) async {
    if (email.isEmpty || password.isEmpty) {
      _errorMessage = 'Email and password cannot be empty';
      notifyListeners();
      return;
    }

    _isLoading = true;
    notifyListeners();

    await _apiHandler.login(email, password);

    _isLoading = false;
    notifyListeners();
  }

  // Function to logout and remove token
  Future<void> logout() async {
    await _storage.delete(key: 'accessToken');
    _token = null;
    notifyListeners();
  }
}
