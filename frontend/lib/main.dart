import 'package:flutter/material.dart';
import 'package:frontend/Views/search_page.dart';
import 'package:frontend/main_page.dart';

import 'Views/home_page.dart';

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
        //'/add-opportunity': (context) => CreateOpportunityScreen(),
        '/home': (context) => HomePage(),
        '/search': (context) => SearchPage(),
      },
    );
  }
}
