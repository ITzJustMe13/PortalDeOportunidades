import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/paginated_opportunity_gallery.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/Opportunity.dart';
import '../Components/on_the_rise_opportunities_carousel.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:provider/provider.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  late Future<List<Opportunity>?> _opportunitiesFuture;
  late Future<List<Opportunity>?> _impulsedOpportunitiesFuture;

  @override
  void initState() {
    super.initState();
    _opportunitiesFuture = Provider.of<OpportunityApiHandler>(context, listen: false)
        .getAllOpportunities();
    _impulsedOpportunitiesFuture = Provider.of<OpportunityApiHandler>(context, listen: false)
        .getAllImpulsedOpportunities();
  }

  @override
  Widget build(BuildContext context) {
    const double padding = 24.0;

    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          // Calculate the screen width based on parent constraints
          double screenWidth = constraints.maxWidth;

          // Dynamically adjust card width based on screen size
          double componentWidth = screenWidth > 1200
              ? screenWidth * 0.6 // For large screens (e.g., desktop)
              : screenWidth * 1; // For smaller screens (e.g., mobile, tablet)

          return ListView(
            padding: const EdgeInsets.all(padding),
            children: [
              Center(
                child: SizedBox(
                    width: componentWidth,
                    child: Column(
                      children: [
                        //FutureBuilder for 'impulsed opportunities'
                        FutureBuilder<List<Opportunity>?>(
                          future: _impulsedOpportunitiesFuture,
                          builder: (context, snapshot) {
                            if (snapshot.connectionState == ConnectionState.waiting) {
                              return const CircularProgressIndicator();
                            } else if (snapshot.hasError) {
                              return Text('Error: ${snapshot.error}');
                            } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
                              return const Text('Sem oportunidades em Destaque');
                            } else {
                              List<Opportunity> opportunitiesOnTheRiseList = snapshot.data!;
                              return OnTheRiseOpportunityCarousel(
                                  opportunitiesOnTheRiseList: opportunitiesOnTheRiseList);
                            }
                          },
                        ),
                        SizedBox(height: padding),
                        Divider(thickness: 1, color: Colors.black),
                        SizedBox(height: padding),
                        
                        // FutureBuilder for 'all opportunities'
                        FutureBuilder<List<Opportunity>?>(
                          future: _opportunitiesFuture,
                          builder: (context, snapshot) {
                            if (snapshot.connectionState == ConnectionState.waiting) {
                              return const CircularProgressIndicator();
                            } else if (snapshot.hasError) {
                              return Text('Error: ${snapshot.error}');
                            } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
                              return const Text('Sem Oportunidades Disponiveis');
                            } else {
                              List<Opportunity> opportunitiesList = snapshot.data!;
                              return PaginatedOpportunityGallery(
                                allOpportunities: opportunitiesList,
                              );
                            }
                          },
                        ),
                      ],
                    )),
              ),
            ],
          );
        },
      ),
    );
  }
}
