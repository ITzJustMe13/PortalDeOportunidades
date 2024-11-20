import 'package:flutter/material.dart';
import 'package:frontend/Views/EditProfileScreen.dart';
import 'package:frontend/Views/ProfileScreen.dart';
import 'package:frontend/Views/ReviewsHistoryScreen.dart';
import 'package:frontend/main_page.dart';

void main() {
  runApp(const MainApp());
}

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: const MainPage(),
      routes: {
        '/profile': (context) => ProfileScreen(),
        '/edit-profile': (context) => EditProfileScreen(),
        '/reviews-history': (context) => ReviewsHistoryScreen()
      },
    );
  }
}
