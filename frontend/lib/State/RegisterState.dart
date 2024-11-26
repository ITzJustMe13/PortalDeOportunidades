import 'package:flutter/material.dart';
import 'package:frontend/Models/User.dart';
import 'package:http/http.dart' as http;
import '../Services/user_api_handler.dart';

class RegisterState with ChangeNotifier {
  final _apiHandler = UserApiHandler(http.Client());

  String? _errorMessage;
  bool _isLoading = false;

  String? get errorMessage => _errorMessage;
  bool get isLoading => _isLoading;

  // Function to register
  Future<void> register(User user, BuildContext context) async {
    _isLoading = true;
    notifyListeners();

    User? createUser = await _apiHandler.createUser(user);

    if (createUser != null) {
      Navigator.pushNamed(context, '/login');
    } else {
      _errorMessage = 'Erro ao criar conta';
    }

    _isLoading = false;
    notifyListeners();
  }
}
