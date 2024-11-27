import 'package:flutter/material.dart';

class DynamicActiveInactiveButton extends StatelessWidget {
  final void Function()? onPressed;
  final bool isActive; // Add this to control the button's state

  const DynamicActiveInactiveButton({
    super.key,
    this.onPressed,
    required this.isActive, // Ensure isActive is passed when this widget is used
  });

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        // Define a threshold width for switching from text to icon
        double thresholdWidth = 100.0;

        if (constraints.maxWidth < thresholdWidth) {
          // Show IconButton if the screen width is below threshold
          return IconButton(
            color: isActive ? Colors.green : Colors.grey, // Change color based on isActive
            onPressed: onPressed,
            icon: Icon(isActive ? Icons.check : Icons.close), // Change icon based on isActive
          );
        } else {
          // Show TextButton if the screen width is above threshold
          return TextButton(
            onPressed: onPressed,
            style: TextButton.styleFrom(
              backgroundColor: isActive ? Colors.green : Colors.grey, // Change color based on isActive
              padding: EdgeInsets.symmetric(horizontal: 12, vertical: 4),
            ),
            child: Text(
              isActive ? "Ativo" : "Inativo", // Change text based on isActive
              style: TextStyle(fontSize: 12, color: Colors.white),
            ),
          );
        }
      },
    );
  }
}
