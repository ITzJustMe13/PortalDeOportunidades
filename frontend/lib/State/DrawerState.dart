import 'package:flutter/material.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:http/http.dart' as http;

class CustomDrawerState with ChangeNotifier {
  final UserApiHandler _userApiHandler;
  bool? _isLoggedIn;

  CustomDrawerState({required http.Client httpClient, String? token})
      : _userApiHandler = UserApiHandler(httpClient) {
    _checkLoginStatus();
  }

  bool get isLoggedIn => _isLoggedIn ?? false;

  Future<void> _checkLoginStatus() async {
    _isLoggedIn = await _userApiHandler.getStoredUser() != null;
    notifyListeners();
  }

  void navigateToRoute(BuildContext context, String route) {
    _checkLoginStatus();
    if (route.isNotEmpty) {
      Navigator.pushNamed(context, route);
    } else {
      print("Error: Route cannot be empty.");
    }
  }

  void ensureUserLoggedIn(BuildContext context, String route) {
    if (isLoggedIn) {
      navigateToRoute(context, route);
    } else {
      _showLoginDialog(context);
    }
  }

  void logout(BuildContext context, String route) {
    _userApiHandler.logout();
    _isLoggedIn = false;
    navigateToRoute(context, route);
  }

  void _showLoginDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text("Login necessário"),
          content: const Text(
              "Por favor faça login para acessar esta funcionalidade."),
          actions: [
            TextButton(
              child: const Text("Login"),
              onPressed: () {
                navigateToRoute(context, "/login");
              },
            ),
            TextButton(
              child: const Text("Cancel"),
              onPressed: () {
                Navigator.of(context).pop();
              },
            ),
          ],
        );
      },
    );
  }
}
