import 'package:flutter/material.dart';
import 'package:frontend/Components/on_the_rise_opportunity_card.dart';
import 'package:frontend/Components/opportunity_card.dart';
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

          return ListView(
            padding: const EdgeInsets.all(padding),
            children: [
              Center(
                child: SizedBox(
                    width: componentWidth,
                    child: Column(
                      children: [
                        for (var opportunity in opportunitiesOnTheRiseList)
                          Padding(
                            padding: const EdgeInsets.all(8.0),
                            child: OnTheRiseOpportunityCard(
                              opportunity: opportunity,
                            ),
                          ),
                        SizedBox(height: padding),
                        Divider(thickness: 1, color: Colors.black),
                        SizedBox(height: padding),
                        GridView.builder(
                          shrinkWrap: true,
                          physics: NeverScrollableScrollPhysics(),
                          gridDelegate:
                              SliverGridDelegateWithMaxCrossAxisExtent(
                            maxCrossAxisExtent: 400,
                            crossAxisSpacing: 8.0,
                            mainAxisSpacing: 8.0,
                          ),
                          itemCount: opportunitiesList.length,
                          itemBuilder: (context, index) {
                            return Padding(
                              padding: const EdgeInsets.all(8.0),
                              child: OpportunityCard(
                                  opportunity: opportunitiesList[index]),
                            );
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
