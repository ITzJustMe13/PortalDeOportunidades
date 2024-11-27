
import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:frontend/Models/User.dart';
import 'package:http/http.dart' as http;
import '../Services/user_api_handler.dart';

class LoginState with ChangeNotifier {
  final _storage = FlutterSecureStorage();
  final _apiHandler = UserApiHandler(http.Client());

  String? _token;
  String? _errorMessage;
  bool _isLoading = false;

  String? get token => _token;
  String? get errorMessage => _errorMessage;
  bool get isLoading => _isLoading;

  // Function to login
  Future<bool> login(
      String email, String password, BuildContext context) async {
    if (email.isEmpty || password.isEmpty) {
      _errorMessage = 'O email e a password não podem estar vazios';
      notifyListeners();
      return false;
    }

    _isLoading = true;
    notifyListeners();

    var response = await _apiHandler.login(email, password);
    _isLoading = false;
    print(response);

    if (response == null) {
      _errorMessage =
          'O email ou a password estão incorretos. Ou a conta ainda não foi ativada.';
      notifyListeners();
      return false;
    }
    Navigator.pushNamed(context, '/');
    notifyListeners();

    return true;
  }

  // Function to logout and remove token
  Future<void> logout() async {
    await _storage.delete(key: 'accessToken');
    _token = null;
    notifyListeners();
  }
}
