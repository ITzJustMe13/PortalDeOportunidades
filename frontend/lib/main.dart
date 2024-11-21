import 'package:flutter/material.dart';
import 'package:frontend/Views/HistoryReservationScreen.dart';
import 'package:frontend/main_page.dart';

void main() {
  runApp(const MainApp());
}

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return  MaterialApp(
      home: const MainPage(),
      routes: {
        '/reservation-history': (context) => HistoryReservationScreen(),
      },
    );
  }
}
