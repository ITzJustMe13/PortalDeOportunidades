import 'package:flutter/material.dart';

class DynamicActionButton extends StatelessWidget {
  final void Function()? onPressed;
  final String text;
  final IconData icon;
  final Color color;
  final double thresholdWidth;

  const DynamicActionButton({
    super.key,
    required this.onPressed,
    required this.text,
    required this.icon,
    required this.color,
    this.thresholdWidth = 100.0, // Default threshold width
  });

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        if (constraints.maxWidth < thresholdWidth) {
          // Display icon only when below the threshold
          return IconButton(
            color: color,
            onPressed: onPressed,
            icon: Icon(icon),
          );
        } else {
          // Display full button with text when above the threshold
          return TextButton(
            onPressed: onPressed,
            style: TextButton.styleFrom(
              backgroundColor: color,
              padding: EdgeInsets.symmetric(horizontal: 12, vertical: 4),
            ),
            child: Text(
              text,
              style: TextStyle(fontSize: 12, color: Colors.white),
            ),
          );
        }
      },
    );
  }
}
