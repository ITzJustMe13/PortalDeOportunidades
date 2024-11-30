import 'package:flutter/material.dart';
import 'package:frontend/Components/DynamicActionButton.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'dart:convert'; // For Base64 decoding
import 'package:frontend/Views/OpportunityDetailsScreen.dart';
import 'package:frontend/Views/EditOpportunityScreen.dart';
import 'package:frontend/Views/OpportunityManager.dart';
import 'package:provider/provider.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:frontend/Components/ConfirmationDialog.dart';

class OpportunityManageCard extends StatefulWidget {
  final Opportunity opportunity;

  const OpportunityManageCard({
    super.key,
    required this.opportunity,
  });

  @override
  _OpportunityManageCardState createState() => _OpportunityManageCardState();
}

class _OpportunityManageCardState extends State<OpportunityManageCard> {
  late bool isActive; // Local state for active/inactive

  @override
  void initState() {
    super.initState();
    isActive = widget.opportunity.isActive; // Initialize state with opportunity's status
  }

  void toggleStatus(bool activate) async {
    try {
      final bool success = activate
          ? await Provider.of<OpportunityApiHandler>(context, listen: false)
              .activateOpportunity(widget.opportunity.opportunityId)
          : await Provider.of<OpportunityApiHandler>(context, listen: false)
              .deactivateOpportunity(widget.opportunity.opportunityId);

      if (success) {
        setState(() {
          isActive = activate;
        });
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(activate
                ? 'Oportunidade ativada com sucesso!'
                : 'Oportunidade desativada com sucesso!'),
            backgroundColor: Colors.green,
          ),
        );
      } else {
        _showErrorSnackbar();
      }
    } catch (e) {
      _showErrorSnackbar();
    }
  }


  void _showErrorSnackbar() {
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text('Erro ao alterar o status da oportunidade.'),
        backgroundColor: Colors.red,
      ),
    );
  }

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
                      base64Decode(widget.opportunity.opportunityImgs[0].imageBase64),
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
                    widget.opportunity.category.name.replaceAll("_", " "),
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
                    color: isActive ? Colors.green : Colors.red, // Color based on active status
                    borderRadius: BorderRadius.circular(4.0),
                  ),
                  child: Text(
                    isActive ? 'Ativo' : 'Inativo',
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
                  widget.opportunity.name,
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
                  '${widget.opportunity.vacancies} Vagas Disponíveis',
                  style: TextStyle(fontSize: 14),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
                Text(
                  '${widget.opportunity.price}€ / Pessoa',
                  style: TextStyle(fontSize: 14, color: Colors.grey[700]),
                ),
                Text(
                  'Data: ${widget.opportunity.date}',
                  style: TextStyle(fontSize: 14, color: Colors.grey[600]),
                ),
                Text(
                  '${widget.opportunity.address} / ${widget.opportunity.location.name}',
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
                            builder: (context) => OpportunityDetailsScreen(opportunity: widget.opportunity),
                          ),
                        );
                      },
                    ),

                    // Edit Button
                    DynamicActionButton(
                      text: 'Editar',
                      icon: Icons.edit,
                      color: Colors.green,
                      onPressed: () {
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (context) => EditOpportunityScreen(opportunity: widget.opportunity),
                          ),
                        );
                      },
                    ),

                    // Active/Inactive Button
                    DynamicActionButton(
                      text: isActive ? 'Desativar' : 'Ativar',
                      color: isActive ? Colors.red : Colors.green,
                      icon: isActive ? Icons.visibility_off : Icons.visibility,
                      onPressed: () => toggleStatus(!isActive),
                    ),

                    // Delete Button
                    DynamicActionButton(
                      text: 'Apagar',
                      icon: Icons.delete,
                      color: Colors.red,
                      onPressed: () async {
                        bool confirmDelete = await showDialog<bool>(
                          context: context,
                          builder: (BuildContext context) {
                            return ConfirmationDialog(
                              action: 'apagar',
                              onConfirm: () async {
                                final bool success = await Provider.of<OpportunityApiHandler>(context, listen: false)
                                    .deleteOpportunity(widget.opportunity.opportunityId);

                                return success; // Return the result of the delete operation
                              },
                            );
                          },
                        ) ?? false;

                        if (confirmDelete) {
                          ScaffoldMessenger.of(context).showSnackBar(
                            const SnackBar(
                              content: Text('Oportunidade apagada com sucesso!'),
                              backgroundColor: Colors.green,
                            ),
                          );

                          Navigator.pushReplacement(
                            context,
                            MaterialPageRoute(builder: (context) => OpportunityManagerScreen()),
                          );
                        } else {
                          ScaffoldMessenger.of(context).showSnackBar(
                            const SnackBar(
                              content: Text('Falha ao apagar a oportunidade.'),
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
