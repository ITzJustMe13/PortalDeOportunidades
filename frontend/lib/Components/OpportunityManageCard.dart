import 'package:flutter/material.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'dart:convert'; // For Base64 decoding

class OpportunityManageCard extends StatelessWidget {
  final Opportunity opportunity;

  const OpportunityManageCard({
    Key? key,
    required this.opportunity,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Card(
      elevation: 4.0,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8.0),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Image Section with Gray Overlay and Category Tag
          // Category tag (Top left corner)
              Positioned(
                top: 10,
                left: 10,
                child: Container(
                  padding: EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  color: Colors.green,
                  child: Text(
                    opportunity.category.name.replaceAll("_"," "), // Assuming OppCategory is an enum
                    style: TextStyle(
                      color: Colors.white,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ),
          Stack(
            children: [
              // Image (First Image of Opportunity)
              Container(
                height: 180,
                decoration: BoxDecoration(
                  image: DecorationImage(
                    image: MemoryImage(
                      base64Decode(opportunity.opportunityImgs[0].imageBase64), // Decode Base64 image string
                    ),
                    fit: BoxFit.cover,
                  ),
                ),
              ),
              // Gray overlay on top of the image
              Container(
                color: Colors.black.withOpacity(0.4),
                height: 180,
              ),
            ],
          ),
          // Card Body
          Padding(
            padding: const EdgeInsets.all(8.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  opportunity.name,
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                SizedBox(height: 4),
                Text(
                  '${opportunity.vacancies} Vagas Disponíveis',
                  maxLines: 3,
                  overflow: TextOverflow.ellipsis,
                ),
                SizedBox(height: 4),
                Text(
                  '${opportunity.price}€ / Pessoa',
                  maxLines: 3,
                  overflow: TextOverflow.ellipsis,
                ),
                SizedBox(height: 4),
                Text(
                  'Data: ${opportunity.date}',
                  maxLines: 3,
                  overflow: TextOverflow.ellipsis,
                ),
                Text(
                  '${opportunity.address} / ${opportunity.location.name}',
                  maxLines: 3,
                  overflow: TextOverflow.ellipsis,
                ),
                SizedBox(height: 8),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    // Details Button (Left)
                    ElevatedButton(
                      onPressed: () {
                        // Add your action for the details button here
                      },
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.green,
                        foregroundColor: Colors.white, // Background color for details button
                      ),
                      child: Text("Details"),
                    ),
                    SizedBox(width: 75),
                    ElevatedButton(
                      onPressed: () {
                        // Add your action for the delete button here
                      },
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.red,
                        foregroundColor: Colors.white,
                      ),
                      child: Text("Delete"),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
