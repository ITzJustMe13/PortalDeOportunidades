import 'package:flutter/material.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:http/http.dart' as http;
import 'Api/opportunity_api_handler.dart';

class MainPage extends StatefulWidget {
  const MainPage({super.key});

  @override
  State<MainPage> createState() => _MyWidgetState();
}

class _MyWidgetState extends State<MainPage> {
  late OpportunityApiHandler opportunityApiHandler;

  @override
  void initState() {
    super.initState();
    // Initialize the UserApiHandler with http.Client()
    opportunityApiHandler = OpportunityApiHandler(http.Client());
  }

@override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Portal de Oportunidades'),
        centerTitle: true,
        backgroundColor: const Color(0xFF50C878),
        foregroundColor: Colors.white,
        actions: [
          IconButton(icon: const Icon(Icons.add),
          onPressed:(){
            //Navigator.pushNamed(context, '/add-opportunity');
            Navigator.pushNamed(context, '/home');
          })
        ]
      ),
      body: FutureBuilder<Opportunity?>(
        future: opportunityApiHandler.getOpportunityByID(2), // Use the instance here
        builder: (context, snapshot) {
          // Handle loading state
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }

          // Handle errors
          if (snapshot.hasError) {
            return Center(child: Text('Error: ${snapshot.error}'));
          }

          // Handle no data (opportunity not found or error in API call)
          if (!snapshot.hasData || snapshot.data == null) {
            return const Center(child: Text('Opportunity not found.'));
          }

          // Extract the opportunity data from the snapshot
          var opportunity = snapshot.data!;

          // Display the opportunity data
          return Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Opportunity ID: ${opportunity.opportunityId}',  // Access `id` from Opportunity object
                  style: const TextStyle(fontSize: 18),
                ),
                const SizedBox(height: 8),
                Text(
                  'Name: ${opportunity.name}',  // Access `name` from Opportunity object
                  style: const TextStyle(fontSize: 18),
                ),
                const SizedBox(height: 8),
                Text(
                  'Price: ${opportunity.price}',  // Access `price` from Opportunity object
                  style: const TextStyle(fontSize: 18),
                ),
              ],
            ),
          );
        },
      ),
    );
  }
}
