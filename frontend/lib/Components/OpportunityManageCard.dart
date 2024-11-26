import 'package:flutter/material.dart';
import 'package:frontend/Components/dynamic_active_inactive_button.dart';
import 'package:frontend/Components/dynamic_details_button.dart';
import 'package:frontend/Components/dynamic_delete_button.dart';
import 'package:frontend/Components/dynamic_edit_button.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'dart:convert'; // For Base64 decoding
import 'package:frontend/Views/OpportunityDetailsScreen.dart';
import 'package:frontend/Views/EditOpportunityScreen.dart';
import 'package:intl/intl.dart';

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
      margin: const EdgeInsets.all(8.0), // Adds space between cards
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8.0),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Stack(
            children: [
              // Image
              Container(
                height: 180,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.vertical(
                    top: Radius.circular(8.0),
                  ),
                  image: DecorationImage(
                    image: MemoryImage(
                      base64Decode(opportunity.opportunityImgs[0].imageBase64),
                    ),
                    fit: BoxFit.cover,
                  ),
                ),
              ),
              // Gray Overlay
              Container(
                height: 180,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.vertical(
                    top: Radius.circular(8.0),
                  ),
                  color: Colors.black.withOpacity(0.4),
                ),
              ),
              // Category Tag
              Positioned(
                top: 10,
                left: 10,
                child: Container(
                  padding: EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: Colors.green,
                    borderRadius: BorderRadius.circular(4.0),
                  ),
                  child: Text(
                    opportunity.category.name.replaceAll("_", " "),
                    style: TextStyle(
                      color: Colors.white,
                      fontWeight: FontWeight.bold,
                      fontSize: 12,
                    ),
                  ),
                ),
              ),
            ],
          ),

          // Card Body
          Padding(
            padding: const EdgeInsets.all(12.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Title
                Text(
                  opportunity.name,
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                  ),
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                ),
                SizedBox(height: 8),

                // Details
                Text(
                  '${opportunity.vacancies} Vagas Disponíveis',
                  style: TextStyle(fontSize: 14),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
                Text(
                  '${opportunity.price}€ / Pessoa',
                  style: TextStyle(fontSize: 14, color: Colors.grey[700]),
                ),
                Text(
                  'Data: ${opportunity.date}',
                  style: TextStyle(fontSize: 14, color: Colors.grey[600]),
                ),
                Text(
                  '${opportunity.address} / ${opportunity.location.name}',
                  style: TextStyle(fontSize: 14, color: Colors.grey[600]),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
                SizedBox(height: 12),

                // Action Buttons
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    // Details Button
                    DynamicDetailsButton(
                      onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => OpportunityDetailsScreen(opportunity: opportunity),
                        ),
                      );
                    },
                    ),

                    DynamicEditButton(
                      onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => EditOpportunityScreen(opportunity: opportunity),
                        ),
                      );
                    },
                    ),

                    DynamicActiveInactiveButton(isActive: true, onPressed: (){
                      
                    },),
                    // Delete Button
                    DynamicDeleteButton(
                      onPressed: () {
                       
                    },
                    )
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
