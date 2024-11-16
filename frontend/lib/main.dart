import 'package:flutter/material.dart';
import 'package:frontend/main_page.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../Screens/login_test.dart';
import '../State/LoginState.dart';

void main() {
  runApp(MainApp());
}

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (context) => LoginState(),
      child: MaterialApp(
        title: 'Flutter Login Demo',
        theme: ThemeData(
          primarySwatch: Colors.blue,
        ),
        home: login_test(),
      ),
    );
  }
}
