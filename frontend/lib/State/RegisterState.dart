import 'package:flutter/material.dart';
import 'package:frontend/Models/User.dart';
import 'package:http/http.dart' as http;
import '../Services/user_api_handler.dart';

class RegisterState with ChangeNotifier {
  final _apiHandler = UserApiHandler(http.Client(), null);

  String? _errorMessage;
  bool _isLoading = false;

  String? get errorMessage => _errorMessage;
  bool get isLoading => _isLoading;

  // Function to register
  Future<void> register(User user) async {
    _isLoading = true;
    notifyListeners();

    await _apiHandler.createUser(user);

    _isLoading = false;
    notifyListeners();
  }
}
