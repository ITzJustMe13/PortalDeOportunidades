import 'package:flutter/material.dart';

// Reusable widget for text fields
class OpportunityTextField extends StatelessWidget {
  final String label;
  final TextEditingController controller;
  final int maxLines;
  final TextInputType? inputType;

  const OpportunityTextField({super.key, 
    required this.label,
    required this.controller,
    this.maxLines = 1,
    this.inputType,
  });

  @override
  Widget build(BuildContext context) {
    return TextField(
      decoration: InputDecoration(
        labelText: label,
        border: OutlineInputBorder(),
      ),
      controller: controller,
      maxLines: maxLines,
      keyboardType: inputType,
    );
  }
}