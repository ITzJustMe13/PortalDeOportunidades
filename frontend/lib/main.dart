import 'package:flutter/material.dart';

import 'package:frontend/Views/HistoryReservationScreen.dart';
import 'package:frontend/Views/EditProfileScreen.dart';
import 'package:frontend/Views/ProfileScreen.dart';
import 'package:frontend/Views/ReviewsHistoryScreen.dart';
import 'package:frontend/Views/CreateOpportunityScreen.dart';
import 'package:frontend/Views/OpportunityDetailsScreen.dart';
import 'package:frontend/Views/OpportunityManager.dart';
import 'package:frontend/Views/favorites_page.dart';
import 'package:frontend/Views/search_page.dart';


import 'Views/home_page.dart';

void main() {
  runApp(MainApp());
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
      home: HomePage(),
      routes: {
        '/search': (context) => const SearchPage(),
       // '/profile': (context) => const ProfilePage(),
        '/favorites': (context) => const FavoritesPage(),
        '/create-opportunity': (context) => const CreateOpportunityPage(),
        '/your-opportunities': (context) => const YourOpportunitiesPage(),
        '/your-reservations': (context) => const YourReservationsPage(),
        '/add-opportunity': (context) => const CreateOpportunityScreen(),
        '/profile': (context) => const ProfileScreen(),
        '/edit-profile': (context) => const EditProfileScreen(),
        '/reviews-history': (context) => const ReviewsHistoryScreen(),
        '/reservation-history': (context) => const HistoryReservationScreen(),
      },
    );
  }
}
