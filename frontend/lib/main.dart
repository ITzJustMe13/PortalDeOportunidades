import 'package:flutter/material.dart';
import 'package:frontend/State/ActivationState.dart';
import 'package:frontend/State/AppBarState.dart';
import 'package:frontend/State/CreateOpportunityState.dart';
import 'package:frontend/State/LoginState.dart';
import 'package:frontend/State/RegisterState.dart';
import 'package:frontend/Views/ActivationSucessScreen.dart';
import 'package:frontend/Views/LoginScreen.dart';
import 'package:frontend/Views/PaymentCancelScreen.dart';
import 'package:frontend/Views/RegisterScreen.dart';
import 'package:http/http.dart' as http;

import 'package:frontend/Views/HistoryReservationScreen.dart';
import 'package:frontend/Views/EditProfileScreen.dart';
import 'package:frontend/Views/ProfileScreen.dart';
import 'package:frontend/Views/ReviewsHistoryScreen.dart';
import 'package:frontend/Views/CreateOpportunityScreen.dart';
import 'package:frontend/Views/OpportunityManager.dart';
import 'package:frontend/Views/FavoritesPage.dart';
import 'package:frontend/Views/SearchPage.dart';
import 'package:frontend/Views/PaymentSucessScreen.dart';

import 'Views/HomePage.dart';
import 'package:provider/provider.dart';
import 'package:frontend/Services/payment_api_handler.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:frontend/Services/reservation_api_handler.dart';
import 'package:frontend/Services/review_api_handler.dart';

void main() {
  runApp(
    MultiProvider(
      providers: [
        Provider<OpportunityApiHandler>(
            create: (_) => OpportunityApiHandler(http.Client())),
        Provider<UserApiHandler>(create: (_) => UserApiHandler(http.Client())),
        Provider<ReservationApiHandler>(
            create: (_) => ReservationApiHandler(http.Client())),
        Provider<ReviewApiHandler>(
            create: (_) => ReviewApiHandler(http.Client())),
        Provider<PaymentApiHandler>(
            create: (_) => PaymentApiHandler(http.Client())),
        ChangeNotifierProvider<LoginState>(create: (_) => LoginState()),
        ChangeNotifierProvider<RegisterState>(create: (_) => RegisterState()),
        ChangeNotifierProvider<CustomAppBarState>(
            create: (_) => CustomAppBarState()),
        ChangeNotifierProvider<ActivationState>(
            create: (_) => ActivationState()),
        ChangeNotifierProvider<CreateOpportunityState>(
            create: (_) => CreateOpportunityState())
      ],
      child: MainApp(),
    ),
  );
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
        '/favorites': (context) => const FavoritesPage(),
        '/create-opportunity': (context) => const CreateOpportunityScreen(),
        '/your-opportunities': (context) => const OpportunityManagerScreen(),
        //'/your-reservations': (context) => const YourReservationsPage(),
        '/profile': (context) => const ProfileScreen(),
        '/edit-profile': (context) => const EditProfileScreen(),
        '/reviews-history': (context) => const ReviewsHistoryScreen(),
        '/reservation-history': (context) => const HistoryReservationScreen(),
        '/login': (context) => LoginScreen(),
        '/register': (context) => RegisterScreen(),
        '/payment-success': (context) => PaymentSuccessScreen(),
        '/payment-cancel': (context) => PaymentCancelScreen(),
      },
      onGenerateRoute: (settings) {
        Uri? uri = Uri.tryParse(settings.name ?? '');

        // Check for deep link path
        if (uri != null && uri.path == '/activate-account') {
          String? token = uri.queryParameters['token'];
          return MaterialPageRoute(
            builder: (context) => ActivationSuccessPage(token: token),
          );
        }

        // Default route
        return MaterialPageRoute(builder: (context) => HomePage());
      },
    );
  }
}
