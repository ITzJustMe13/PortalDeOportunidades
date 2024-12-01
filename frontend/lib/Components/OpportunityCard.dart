import 'package:flutter/material.dart';
import 'package:frontend/Components/DynamicActionButton.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Views/OpportunityDetailsScreen.dart';
import 'dart:convert';

class OpportunityCard extends StatelessWidget {
  final Opportunity opportunity;

  const OpportunityCard({
    super.key,
    required this.opportunity,
  });

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        return Card(
          elevation: 4,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              _buildImageSection(),
              _buildDetailsSection(context),
            ],
          ),
        );
      },
    );
  }

  Widget _buildImageSection() {
    return Stack(
      children: [
        opportunity.opportunityImgs.isNotEmpty
      ? Image.memory(
          base64Decode(opportunity.opportunityImgs.first.imageBase64),
          fit: BoxFit.cover,
          height: 150,
          width: double.infinity,
          errorBuilder: (context, error, stackTrace) {
            // Fallback in case the image fails to load
            return Container(
              height: 150,
              width: double.infinity,
              color: Colors.grey,
              child: Center(
                child: Icon(
                  Icons.broken_image,
                  color: Colors.white,
                ),
              ),
            );
          },
        )
      : Container(
          // Fallback if there are no images
          height: 150,
          width: double.infinity,
          color: Colors.grey,
          child: Center(
            child: Icon(
              Icons.image_not_supported,
              color: Colors.white,
            ),
          ),
        ),
        Positioned(
          top: 0,
          left: 0,
          child: Container(
            padding: const EdgeInsets.all(4.0),
            color: Colors.black54,
            child: Text(
              opportunity.category.toString().split('.').last,
              style: TextStyle(
                color: Colors.white,
                fontSize: 14,
              ),
            ),
          ),
        ),
        Positioned(
          bottom: 0,
          left: 0,
          child: Container(
              padding: const EdgeInsets.all(4.0),
              color: Colors.black54,
              child:
                  Column(mainAxisAlignment: MainAxisAlignment.start, children: [
                Text(
                  "${opportunity.price.toString()}€ /Pessoa",
                  style: TextStyle(fontSize: 12, color: Colors.white),
                ),
                Text(
                  opportunity.location.toString().split('.').last,
                  style: TextStyle(fontSize: 14, color: Colors.white),
                ),
              ])),
        ),
      ],
    );
  }

  Widget _buildDetailsSection(BuildContext context) {
    return Container(
      color: Color(0xFFD9D9D9),
      padding: EdgeInsets.all(8.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            opportunity.name,
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
            style: TextStyle(fontSize: 14, fontWeight: FontWeight.bold),
          ),
          SizedBox(
            width: double.infinity,
            child: Wrap(
              spacing: 8,
              crossAxisAlignment: WrapCrossAlignment.center,
              alignment: WrapAlignment.spaceBetween,
              children: [
                
                SizedBox(height: 8),
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
              ],
            ),
          )
        ],
      ),
    );
  }
}
