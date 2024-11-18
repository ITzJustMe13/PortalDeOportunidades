import 'package:flutter/material.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/OpportunityImg.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/OpportunityManageCard.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';

class OpportunityManager extends StatelessWidget {

  @override
  Widget build(BuildContext context) {
    // Define oppImgs here within the build method
    final List<OpportunityImg> oppImgs = [
      OpportunityImg(
        imgId: 1,
        opportunityId: 1,
        imageBase64: 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1PeAAAADUlEQVR42mP8/5+hHgAHggJ/pkT1aQAAAABJRU5ErkJggg==', // Red 1x1 PNG
      ),
      OpportunityImg(
        imgId: 2,
        opportunityId: 1,
        imageBase64: 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1PeAAAADUlEQVR42mP8/5+hHgAHggJ/pkT1aQAAAABJRU5ErkJggg==', // Another red 1x1 PNG (example)
      ),
      OpportunityImg(
        imgId: 3,
        opportunityId: 1,
        imageBase64: 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1PeAAAADUlEQVR42mP8/5+hHgAHggJ/pkT1aQAAAABJRU5ErkJggg==', // Another red 1x1 PNG
      ),
    ];

    final List<Opportunity> _opportunities = [
      Opportunity(
        opportunityId: 1,
        userId: 1, 
        name: 'Opportunity 1', 
        description: 'This is a very nice description Sou lindo', 
        price: 14.99, 
        vacancies: 2,
        isActive: true,
        category: OppCategory.COZINHA_TIPICA,
        location: Location.BRAGA,
        address: "Rua das Flores 21 Braga",
        reviewScore: 4.5,
        date: DateTime.now(),
        isImpulsed: false,
        opportunityImgs: oppImgs,
      ),
      Opportunity(
        opportunityId: 2,
        userId: 1, 
        name: 'Opportunity 2', 
        description: 'This is a very nice description Sou lindo', 
        price: 14.99, 
        vacancies: 2,
        isActive: true,
        category: OppCategory.AGRICULTURA,
        location: Location.BRAGA,
        address: "Rua das Flores 21 Braga",
        reviewScore: 0,
        date: DateTime.now(),
        isImpulsed: false,
        opportunityImgs: oppImgs,
      ),
      Opportunity(
        opportunityId: 3,
        userId: 1, 
        name: 'Opportunity 3', 
        description: 'This is a very nice description Sou lindo', 
        price: 14.99, 
        vacancies: 2,
        isActive: true,
        category: OppCategory.DESPORTOS_ATIVIDADES_AO_AR_LIVRE,
        location: Location.BRAGA,
        address: "Rua das Flores 21 Braga",
        reviewScore: 2.5,
        date: DateTime.now(),
        isImpulsed: false,
        opportunityImgs: oppImgs,
      ),
    ];

    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth < 600) {
            // Mobile Layout
            return _buildMobileLayout(_opportunities);
          } else if (constraints.maxWidth < 1200) {
            // Tablet Layout
            return _buildTabletLayout(_opportunities);
          } else {
            // Desktop Layout
            return _buildDesktopLayout(_opportunities);
          }
        },
      ),
    );
  }

  // Mobile Layout (Vertical Scroll)
  Widget _buildMobileLayout(List<Opportunity> opportunities) {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(20.0),
      child: Column(
        children: [
          Text(
            "As suas Oportunidades",
            style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 20),
          // Using List.generate to add spacing between cards
          ...opportunities
              .map((opportunity) => Padding(
                    padding: const EdgeInsets.only(bottom: 20.0),
                    child: OpportunityManageCard(opportunity: opportunity),
                  ))
              .toList(),
        ],
      ),
    );
  }


  // Tablet Layout (Vertical Scroll with Scrollbar)
  Widget _buildTabletLayout(List<Opportunity> opportunities) {
    return Scrollbar(
      thumbVisibility: true,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.all(20.0),
            child: Text(
              "As suas Oportunidades",
              style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
            ),
          ),
          SizedBox(height: 20),
          Expanded(
            child: GridView.builder(
              padding: const EdgeInsets.symmetric(horizontal: 20),
              gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
                crossAxisCount: 2, // Two cards per row
                crossAxisSpacing: 10, // Space between columns
                mainAxisSpacing: 20, // Space between rows
                childAspectRatio: 1.5,
              ),
              itemCount: opportunities.length,
              itemBuilder: (context, index) {
                return OpportunityManageCard(opportunity: opportunities[index]);
              },
            ),
          ),
        ],
      ),
    );
  }





  // Desktop Layout (Both vertical and horizontal scroll with Scrollbar)
  Widget _buildDesktopLayout(List<Opportunity> opportunities) {
    return Scrollbar(
      thumbVisibility: true,
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              "As suas Oportunidades",
              style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
            ),
            SizedBox(height: 20),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                ...opportunities.map((opportunity) {
                  return OpportunityManageCard(opportunity: opportunity);
                }).toList(),
              ],
            )
          ],
        ),
      ),
    );
  }

}