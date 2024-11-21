// lib/screens/login_screen.dart
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../State/LoginState.dart';

class login_test extends StatelessWidget {
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();

  login_test({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Login')),
      body: Consumer<LoginState>(
        builder: (context, loginState, child) {
          return Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                TextField(
                  controller: _emailController,
                  decoration: InputDecoration(labelText: 'Email'),
                  keyboardType: TextInputType.emailAddress,
                ),
                SizedBox(height: 16.0),
                TextField(
                  controller: _passwordController,
                  decoration: InputDecoration(labelText: 'Password'),
                  obscureText: true,
                ),
                SizedBox(height: 24.0),
                if (loginState.isLoading)
                  CircularProgressIndicator()
                else
                  ElevatedButton(
                    onPressed: () async {
                      final email = _emailController.text;
                      final password = _passwordController.text;
                      await loginState.login(email, password);
                    },
                    child: Text('Login'),
                  ),
                SizedBox(height: 16.0),
                if (loginState.errorMessage != null)
                  Text(
                    loginState.errorMessage!,
                    style: TextStyle(color: Colors.red),
                  ),
              ],
            ),
          );
        },
      ),
    );
  }
}
