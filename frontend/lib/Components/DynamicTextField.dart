import 'package:flutter/material.dart';
import 'package:flutter/services.dart'; // Required for TextInputFormatter

class DynamicTextField extends StatelessWidget {
  final String label;
  final TextEditingController controller;
  final int maxLines;
  final TextInputType? inputType;
  final List<TextInputFormatter>? inputFormatters; // New optional parameter

  const DynamicTextField({
    super.key,
    required this.label,
    required this.controller,
    this.maxLines = 1,
    this.inputType,
    this.inputFormatters,
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
      inputFormatters: inputFormatters, // Apply inputFormatters
    );
  }
}
