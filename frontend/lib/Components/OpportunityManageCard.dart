import 'package:flutter/material.dart';
import 'package:frontend/Components/DynamicActionButton.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'dart:convert'; // For Base64 decoding
import 'package:frontend/Views/OpportunityDetailsScreen.dart';
import 'package:frontend/Views/EditOpportunityScreen.dart';
import 'package:provider/provider.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';

class OpportunityManageCard extends StatelessWidget {
  final Opportunity opportunity;

  const OpportunityManageCard({
    super.key,
    required this.opportunity,
  });

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
              // Active/Inactive Tag
              Positioned(
                bottom: 10,
                left: 10,
                child: Container(
                  padding: EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: opportunity.isActive ? Colors.green : Colors.red, // Color based on active status
                    borderRadius: BorderRadius.circular(4.0),
                  ),
                  child: Text(
                    opportunity.isActive ? 'Ativo' : 'Inativo',
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
                    DynamicActionButton(
                      text: 'Detalhes',
                      icon: Icons.details,
                      color: Color(0xFF50C878),
                      onPressed: () {
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (context) => OpportunityDetailsScreen(opportunity: opportunity),
                          ),
                        );
                      },
                    ),

                    DynamicActionButton(
                      text: 'Editar',
                      icon: Icons.edit,
                      color: Colors.green,
                      onPressed: () {
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (context) => EditOpportunityScreen(opportunity: opportunity),
                          ),
                        );
                      },
                    ),

                    DynamicActionButton(
                      text: 'Ativo/Inativo',
                      color: Colors.yellow,
                      icon: Icons.adjust,
                      onPressed: () {
                        
                      },
                    ),

                    // Delete Button
                    DynamicActionButton(
                      text: 'Apagar',
                      icon: Icons.delete,
                      color: Colors.red,
                      onPressed: () async {
                        final bool success = await Provider.of<OpportunityApiHandler>(context, listen: false)
                            .deleteOpportunity(opportunity.opportunityId);

                        if (success) {
                          ScaffoldMessenger.of(context).showSnackBar(
                            const SnackBar(
                              content: Text('Oportunidade apagada com sucesso!'),
                              backgroundColor: Colors.green,
                            ),
                          );
                        } else {
                          ScaffoldMessenger.of(context).showSnackBar(
                            const SnackBar(
                              content: Text('Falha ao apagar a oportunidade. Pode possuir ainda reservas para esta Opportunidade, se o erro persistir contacte-nos.'),
                              backgroundColor: Colors.red,
                            ),
                          );
                        }
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
