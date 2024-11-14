import 'package:flutter/material.dart';
import 'package:frontend/Components/on_the_rise_opportunity_card.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/Opportunity.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  _HomePageState createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  var opportunitiesOnTheRiseList = [
    Opportunity(
        name: "Oportunidade1",
        price: 10.2,
        vacancies: 1,
        isActive: true,
        category: OppCategory.AGRICULTURA,
        description: "description",
        location: Location.ACORES,
        address: "address",
        userId: 1,
        reviewScore: 42,
        date: DateTime.now(),
        isImpulsed: true,
        opportunityImgs: []),
    Opportunity(
        name:
            "Oportunidade2 asdadddddddddddd dddddddddddddddddd ddddddddddddddddddddddddddddddddddddddddd ddddddddddddddddddddddddddddd",
        price: 10.2,
        vacancies: 1,
        isActive: true,
        category: OppCategory.AGRICULTURA,
        description: "description",
        location: Location.ACORES,
        address: "address",
        userId: 1,
        reviewScore: 42,
        date: DateTime.now(),
        isImpulsed: true,
        opportunityImgs: []),
    Opportunity(
        name: "Oportunidade3",
        price: 10.2,
        vacancies: 1,
        isActive: true,
        category: OppCategory.AGRICULTURA,
        description: "description",
        location: Location.ACORES,
        address: "address",
        userId: 1,
        reviewScore: 42,
        date: DateTime.now(),
        isImpulsed: true,
        opportunityImgs: []),
  ];

  var opportunitiesList = [
    Opportunity(
        name: "Oportunidade4",
        price: 10.2,
        vacancies: 1,
        isActive: true,
        category: OppCategory.AGRICULTURA,
        description: "description",
        location: Location.ACORES,
        address: "address",
        userId: 1,
        reviewScore: 42,
        date: DateTime.now(),
        isImpulsed: false,
        opportunityImgs: []),
    Opportunity(
        name: "Oportunidade5",
        price: 10.2,
        vacancies: 1,
        isActive: true,
        category: OppCategory.AGRICULTURA,
        description: "description",
        location: Location.ACORES,
        address: "address",
        userId: 1,
        reviewScore: 42,
        date: DateTime.now(),
        isImpulsed: false,
        opportunityImgs: []),
    Opportunity(
        name: "Oportunidade6",
        price: 10.2,
        vacancies: 1,
        isActive: true,
        category: OppCategory.AGRICULTURA,
        description: "description",
        location: Location.ACORES,
        address: "address",
        userId: 1,
        reviewScore: 42,
        date: DateTime.now(),
        isImpulsed: false,
        opportunityImgs: []),
  ];

  @override
  Widget build(BuildContext context) {
    const double padding = 24.0;

    return Scaffold(
      appBar: AppBar(
        title: Text('Opportunities'),
      ),
      body: LayoutBuilder(
        builder: (context, constraints) {
          // Calculate the screen width based on parent constraints
          double screenWidth = constraints.maxWidth;

          // Dynamically adjust card width based on screen size
          double componentWidth = screenWidth > 1200
              ? screenWidth * 0.6 // For large screens (e.g., desktop)
              : screenWidth * 1; // For smaller screens (e.g., mobile, tablet)

          return Center(
            // Add this Center widget to center the ListView
            child: SizedBox(
              width: componentWidth,
              child: ListView(
                padding: const EdgeInsets.all(padding),
                children: [
                  // For each opportunity, wrap in a SizedBox with width constraint
                  for (var opportunity in opportunitiesOnTheRiseList)
                    Padding(
                      padding: const EdgeInsets.all(8.0),
                      child: OnTheRiseOpportunityCard(
                        opportunity: opportunity,
                      ),
                    ),
                  // For testing, you can also display the other list
                  for (var opportunity in opportunitiesList)
                    Padding(
                      padding: const EdgeInsets.all(8.0),
                      child: Text(opportunity.name),
                    ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}