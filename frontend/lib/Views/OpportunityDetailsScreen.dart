import 'package:flutter/material.dart';
import 'package:frontend/Views/CustomAppBar.dart';
import 'package:frontend/Views/CustomDrawer.dart';

class OpportunityDetailsScreen extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: Center(
        child: const Text(
          'Welcome to Your Opportunity Details Screen!',
          style: TextStyle(fontSize: 24),
        ),
      ),
    );
  }
}
