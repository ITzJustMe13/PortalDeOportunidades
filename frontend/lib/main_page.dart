import 'package:flutter/material.dart';
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
          IconButton(
              icon: const Icon(Icons.add),
              onPressed: () {
                //Navigator.pushNamed(context, '/add-opportunity');
                Navigator.pushNamed(context, '/home');
              }),
          IconButton(
              icon: const Icon(Icons.add),
              onPressed: () {
                //Navigator.pushNamed(context, '/add-opportunity');
                Navigator.pushNamed(context, '/search');
              })
        ],
      ),
    );
  }
}
