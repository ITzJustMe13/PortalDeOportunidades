import 'package:flutter/material.dart';
import 'package:frontend/Models/User.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:http/http.dart' as http;

class CustomAppBarState with ChangeNotifier {
  final _apiHandler = UserApiHandler(http.Client());
  bool? _isLoggedIn;
  String _userName = "Convidado";

  CustomAppBarState() {
    _initialize();
  }

  // Initialization logic
  void _initialize() async {
    await checkLoginStatus();
  }

  Future<void> checkLoginStatus() async {
    User? user = await _apiHandler.getStoredUser();

    if (user != null) {
      _userName = user.firstName;
      _isLoggedIn = true;
    } else {
      _userName = "Convidado";
      _isLoggedIn = false;
    }

    notifyListeners();
  }

  bool get isLoggedIn => _isLoggedIn ?? false;
  String get userName => _userName;
}
