import 'package:flutter/material.dart';

class OpportunityDetails extends StatelessWidget {
  final String name;
  final String description;

  const OpportunityDetails({
    super.key,
    required this.name,
    required this.description
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          name,
          style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
        ),
        SizedBox(height: 10),
        Text(description, style: TextStyle(fontSize: 16)),
      ],
    );
  }
}
