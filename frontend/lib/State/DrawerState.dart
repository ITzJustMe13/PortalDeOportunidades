import 'package:flutter/material.dart';
import 'package:frontend/State/LoginState.dart';

class CustomDrawerState with ChangeNotifier {
  final LoginState _loginState;

  CustomDrawerState({
    required LoginState loginState, // Accept as a parameter
  }) : _loginState = loginState;

  void navigateToRoute(BuildContext context, String route) {
    if (route.isNotEmpty) {
      Navigator.pushNamed(context, route);
    } else {
      return;
    }
  }

  void ensureUserLoggedIn(BuildContext context, String route) {
    if (_loginState.isLoggedIn) {
      navigateToRoute(context, route);
    } else {
      _showLoginDialog(context);
    }
  }

  void logout(BuildContext context, String route) {
    _loginState.logout();

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
