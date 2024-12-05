import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/DynamicActionButton.dart';
import 'package:provider/provider.dart';
import '../State/LoginState.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  String? _emailError;
  String? _passwordError;
  final _formKey = GlobalKey<FormState>();

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: Consumer<LoginState>(
        builder: (context, loginState, child) {
          return LayoutBuilder(
            builder: (context, constraints) {
              double screenWidth = constraints.maxWidth;

              double componentWidth = screenWidth > 1200
                  ? screenWidth * 0.4
                  : (screenWidth > 800 ? screenWidth * 0.6 : screenWidth * 1);

              return SingleChildScrollView(
                padding: const EdgeInsets.all(16.0),
                child: Form(
                  key: _formKey,
                  child: Center(
                    child: SizedBox(
                      width: componentWidth,
                      child: Card(
                        elevation: 4,
                        child: Padding(
                          padding: const EdgeInsets.all(16.0),
                          child: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              Text("Login",
                                  style: TextStyle(
                                      fontSize: 24,
                                      fontWeight: FontWeight.bold,
                                      color: Colors.black)),
                              SizedBox(height: 16.0),
                              TextFormField(
                                controller: _emailController,
                                decoration: InputDecoration(
                                  labelText: 'Email',
                                  errorText: _emailError,
                                ),
                                keyboardType: TextInputType.emailAddress,
                                validator: (value) {
                                  if (value!.isEmpty || !value.contains('@')) {
                                    return 'Please enter a valid email';
                                  }
                                  return null;
                                },
                              ),
                              SizedBox(height: 16.0),
                              TextFormField(
                                controller: _passwordController,
                                decoration: InputDecoration(
                                  labelText: 'Password',
                                  errorText: _passwordError,
                                ),
                                obscureText: true,
                                validator: (value) {
                                  if (value == null) {
                                    return 'Please enter your password';
                                  }
                                  return null;
                                },
                              ),
                              SizedBox(height: 24.0),
                              if (loginState.isLoading)
                                CircularProgressIndicator()
                              else
                                DynamicActionButton(
                                  onPressed: () async {
                                    if (_formKey.currentState!.validate()) {
                                      final email = _emailController.text;
                                      final password = _passwordController.text;
                                      bool isLoggedIn = await loginState.login(
                                          email, password, context);
                                      if (!isLoggedIn) {
                                        setState(() {
                                          _emailError =
                                              'Invalid email or password';
                                          _passwordError =
                                              'Invalid email or password';
                                        });
                                      }
                                    }
                                  },
                                  text: 'Login',
                                  color: Color(0xFF00C75A),
                                  icon: Icons.login_rounded,
                                ),
                              SizedBox(height: 16.0),
                              if (loginState.errorMessage != null)
                                Text(
                                  loginState.errorMessage!,
                                  style: TextStyle(color: Colors.red),
                                ),
                              DynamicActionButton(
                                  onPressed: () async {
                                    loginState.loginWithGoogle();
                                  },
                                  text: "Login com Google",
                                  icon: Icons.abc,
                                  color: Color(0xFF4285F4))
                            ],
                          ),
                        ),
                      ),
                    ),
                  ),
                ),
              );
            },
          );
        },
      ),
    );
  }
}
