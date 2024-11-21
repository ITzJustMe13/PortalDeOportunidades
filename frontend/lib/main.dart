import 'package:flutter/material.dart';
import 'package:frontend/Views/OpportunityDetailsScreen.dart';
import 'package:frontend/Views/OpportunityManager.dart';
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
      title: 'Portal de Oportunidades',
      theme: ThemeData(
        primarySwatch: Colors.green,
      ),
      home: OpportunityManager(),
      /*
      initialRoute: '/',
      routes: {
        '/': (context) => const HomePage(),
        '/search': (context) => const SearchPage(),
        '/profile': (context) => const ProfilePage(),
        '/favorites': (context) => const FavoritesPage(),
        '/create-opportunity': (context) => const CreateOpportunityPage(),
        '/your-opportunities': (context) => const YourOpportunitiesPage(),
        '/your-reservations': (context) => const YourReservationsPage(),
      },
      */
    );
  }
}
