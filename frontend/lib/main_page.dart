import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'Api/user_api_handler.dart';  // Make sure to import your UserApiHandler class

class MainPage extends StatefulWidget {
  const MainPage({super.key});

  @override
  State<MainPage> createState() => _MyWidgetState();
}

class _MyWidgetState extends State<MainPage> {
  late UserApiHandler userApiHandler;

  @override
  void initState() {
    super.initState();
    // Initialize the UserApiHandler with http.Client()
    userApiHandler = UserApiHandler(http.Client());
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Portal de Oportunidades'),
        centerTitle: true,
        backgroundColor: const Color(0xFF50C878),
        foregroundColor: Colors.white,
      ),
      body: FutureBuilder<Map<String, dynamic>?>(
        // Fetch user by ID (replace 1 with the actual user ID you want to fetch)
        future: userApiHandler.getUserByID(1), // Call the method from UserApiHandler
        builder: (context, snapshot) {
          // Handle loading state
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }
          
          // Handle errors
          if (snapshot.hasError) {
            return Center(child: Text('Error: ${snapshot.error}'));
          }

          // Handle no data (user not found or error in API call)
          if (!snapshot.hasData || snapshot.data == null) {
            return const Center(child: Text('User not found.'));
          }

          // Extract the user data from the snapshot
          var user = snapshot.data!;

          // Display the user data
          return Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'User ID: ${user['id']}',
                  style: const TextStyle(fontSize: 18),
                ),
                const SizedBox(height: 8),
                Text(
                  'Name: ${user['name']}',
                  style: const TextStyle(fontSize: 18),
                ),
                const SizedBox(height: 8),
                Text(
                  'Email: ${user['email']}',
                  style: const TextStyle(fontSize: 18),
                ),
                // Add more fields based on your API response structure
              ],
            ),
          );
        },
      ),
    );
  }
}
