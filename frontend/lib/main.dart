import 'package:flutter/material.dart';
import 'package:frontend/Views/OpportunityDetailsScreen.dart';
import 'package:frontend/main_page.dart';

void main() {
  runApp(const MainApp());
}

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false, // Removes the debug banner
      title: 'Flutter App',
      theme: ThemeData(
        primarySwatch: Colors.green,
      ),
      home: OpportunityDetailsScreen(), // No need to use `.new`
    );
  }
}
